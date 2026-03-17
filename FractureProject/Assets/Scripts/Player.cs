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

    public Vector3 ejectionDirection = new Vector3(0, 0, 0);
    
    
    private Vector3 lastPositionAllowed;
    
    private float lastExitTime;

    void Update()
    {
        lastPositionAllowed = transform.position;
        
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
            ChangeState(States.Idle);
            return;
        }

        Vector3 movementVector = targetCrowdPoint.position - transform.position;
    
        if (movementVector.magnitude > 0.01f)
        {
            ejectionDirection = movementVector.normalized;
        }

        transform.position = Vector3.MoveTowards(
            transform.position, 
            targetCrowdPoint.position, 
            crowdSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetCrowdPoint.position) < 0.1f)
        {
            if (targetCrowdPoint is ExitCrowdNode)
            {
                ApplyEjection();
            }
            else
            {
                targetCrowdPoint = targetCrowdPoint.nextNode;
            }
        }
    }

    private void ApplyEjection()
    {
        transform.position += ejectionDirection * ejectionDistance;
    
        targetCrowdPoint = null;
        ChangeState(States.Idle);
    
        ejectionDirection = Vector3.zero;
        
        lastExitTime = Time.time;
    }

    public void SetCrowdToFollow(CrowdNode nextNode)
    {
        if (currentState == States.Transported || Time.time < lastExitTime + 0.2f) return;
            
        targetCrowdPoint = nextNode;
        ChangeState(States.Transported);
    }

    public void BlockByCrowd()
    {
        transform.position = lastPositionAllowed;
        
        if (skewedDirection.magnitude > 0.01f)
        {
            transform.position -= skewedDirection * 0.02f;
        }
    }
    
}

