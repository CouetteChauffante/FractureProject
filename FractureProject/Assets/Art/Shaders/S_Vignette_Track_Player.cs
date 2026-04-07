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
        var dir = Camera.transform.position - transform.position;
        var ray = new Ray(transform.position, dir.normalized);

        if (Physics.Raycast(ray, 3000, Mask))
        {
            wallMaterial.SetFloat(SizeID, 1);
            Debug.DrawLine(transform.position, Vector3.forward, Color.green);
        }

        else
        {
            wallMaterial.SetFloat(SizeID, 0);
            Debug.DrawLine(transform.position, Vector3.forward, Color.red);
        }

    var view = Camera.WorldToViewportPoint(transform.position);
        wallMaterial.SetVector(PosID, view);
    }
}
