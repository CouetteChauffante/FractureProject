using UnityEngine;
using UnityEngine.UIElements;

public class S_Vignette_Track_Player : MonoBehaviour
{
    public static int PosID = Shader.PropertyToID("_Position");
    public static int SizeID = Shader.PropertyToID("_Size");

    public Material wallMaterial;
    public Camera Camera;
    public LayerMask Mask;

    void Update()
    {
        var dir = transform.position - Camera.transform.position;
        var ray = new Ray(Camera.transform.position, dir.normalized);

        if (Physics.Raycast(ray, out RaycastHit hit, 3000f, Mask))
        {
            wallMaterial.SetFloat(SizeID, 1);
            Debug.DrawLine(Camera.transform.position, hit.point, Color.green);
        }
        else
        {
            wallMaterial.SetFloat(SizeID, 0);
            Debug.DrawLine(Camera.transform.position, transform.position, Color.red);
        }

        var view = Camera.WorldToViewportPoint(transform.position);
        wallMaterial.SetVector(PosID, view);
    }
}