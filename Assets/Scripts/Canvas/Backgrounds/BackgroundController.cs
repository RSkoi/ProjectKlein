using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BackgroundController : MonoBehaviour
{
    private BackgroundTransition _transition;
    private UnityAction callback;

    [Tooltip("The raw image inside the background container on the canvas.")]
    public RawImage image;
    private Animation _transitionAnimation;
    [Tooltip("The transition proxy for background transition.")]
    public RawImage transitionImage;

    public void Start()
    {
        if (!PlayerSingleton.Instance.sceneDirectorComponent.idle) {
            _transition = PlayerSingleton.Instance.backgroundTransition;
            _transition.OnFadeOutDone.AddListener(FadeOutDoneWorker);
        }
        _transitionAnimation = image.GetComponent<Animation>();
    }

    public void ShowBackground(Texture texture)
    {
        image.texture = texture;
    }

    public void TransitionBackground(Texture texture)
    {
        transitionImage.texture = texture;

        _transitionAnimation.Stop();
        _transitionAnimation.Play();
        StartCoroutine(TransitionBackgroundWait(texture));
    }

    private IEnumerator TransitionBackgroundWait(Texture texture)
    {
        yield return new WaitForSeconds(_transitionAnimation.clip.length - 0.1f);

        image.texture = texture;
        image.color = Color.white;
        _transitionAnimation.Stop();
    }

    public void RegisterFadeOutCallbackAndFade(UnityAction call)
    {
        callback = call;
        _transition.FadeToEnd();
    }

    public void FadeOutDoneWorker()
    {
        if (callback != null)
        {
            callback.Invoke();
            callback = null;
        }
    }
}
