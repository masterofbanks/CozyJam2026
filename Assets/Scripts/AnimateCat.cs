using UnityEngine;
using static PlayerInput;

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

    public Directions UpdateDirectionState(Vector2 dir)
    {
        Directions CurrentDirection = Directions.Down;
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if (dir.x < 0)
            {
                CurrentDirection = Directions.Left;
            }
            else
            {
                CurrentDirection = Directions.Right;
            }
        }
        else
        {
            if (dir.y > 0)
            {
                CurrentDirection = Directions.Up;
            }
            else
            {
                CurrentDirection = Directions.Down;
            }
        }

        return CurrentDirection;
    }

}
