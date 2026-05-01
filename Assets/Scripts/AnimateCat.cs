using UnityEngine;

public class AnimateCat
{
    private Animator animator;
    public AnimateCat(Animator animator)
    {
        this.animator = animator;
    }
    public void SendAnimationInformation(PlayerInput.Directions direction, bool moving)
    {
        animator.SetInteger("Direction", (int)direction);
        animator.SetBool("Moving", moving);
    }
    
}
