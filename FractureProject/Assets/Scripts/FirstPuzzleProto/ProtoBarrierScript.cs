using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProtoBarrierScript : MonoBehaviour
{
    private bool isPlayerClose;
    [SerializeField] private float xAxisMax;
    private float xAxisDistance;
    private Vector3 targetPos;
    
    private void Start()
    {
        xAxisDistance = xAxisMax-transform.position.x;
        targetPos = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))        isPlayerClose = true;       
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))        isPlayerClose = false;       
    }

    private void Update()
    {
        if (isPlayerClose && (Input.GetKeyDown(KeyCode.Q)||Input.GetButtonDown("Fire1")))
        {
            if(Mathf.Abs(xAxisMax-targetPos.x)>0.1f) targetPos += new Vector3(xAxisDistance/3, 0, 0);
        }
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * 10f);
    }
}
