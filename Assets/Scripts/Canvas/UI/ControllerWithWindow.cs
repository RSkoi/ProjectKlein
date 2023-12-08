using UnityEngine;

public abstract class ControllerWithWindow : MonoBehaviour
{
    [Tooltip("The window container.")]
    public GameObject window;

    public abstract void ToggleWindow();
    public GameObject GetWindow()
    {
        return window;
    }
}
