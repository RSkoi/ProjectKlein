using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ChoiceController : MonoBehaviour
{
    private PlayerInput _playerInput;
    private FlagManager _flagManager;

    [Tooltip("Container GO for the choice UI templates.")]
    public GameObject choiceContainer;
    [Tooltip("The canvas group component attached to the choice container. Used for fading.")]
    public CanvasGroup choiceCanvasGroup;
    [Tooltip("The UI template for choices.")]
    public GameObject choiceTemplate;
    [Tooltip("How long it takes for the choices to fade in/out.")]
    public float choiceFadeDuration = 1f;
    [Tooltip("Notifies listeners when a choice has been made/clicked on.")]
    public UnityEvent<ChoiceDataType> OnChoiceMade = new();

    void Start()
    {
        _playerInput = PlayerSingleton.Instance.playerInput;
        _flagManager = PlayerSingleton.Instance.flagManager;
    }

    public void ShowChoice(ChoiceDataType choice, int poolIndex)
    {
        SwitchToBlankMap();

        Transform choiceInPool = choiceContainer.transform.GetChild(poolIndex);
        choiceInPool.gameObject.SetActive(true);
        Button button = choiceInPool.gameObject.GetComponent<Button>();
        button.onClick.AddListener(() => ChoiceEffect(choice));
        if (choice.requiresFlag
            && (choice.requiresFlagValue == -1
                ? !_flagManager.ContainsFlag(choice.requiresFlagId)
                : !_flagManager.ContainsFlag(choice.requiresFlagId, choice.requiresFlagValue)))
            button.interactable = false;
        choiceInPool.GetChild(0).GetComponent<TMP_Text>().SetText(choice.text);

        StartCoroutine(FadeChoicesGroupIn());
    }

    public void ShowChoice(ChoiceDataType[] choices)
    {
        if (choices.Length > choiceContainer.transform.childCount)
            Populate(choices.Length - choiceContainer.transform.childCount);

        for (int i = 0; i < choices.Length; i++)
            ShowChoice(choices[i], i);
    }

    public void ChoiceEffect(ChoiceDataType choice)
    {
        Debug.Log($"Chose choice {choice.text}" + (choice.setsFlag ? $" with flag {choice.flagId} {choice.flagValue}" : ""));

        OnChoiceMade.Invoke(choice);

        if (choice.setsFlag)
            _flagManager.AddFlag(choice.flagId, choice.flagValue);

        Depopulate();
        SwitchToBaseMap();
    }

    private void Populate(int count)
    {
        for (int i = 0; i < count; i++)
            Instantiate(choiceTemplate, choiceContainer.transform, false);
    }

    private void Depopulate()
    {
        // no actual destroying of GOs, as that triggers the garbage collector -> lag
        foreach (Transform child in choiceContainer.transform)
        {
            child.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
            // moved to after fade out
            //child.gameObject.SetActive(false);
        }

        StartCoroutine(FadeChoicesGroupOut());
    }

    private void SwitchToBlankMap()
    {
        _playerInput.SwitchCurrentActionMap("BlankMap");
    }

    private void SwitchToBaseMap()
    {
        _playerInput.SwitchCurrentActionMap("BaseMap");
    }

    private IEnumerator FadeChoicesGroupIn()
    {
        while (choiceCanvasGroup.alpha < 1f)
        {
            choiceCanvasGroup.alpha += Time.fixedDeltaTime / choiceFadeDuration;

            if (choiceCanvasGroup.alpha > 0.99f)
                choiceCanvasGroup.alpha = 1f;

            yield return null;
        }
    }

    private IEnumerator FadeChoicesGroupOut()
    {
        while (choiceCanvasGroup.alpha > 0f)
        {
            choiceCanvasGroup.alpha -= Time.fixedDeltaTime / choiceFadeDuration;

            if (choiceCanvasGroup.alpha < 0.01f)
                choiceCanvasGroup.alpha = 0f;

            yield return null;
        }
        
        foreach (Transform child in choiceContainer.transform)
            child.gameObject.SetActive(false);
    }
}
