using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    public Animator animator;
    
    public void SetDirection(Vector2 direction)
    {
        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);
    }
    
    public void SetMovement(bool isMoving) => animator.SetBool("isMoving", isMoving);
    
    public void SetTransportedState(bool isTransported) => animator.SetBool("isTransported", isTransported);
    
    
}
