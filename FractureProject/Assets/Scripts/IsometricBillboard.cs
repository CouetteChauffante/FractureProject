using UnityEngine;

public class IsometricBillboard : MonoBehaviour
{
    private Transform camTransform;

    void Start()
    {
        if (Camera.main != null)
            camTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        Vector3 targetPostion = camTransform.position;

        targetPostion.y = transform.position.y;

        transform.LookAt(targetPostion);
    }
}