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
        Transported,
        Ejected
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
        rb = GetComponent<Rigidbody>();
    }

    public float moveSpeed;
    
    private Vector3 direction;
    private Vector3 skewedDirection;

    public float crowdSpeed;

    private CrowdNode targetCrowdPoint;
    
    private Vector3 lastPositionAllowed;
    
    public float ejectionDistance; 
    public float ejectionSpeed;
    private Vector3 ejectionDirection;
    private Vector3 ejectionTargetPosition;

    private Rigidbody rb;

    void Update()
    {
        lastPositionAllowed = transform.position;
        
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        
        direction = new Vector3(h, 0, v).normalized;
        
        if (currentState != States.Transported && currentState != States.Ejected)
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
            case States.Ejected:
                ApplyEjection();
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

        rb.MovePosition(transform.position + skewedDirection * moveSpeed * Time.deltaTime);
    }

    public void FollowCrowd()
    {
        transform.position = Vector3.MoveTowards(
            transform.position, 
            targetCrowdPoint.position, 
            crowdSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetCrowdPoint.position) < 0.1f)
        {
            if (targetCrowdPoint.nextNode is ExitCrowdNode)
            {
                ejectionTargetPosition = transform.position + (ejectionDirection * ejectionDistance);
                ChangeState(States.Ejected);
            }
            else
            {
                if (targetCrowdPoint.nextNode.nextNode is ExitCrowdNode)
                    ejectionDirection = (targetCrowdPoint.nextNode.position - targetCrowdPoint.position).normalized;
                
                targetCrowdPoint = targetCrowdPoint.nextNode;
            }
        }
    }

    private void ApplyEjection()
    {
        transform.position = Vector3.MoveTowards(
            transform.position, 
            ejectionTargetPosition, 
            ejectionSpeed * Time.deltaTime
        );
        
        if (Vector3.Distance(transform.position, ejectionTargetPosition) < 0.01f)
        {
            ChangeState(States.Idle);
        }
    }

    public void SetCrowdToFollow(CrowdNode startNode)
    {
        if (currentState == States.Transported || currentState == States.Ejected) return;
        
        if (startNode.nextNode.nextNode is ExitCrowdNode)
            ejectionDirection = (startNode.nextNode.position - startNode.position).normalized;

        targetCrowdPoint = startNode.nextNode;
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

