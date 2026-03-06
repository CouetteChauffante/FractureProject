using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    public Animator animator;

    private static readonly int IsMovingHash = Animator.StringToHash("isMoving");
    private static readonly int IsTransportedHash = Animator.StringToHash("isTransported");
    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int MoveYHash = Animator.StringToHash("MoveY");

    public void OnStateChanged(Player.States newState)
    {
        animator.SetBool(IsMovingHash, newState == Player.States.Walking);
        animator.SetBool(IsTransportedHash, newState == Player.States.Transported);
    }

    public void UpdateMoveDirection(float dirX, float dirY)
    {
        animator.SetFloat(MoveXHash, dirX);
        animator.SetFloat(MoveYHash, dirY);
    }
}
