using UnityEngine;
using UnityEngine.InputSystem;

public class HideUI : MonoBehaviour
{
    private PlayerInput _playerInput;

    [Tooltip("All the UI containers that should be hidden on input")]
    public GameObject[] hideContainers;
    private bool hidden = false;

    void Start()
    {
        _playerInput = PlayerSingleton.Instance.playerInput;
    }

    public void ToggleHide()
    {
        if (!hidden)
            Hide();
        else
            Unhide();
    }

    private void Hide()
    {
        foreach (GameObject go in hideContainers)
            go.SetActive(false);

        SwitchToInspectionMap();

        hidden = true;
    }

    private void Unhide()
    {
        foreach (GameObject go in hideContainers)
            go.SetActive(true);

        SwitchToBaseMap();

        hidden = false;
    }

    private void SwitchToInspectionMap()
    {
        _playerInput.SwitchCurrentActionMap("BgInspectionMap");
    }

    private void SwitchToBaseMap()
    {
        _playerInput.SwitchCurrentActionMap("BaseMap");
    }
}
