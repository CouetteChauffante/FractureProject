using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance { get; private set; }
    
    private AnimatorController animatorController;
    
    public enum States
    {
        Idle,
        Walking,
        Transported
    }
    
    public States currentState = States.Idle;

    private void Awake()
    {
        if (instance != null)
            throw new Exception("Multiple players in scene");
        
        instance = this;
    }

    private void Start()
    {
        animatorController = GetComponent<AnimatorController>();
    }

    public float moveSpeed = 5f;
    
    private Vector3 direction;
    private Vector3 skewedDirection;

    public float crowdSpeed = 15f;

    private CrowdNode targetCrowdPoint;
    
    
    public float ejectionDistance = 1.5f;

    public Vector3 ejectionDirection;

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        
        direction = new Vector3(h, 0, v).normalized;
        
        if (currentState != States.Transported)
        {
            ChangeState(direction.magnitude > 0.1f ? States.Walking : States.Idle);
        }

        switch (currentState)
        {
            case States.Walking: 
                Move();
                animatorController.UpdateMoveDirection(direction.x, direction.z);
                break;
            case States.Transported: 
                FollowCrowd(); 
                break;
        }
    }

    public void ChangeState(States newState)
    {
        if (currentState == newState) return;
        
        currentState = newState;
        animatorController.OnStateChanged(newState);
    }

    public void Move()
    {
        skewedDirection = Quaternion.Euler(0, 45, 0) * direction;

        transform.Translate(skewedDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    public void FollowCrowd()
    {
        if (targetCrowdPoint == null)
        {
            transform.position += ejectionDirection * ejectionDistance;
            ChangeState(States.Idle);
            return;
        }
            
        transform.position = Vector3.MoveTowards(
            transform.position, 
            targetCrowdPoint.position, 
            crowdSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetCrowdPoint.position) < 0.1f)
            targetCrowdPoint = targetCrowdPoint.nextNode;
    }

    public void SetCrowdToFollow(CrowdNode nextNode)
    {
        if (currentState == States.Transported) return;
            
        targetCrowdPoint = nextNode;
        ChangeState(States.Transported);
    }
    
}

