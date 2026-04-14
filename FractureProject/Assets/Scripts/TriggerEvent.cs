using System;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent onTriggerEnterAction, /*nico*/ onTriggerExitAction;
    
    [SerializeField] private string targetTag;
    
     public bool isTalking;
    
    public Dialogs dialogs;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            onTriggerEnterAction?.Invoke();
            isTalking = true;
        }
    }
    
    //Nico
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            onTriggerExitAction?.Invoke();
            isTalking = false;
        }
    }
    
    //Emma
    public void TriggerDialog ()
    {
        FindObjectOfType<DialogManager>().StartDialogue(dialogs);
    }
}