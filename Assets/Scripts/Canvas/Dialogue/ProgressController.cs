using UnityEngine;
using UnityEngine.Events;

public class ProgressController : MonoBehaviour
{
    public UnityEvent OnProgress = new();

    public void Progress()
    {
        OnProgress.Invoke();
    }
}
