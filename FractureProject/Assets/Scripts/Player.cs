using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
            throw new Exception("Multiple players in scene");
        
        instance = this;
    }
    
    public float moveSpeed = 5f;
    
    private Vector3 input;
    private Vector3 skewedInput;

    public bool canMove { get; private set; } = true;

    private float crowdSpeed = 15f;

    private CrowdNode targetCrowdPoint;

    void Update()
    {
        if (canMove)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            input = new Vector3(h, 0, v).normalized;

            skewedInput = Quaternion.Euler(0, 45, 0) * input;

            if (input.magnitude > 0.1f)
            {
                transform.Translate(skewedInput * moveSpeed * Time.deltaTime, Space.World);
            }
        }
        else
        {
            if (targetCrowdPoint == null)
            {
                canMove = true;
                return;
            }
            
            transform.position = Vector3.MoveTowards(
                transform.position, 
                targetCrowdPoint.position, 
                crowdSpeed * Time.deltaTime
            );

            if (transform.position == targetCrowdPoint.position)
                targetCrowdPoint = targetCrowdPoint.nextNode;
        }
    }

    public void FollowCrowd(CrowdNode nextNode)
    {
        targetCrowdPoint = nextNode;
        canMove = false;
    }
}
