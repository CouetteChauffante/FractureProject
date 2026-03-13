using UnityEngine;

public class DynamicNode : MonoBehaviour
{
    public Transform refStart;
    public Transform refEnd;
    public LayerMask mask;
    public bool isForward;

    void Update()
    {
        if (refStart == null || refEnd == null) return;

        Vector3 direction = refEnd.position - refStart.position;
        float distance = direction.magnitude;

        if (Physics.Raycast(refStart.position, direction, out RaycastHit hit, distance, mask))
        {
            if (isForward) transform.position = hit.point;
            else
            {
                if (Physics.Raycast(refEnd.position, -direction, out RaycastHit hitBack, distance, mask))
                {
                    //transform.position = hitBack.point;
                }
            }
        }
    }
}