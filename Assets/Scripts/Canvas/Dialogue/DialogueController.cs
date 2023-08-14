using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    private SettingsController _settingsController;

    [Tooltip("The dialogue container on the canvas.")]
    public GameObject dialogueContainer;
    [Tooltip("The dialogue text.")]
    public TMP_Text textBox;
    [Tooltip("The dialogue name label text.")]
    public TMP_Text textBoxName;
    [Tooltip("Font size offset for the dialogue name field.")]
    public float textNameFontSizeOffset = 0f;
    private float _originalFontSize;
    //private float _originalNameFontSize;
    [Tooltip("The dialogue bg raw image on the canvas.")]
    public Image dialogueBgImage;
    [Tooltip("Whether the dialogue container is shown by default.")]
    public bool dialogueContainerDefaultVisible = true;
    [Tooltip("Event invoked on finishing writing the last string.")]
    public UnityEvent OnFinishedString = new();

    private Coroutine currentCoroutine;
    // this is here so fontSize increase is not lost after changing it in the settings
    private float currentFontIncrease = 0f;

    private void Awake()
    {
        _originalFontSize = textBox.fontSize;
        //_originalNameFontSize = textBoxName.fontSize;
    }

    void Start()
    {
        _settingsController = PlayerSingleton.Instance.settingsController;
        dialogueContainer.SetActive(dialogueContainerDefaultVisible);

        SetFontSize(_settingsController.settings.fontSize);
    }

    public void StopWriting()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
    }

    public void ShowString(
        string text,
        string name,
        LocalisationNamePosEnum namePos,
        float speed,
        bool isNarrator,
        float sizeIncrease)
    {
        if (text.Equals(""))
        {
            HideDialogueContainer();
            return;
        }

        ShowDialogueContainer();
        if (speed == 0f)
            UpdateTextbox(text, -1, name, namePos, isNarrator, sizeIncrease);
        else
            currentCoroutine = StartCoroutine(WriteString(text, name, namePos, speed, isNarrator, sizeIncrease));
    }

    private IEnumerator WriteString(
        string text,
        string name,
        LocalisationNamePosEnum namePos,
        float speed,
        bool isNarrator,
        float sizeIncrease)
    {
        for (int i = 0; i < text.Length + 1; i++)
        {
            UpdateTextbox(text, i, name, namePos, isNarrator, sizeIncrease);

            yield return new WaitForSeconds(speed);
        }

        OnFinishedString.Invoke();
        currentCoroutine = null;
    }

    private void UpdateTextbox(
        string text,
        int i,
        string name,
        LocalisationNamePosEnum namePos,
        bool isNarrator,
        float sizeIncrease)
    {
        currentFontIncrease = sizeIncrease;
        SetFontSize(_settingsController.settings.fontSize);

        if (i < 0)
            textBox.SetText(text);
        else
            textBox.SetText(text[..i]);

        if (isNarrator)
        {
            HideDialogueBgContainer();
            textBox.alignment = TextAlignmentOptions.Center;
            textBoxName.alignment = TextAlignmentOptions.Center;
        }
        else
        {
            ShowDialogueBgContainer();
            textBox.alignment = TextAlignmentOptions.Left;
            textBoxName.alignment = TextAlignmentOptions.Left;
        }

        textBoxName.SetText(name);
        textBoxName.alignment = MapNamePosToAlignment(namePos);
    }

    private TextAlignmentOptions MapNamePosToAlignment(LocalisationNamePosEnum namePos)
    {
        switch (namePos)
        {
            case LocalisationNamePosEnum.Rightmost:
                return TextAlignmentOptions.Right;
            case LocalisationNamePosEnum.Leftmost:
                return TextAlignmentOptions.Left;
            case LocalisationNamePosEnum.Middle:
                return TextAlignmentOptions.Center;
            default:
                return TextAlignmentOptions.Center;
        }
    }

    public void ShowDialogueContainer()
    {
        dialogueContainer.SetActive(true);
    }

    public void HideDialogueContainer()
    {
        dialogueContainer.SetActive(false);
    }

    public void ShowDialogueBgContainer()
    {
        Color color = dialogueBgImage.color;
        color.a = 0.6f;
        dialogueBgImage.color = color;
    }

    public void HideDialogueBgContainer()
    {
        Color color = dialogueBgImage.color;
        color.a = 0f;
        dialogueBgImage.color = color;
    }

    public void SetFontSize(float sizeIncreaseFactor)
    {
        if (sizeIncreaseFactor == 0f)
            sizeIncreaseFactor = 1f;

        sizeIncreaseFactor += currentFontIncrease;
        textBox.fontSize = sizeIncreaseFactor * _originalFontSize;
        //textBoxName.fontSize = (sizeIncreaseFactor * _originalNameFontSize) + textNameFontSizeOffset;
    }
}
