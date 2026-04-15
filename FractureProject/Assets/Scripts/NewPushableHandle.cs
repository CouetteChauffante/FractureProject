using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewPushableHandle : MonoBehaviour
{
    private NewPushableObject obj;

    [Header("Controller Vibration Settings")]
    [Range(0f, 1f), Tooltip("Vibration lourde")]
    public float lowFrequency;
    [Range(0f, 1f), Tooltip("Vibration légere")]
    public float highFrequency;
    public float rumbleDuration;

    private void Start()
    {
        obj = GetComponentInParent<NewPushableObject>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            obj.isPlayerNear = true;
            StartCoroutine(Rumble());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            obj.isPlayerNear = false;
        }
    }

    private IEnumerator Rumble()
    {
        Debug.Log("Rumble");
        
        Gamepad gamepad = Gamepad.current;

        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
            Debug.Log(lowFrequency+ ", " + highFrequency);
            yield return new WaitForSeconds(rumbleDuration);
            Debug.Log("Ca fait " + rumbleDuration);
            gamepad.PauseHaptics();
        }
    }
}
