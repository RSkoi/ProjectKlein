using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BackgroundController : MonoBehaviour
{
    private BackgroundTransition _transition;
    private UnityAction callback;

    [Tooltip("The raw image inside the background container on the canvas.")]
    public RawImage image;

    public void Start()
    {
        _transition = PlayerSingleton.Instance.backgroundTransition;
        _transition.OnFadeOutDone.AddListener(FadeOutDoneWorker);
    }

    public void ShowBackground(Texture texture)
    {
        image.texture = texture;
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
