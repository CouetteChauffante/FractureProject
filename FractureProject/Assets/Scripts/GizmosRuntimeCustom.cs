using UnityEngine;

public static class GizmosRuntimeCustom
{
    public static Color color = Color.white;

    private const float LineThickness = 0.05f;

    private static Mesh cubeMesh;
    private static Mesh sphereMesh;
    private static Material gizmoMaterial;
    private static MaterialPropertyBlock propertyBlock;

    private static void InitIfNeeded()
    {
        if (propertyBlock == null)
        {
            propertyBlock = new MaterialPropertyBlock();
        }

        if (gizmoMaterial == null)
        {
            gizmoMaterial = new Material(Shader.Find("Sprites/Default"));
        }

        if (cubeMesh == null)
        {
            GameObject tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubeMesh = tempCube.GetComponent<MeshFilter>().sharedMesh;
            Object.Destroy(tempCube);
        }

        if (sphereMesh == null)
        {
            GameObject tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphereMesh = tempSphere.GetComponent<MeshFilter>().sharedMesh;
            Object.Destroy(tempSphere);
        }
    }

    public static void DrawCube(Vector3 center, Vector3 size)
    {
        InitIfNeeded();
        propertyBlock.SetColor("_Color", color);
        Matrix4x4 matrix = Matrix4x4.TRS(center, Quaternion.identity, size);
        
        Graphics.DrawMesh(cubeMesh, matrix, gizmoMaterial, 0, null, 0, propertyBlock);
    }

    public static void DrawSphere(Vector3 center, float radius)
    {
        InitIfNeeded();
        propertyBlock.SetColor("_Color", color);
        
        Matrix4x4 matrix = Matrix4x4.TRS(center, Quaternion.identity, Vector3.one * (radius * 2f));
        
        Graphics.DrawMesh(sphereMesh, matrix, gizmoMaterial, 0, null, 0, propertyBlock);
    }

    public static void DrawWireCube(Vector3 center, Vector3 size)
    {
        Vector3 half = size / 2f;
        
        Vector3 p0 = center + new Vector3(-half.x, -half.y, -half.z);
        Vector3 p1 = center + new Vector3(half.x, -half.y, -half.z);
        Vector3 p2 = center + new Vector3(half.x, -half.y, half.z);
        Vector3 p3 = center + new Vector3(-half.x, -half.y, half.z);
        Vector3 p4 = center + new Vector3(-half.x, half.y, -half.z);
        Vector3 p5 = center + new Vector3(half.x, half.y, -half.z);
        Vector3 p6 = center + new Vector3(half.x, half.y, half.z);
        Vector3 p7 = center + new Vector3(-half.x, half.y, half.z);

        DrawLine(p0, p1); DrawLine(p1, p2); DrawLine(p2, p3); DrawLine(p3, p0);
        DrawLine(p4, p5); DrawLine(p5, p6); DrawLine(p6, p7); DrawLine(p7, p4);
        DrawLine(p0, p4); DrawLine(p1, p5); DrawLine(p2, p6); DrawLine(p3, p7);
    }

    public static void DrawLine(Vector3 start, Vector3 end)
    {
        InitIfNeeded();
        Vector3 direction = end - start;
        float distance = direction.magnitude;

        if (distance == 0f) return; // Sécurité

        Vector3 lineCenter = start + (direction / 2f);
        Quaternion rotation = Quaternion.LookRotation(direction);
        Vector3 scale = new Vector3(LineThickness, LineThickness, distance);

        propertyBlock.SetColor("_Color", color);
        Matrix4x4 matrix = Matrix4x4.TRS(lineCenter, rotation, scale);
        
        Graphics.DrawMesh(cubeMesh, matrix, gizmoMaterial, 0, null, 0, propertyBlock);
    }
}