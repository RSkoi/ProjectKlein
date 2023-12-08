using UnityEngine;
using UnityEngine.Events;

public class ProgressController : MonoBehaviour
{
    public UnityEvent OnProgress = new();
    public UnityEvent<bool> OnAutoProgress = new();

    public void Progress()
    {
        OnProgress.Invoke();
    }

    public void AutoProgress(bool inputStarted)
    {
        OnAutoProgress.Invoke(inputStarted);
    }
}
