using UnityEngine;

public class CameraTargetTrigger : MonoBehaviour
{
    public Transform cameraTargetPoint;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsometricCameraFollow.instance.ChangeTarget(cameraTargetPoint);
        }
    }
}
