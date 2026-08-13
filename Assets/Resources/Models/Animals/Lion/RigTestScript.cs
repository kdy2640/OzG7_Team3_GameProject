using UnityEngine;

public class RigTestScript : MonoBehaviour
{
    public Animator animator;

    private void ResetAllAnimation()
    {
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsServing", false);
        animator.SetBool("IsTyping", false);
    }

    public void PlayWalk()
    {
        ResetAllAnimation();
        animator.SetBool("IsWalking", true);
    }
    public void PlayRun()
    {
        ResetAllAnimation();
        animator.SetBool("IsRunning", true);
    }
    public void PlayServe()
    {
        ResetAllAnimation();
        animator.SetBool("IsServing", true);
    }
    public void PlayType()
    {
        ResetAllAnimation();
        animator.SetBool("IsTyping", true);
    }

    public void PlayIdle()
    {
        ResetAllAnimation();
    }
}
