using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent onTriggerEnterAction;
    
    [SerializeField] private string targetTag;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            
            Debug.Log("ca trig");
            onTriggerEnterAction?.Invoke();
        }
    }
}