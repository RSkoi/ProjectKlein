using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SceneTransition : MonoBehaviour
{
    private AudioController _audioController;

    [Tooltip("Event invoked on finishing fading from end to start.")]
    public UnityEvent OnFadeStartDone = new();
    [Tooltip("Event invoked on finishing fading from start to end.")]
    public UnityEvent OnFadeEndDone = new();

    [Tooltip("Event invoked on finishing fading in scene.")]
    public UnityEvent OnFadeInSceneDone = new();
    [Tooltip("Event invoked on finishing fading out scene.")]
    public UnityEvent OnFadeOutSceneDone = new();

    [Tooltip("The fade out animation component.")]
    public Animation fadeOutAnimation;
    [Tooltip("The fade in animation component.")]
    public Animation fadeInAnimation;

    [Tooltip("The duration of the bg music audio fade on fading out.")]
    public float fadeOutBgMusicDuration = 1.5f;
    [Tooltip("The duration of the bg music audio fade on fading in.")]
    public float fadeInBgMusicDuration = 1.5f;

    public void Start()
    {
        _audioController = PlayerSingleton.Instance.audioController;
    }

    #region simple fade
    public void FadeInScene()
    {
        FadeIn();
        _audioController.FadeInBgSong(fadeInBgMusicDuration);
        _audioController.FadeInEffects(fadeInBgMusicDuration);

        StartCoroutine(FadeInSceneRoutine());
    }

    private IEnumerator FadeInSceneRoutine()
    {
        yield return new WaitForSeconds(fadeInAnimation.clip.length - 0.1f);

        OnFadeInSceneDone.Invoke();
    }

    public void FadeOutScene()
    {
        FadeOut();
        _audioController.FadeOutBgSong(fadeOutBgMusicDuration);
        _audioController.FadeOutEffects(fadeOutBgMusicDuration);

        StartCoroutine(FadeOutSceneRoutine());
    }

    private IEnumerator FadeOutSceneRoutine()
    {
        yield return new WaitForSeconds(fadeOutAnimation.clip.length - 0.1f);

        OnFadeOutSceneDone.Invoke();
    }
    #endregion

    #region combined fade in/out
    public void FadeToEnd()
    {
        FadeOut();

        StartCoroutine(FadeToEndRoutine());
    }

    private IEnumerator FadeToEndRoutine()
    {
        yield return new WaitForSeconds(fadeOutAnimation.clip.length - 0.1f);

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
    #endregion

    public void FadeOut()
    {
        if (fadeOutAnimation == null)
            return;

        fadeOutAnimation.Stop();
        fadeOutAnimation.Play();
    }

    public void FadeIn()
    {
        if (fadeInAnimation == null)
            return;

        fadeInAnimation.Stop();
        fadeInAnimation.Play();
    }
}
