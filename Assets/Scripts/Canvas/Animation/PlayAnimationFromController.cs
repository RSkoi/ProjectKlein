using UnityEngine;

public class PlayAnimationFromController : MonoBehaviour
{
    [Tooltip("The target animator with the animation inside.")]
    public Animator animator;
    [Tooltip("The name of the animation state in the controller to be played.")]
    public string animationName;

    public void Start() 
    {
        PlayAnimation(animationName);
    }

    public void PlayAnimation(string name)
    {
        if (animator != null && name != "")
            animator.Play(name);
    }
}
