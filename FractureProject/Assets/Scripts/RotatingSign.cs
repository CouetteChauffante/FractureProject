using System;
using UnityEngine;
using UnityEngine.Events;

public class RotatingSign : MonoBehaviour
{
    public UnityEvent onInteraction;
    private bool isPlayerNear;
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player") isPlayerNear = true;
    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag == "Player") isPlayerNear = false;
    }

    private void Update()
    {
        if (isPlayerNear)
        {
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetButton("Fire1"))
            {
                onInteraction.Invoke();
            }
        }
    }
}
