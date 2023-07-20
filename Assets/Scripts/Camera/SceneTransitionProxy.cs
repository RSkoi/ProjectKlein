using UnityEngine;

public class SceneTransitionProxy : MonoBehaviour
{
    private SceneTransition _sceneTransition;

    public void Start()
    {
        _sceneTransition = PlayerSingleton.Instance.sceneTransition;
    }

    public void FadeOutScene()
    {
        _sceneTransition.FadeOutScene();
    }

    public void FadeInScene()
    {
        _sceneTransition.FadeInScene();
    }
}
