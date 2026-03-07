using System;
using UnityEngine;

public class IsometricCameraFollow : MonoBehaviour
{
    public float smoothSpeed = 0.125f;
    
    private Transform target;
    private Vector3 offset;

    private void Start()
    {
        if (Player.instance == null)
            throw new Exception("No Player in scene");
        
        target = Player.instance.transform;
        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;
    }
}