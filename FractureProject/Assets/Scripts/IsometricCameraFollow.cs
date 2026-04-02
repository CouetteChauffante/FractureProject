using System;
using UnityEngine;

public class IsometricCameraFollow : MonoBehaviour
{
    public static IsometricCameraFollow instance { get; private set; }
    
    public float smoothTime = 0.1f;
    
    private Transform currentTarget;
    private Vector3 offset;
    private Vector3 velocity = Vector3.zero;
    
    private void Awake()
    {
        if (instance != null)
            throw new Exception("Multiple camera in scene");
        
        instance = this;
    }

    private void Start()
    {
        if (Player.instance == null)
            throw new Exception("No Player in scene");
        
        currentTarget = Player.instance.transform;
        
        offset = transform.position - currentTarget.position;
    }

    private void LateUpdate()
    {
        if (currentTarget == null) return;

        Vector3 desiredPosition = currentTarget.position + offset;

        transform.position = Vector3.SmoothDamp(
            transform.position, 
            desiredPosition, 
            ref velocity, 
            smoothTime
        );
    }

    public void ChangeTarget(Transform newTarget)
    {
        if (newTarget == null) return;
        
        currentTarget = newTarget;
    }
}