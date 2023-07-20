using UnityEngine;

public class CameraSingleton : MonoBehaviour
{
    public static CameraSingleton Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        SetupData();
    }

    public GameObject cameraContainer;
    private void SetupData()
    {
        cameraContainer = Camera.main.gameObject;
    }
}
