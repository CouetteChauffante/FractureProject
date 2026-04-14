using System;
using UnityEngine;

public class NewPushableHandle : MonoBehaviour
{
    private NewPushableObject obj;

    private void Start()
    {
        obj = GetComponentInParent<NewPushableObject>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            obj.isPlayerNear = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            obj.isPlayerNear = false;
        }
    }
}
