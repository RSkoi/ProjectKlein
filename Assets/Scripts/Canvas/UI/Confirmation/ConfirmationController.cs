using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ConfirmationController : MonoBehaviour
{
    private PlayerInput _playerInput;

    [Tooltip("The confirmation window container.")]
    public GameObject confirmationWindow;
    [Tooltip("The UI blocker/tint panel.")]
    public GameObject uiBlockPanel;
    [Tooltip("The confirmation description textbox.")]
    public TMP_Text confirmationDescTextbox;
    [Tooltip("The confirmation yes textbox.")]
    public TMP_Text confirmationYesTextbox;
    [Tooltip("The confirmation no textbox.")]
    public TMP_Text confirmationNoTextbox;

    [Tooltip("The default description text of the confirmation window")]
    public string defaultText = "Are you sure?";
    [Tooltip("The default yes-button text of the confirmation window")]
    public string defaultYesText = "Yes";
    [Tooltip("The default no-button text of the confirmation window")]
    public string defaultNoText = "No";

    public delegate void YesCallback();
    public delegate void NoCallback();
    private YesCallback yesCallback;
    private NoCallback noCallback;

    public void Start()
    {
        _playerInput = PlayerSingleton.Instance.playerInput;
    }

    public void Confirm(YesCallback yesCallback = null, NoCallback noCallback = null)
    {
        WindowInit(defaultText, defaultYesText, defaultNoText, yesCallback, noCallback);
    }

    public void Confirm(string descText, string yesText, string noText, YesCallback yesCallback = null, NoCallback noCallback = null)
    {
        WindowInit(descText, yesText, noText, yesCallback, noCallback);
    }

    private void WindowInit(string descText, string yesText, string noText, YesCallback yesCallback = null, NoCallback noCallback = null)
    {
        this.yesCallback = yesCallback;
        this.noCallback = noCallback;
        confirmationDescTextbox.text = descText;
        confirmationYesTextbox.text = yesText;
        confirmationNoTextbox.text = noText;
        ToggleWindow();
    }

    private void ToggleWindow()
    {
        if (confirmationWindow.activeSelf)
        {
            ResetCallbacks();
            uiBlockPanel.SetActive(false);
            confirmationWindow.SetActive(false);
            SwitchToBaseMap();
        }
        else
        {
            uiBlockPanel.SetActive(true);
            confirmationWindow.SetActive(true);
            SwitchToBlankMap();
        }
    }

    public void ConfirmYes()
    {
        Debug.Log("ConfirmationController: yes");

        yesCallback?.Invoke();
        ToggleWindow();
    }

    public void ConfirmNo()
    {
        Debug.Log("ConfirmationController: no");

        noCallback?.Invoke();
        ToggleWindow();
    }

    private void ResetCallbacks()
    {
        yesCallback = null;
        noCallback = null;
    }

    private void SwitchToBlankMap()
    {
        _playerInput.SwitchCurrentActionMap("BlankMap");
    }

    private void SwitchToBaseMap()
    {
        _playerInput.SwitchCurrentActionMap("BaseMap");
    }
}
