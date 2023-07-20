using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BackgroundTransition : MonoBehaviour
{
    [Tooltip("Event invoked on finishing fading from end to start.")]
    public UnityEvent OnFadeStartDone = new();
    [Tooltip("Event invoked on finishing fading from start to end.")]
    public UnityEvent OnFadeEndDone = new();

    [Tooltip("Event invoked on finishing fading out. Should be used to switch objects.")]
    public UnityEvent OnFadeOutDone = new();

    [Tooltip("The fade out animation component.")]
    public Animation fadeOutAnimation;
    [Tooltip("The fade in animation component.")]
    public Animation fadeInAnimation;

    public void FadeToEnd()
    {
        FadeOut();

        StartCoroutine(FadeToEndRoutine());
    }

    private IEnumerator FadeToEndRoutine()
    {
        yield return new WaitForSeconds(fadeOutAnimation.clip.length - 0.1f);

        OnFadeOutDone.Invoke();

        FadeIn();

        StartCoroutine(WaitForFadeInToNotify(false));
    }

    public void FadeToStart()
    {
        FadeOut();

        StartCoroutine(FadeToStartRoutine());
    }

    private IEnumerator FadeToStartRoutine()
    {
        yield return new WaitForSeconds(fadeOutAnimation.clip.length - 0.1f);

        OnFadeOutDone.Invoke();

        FadeIn();

        StartCoroutine(WaitForFadeInToNotify(true));
    }

    private IEnumerator WaitForFadeInToNotify(bool notifyStart)
    {
        yield return new WaitForSeconds(fadeInAnimation.clip.length);

        if (notifyStart)
            OnFadeStartDone.Invoke();
        else
            OnFadeEndDone.Invoke();

        fadeInAnimation.Stop();
    }

    private void FadeOut()
    {
        if (fadeOutAnimation == null)
            return;

        fadeOutAnimation.Stop();
        fadeOutAnimation.Play();
    }

    private void FadeIn()
    {
        if (fadeInAnimation == null)
            return;

        fadeInAnimation.Stop();
        fadeInAnimation.Play();
    }
}
