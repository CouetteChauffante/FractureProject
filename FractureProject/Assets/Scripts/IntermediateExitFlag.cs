using UnityEngine;

public class IntermediateExitFlag : MonoBehaviour
{
    [SerializeField] private Vector2 ejectionDirection;

    public Vector2 GetNormalizedDirection() => ejectionDirection.normalized;
    
}