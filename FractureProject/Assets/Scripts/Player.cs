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

    private float crowdSpeed = 15f;

    private CrowdNode targetCrowdPoint;

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        
        direction = new Vector3(h, 0, v).normalized;
        
        currentState = direction.magnitude > 0.1f ? States.Walking : States.Idle;

        switch (currentState)
        {
            case States.Idle:break;
            case States.Walking: Move();break;
            case States.Transported:FollowCrowd();break;
        }
        
        HandleState();
    }

    public void ChangeState(States newState)
    {
        if (currentState == newState) return;
        
        currentState = newState;
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
            ChangeState(States.Idle);
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

    public void SetCrowdToFollow(CrowdNode nextNode)
    {
        currentState = States.Transported;
        targetCrowdPoint = nextNode;
    }

    public void HandleState()
    {
        switch (currentState)
        {
            case States.Idle:
                animatorController.SetMovement(false);
                animatorController.SetTransportedState(false);
                break;
            case States.Walking:
                animatorController.SetMovement(true);
                animatorController.SetDirection(new Vector2(direction.x, direction.z));
                break;
            case States.Transported:
                animatorController.SetTransportedState(true);
                break;
        }
    }}

