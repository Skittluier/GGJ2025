using UnityEngine;

public class Tubes : Obstacle
{
    [SerializeField]
    private Animator animator;

    public void Start()
    {
        TimerManager.AddTimer(AnimateTube, 2);
    }

    public void AnimateTube()
    {
        animator.SetTrigger("Rotate");
        TimerManager.AddTimer(AnimateTube, 5);
    }
}
