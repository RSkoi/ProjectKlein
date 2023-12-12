using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    private GameObject _sceneDirectorGO;
    private SceneDirector _sceneDirector;
    private SceneTransition _sceneTransition;
    private AudioController _audioController;
    private SettingsController _settingsController;
    private SaveController _saveController;
    private FlagManager _flagManager;
    private ConfirmationController _confirmationController;
    private DNCycleController _dnCycleController;

    private int _hoveringOverButton = -1;
    private AsyncOperation _asyncLoadNextScene;

    public Image transitionBlockImage;
    public string newGameSceneName = "";
    public RectTransform menuRectTransform;
    public List<RectTransform> menuButtonsTransforms = new();
    public GameObject menuButtonUnderline;
    private Animation _menuButtonUnderlineAnim;
    public TMP_Text versionNumber;

    public void Start()
    {
        _menuButtonUnderlineAnim = menuButtonUnderline.GetComponent<Animation>();

        versionNumber.text = $"v{Application.version}";
    }

    public void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        if (TMP_TextUtilities.IsIntersectingRectTransform(menuRectTransform, mousePos, Camera.current))
        {
            for (int i = 0; i < menuButtonsTransforms.Count; i++)
            {
                if (_hoveringOverButton == i)
                    continue;

                RectTransform buttonRect = menuButtonsTransforms[i];
                if (TMP_TextUtilities.IsIntersectingRectTransform(buttonRect, mousePos, Camera.current))
                {
                    _hoveringOverButton = i;
                    _menuButtonUnderlineAnim.Rewind();
                    menuButtonUnderline.transform.SetParent(buttonRect, false);
                    menuButtonUnderline.transform.SetSiblingIndex(0);
                    _menuButtonUnderlineAnim.Play();
                    break;
                }
            }
        }
        else
            _hoveringOverButton = -1;
    }

    public void SetupData(PlayerSingleton instance)
    {
        _sceneDirectorGO = GameObject.Find("SceneDirector");
        _sceneDirectorGO.TryGetComponent(out _sceneDirector);
        _sceneDirectorGO.TryGetComponent(out _sceneTransition);
        GameObject.Find("AudioDirector").TryGetComponent(out _audioController);
        GameObject.Find("SettingsManager").TryGetComponent(out _settingsController);
        GameObject.Find("SaveLoad").TryGetComponent(out _saveController);
        GameObject.Find("FlagManager").TryGetComponent(out _flagManager);
        GameObject.Find("ConfirmationController").TryGetComponent(out _confirmationController);
        GameObject.Find("Node").TryGetComponent(out _dnCycleController);

        instance.sceneDirectorComponent = _sceneDirector;
        instance.audioController = _audioController;
        instance.settingsController = _settingsController;
        instance.saveController = _saveController;
        instance.flagManager = _flagManager;
        instance.confirmationController = _confirmationController;
        instance.dnCycleController = _dnCycleController;
    }

    public void StartNewGame()
    {
        CrossSceneDataSaver.DeleteCrossNodeData();
        _saveController.DeleteQuick();
        _flagManager.ClearFlags();
        FadeAndLoadScene(newGameSceneName);
    }

    public void QuitGame()
    {
        _confirmationController.Confirm("Quit Game?", "Yes", "No", QuitGameDelegate);
    }

    private void QuitGameDelegate()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    private void FadeAndLoadScene(string nextSceneName)
    {
        transitionBlockImage.enabled = true;
        _sceneTransition.FadeOutScene();

        StartCoroutine(LoadNextScene(nextSceneName));
    }

    private IEnumerator LoadNextScene(string nextSceneName)
    {
        Debug.Log("Loading next scene");

        _asyncLoadNextScene = SceneManager.LoadSceneAsync(nextSceneName);
        _asyncLoadNextScene.allowSceneActivation = false;

        while (!_asyncLoadNextScene.isDone)
            yield return null;
    }

    public void ActivateNextScene()
    {
        if (_asyncLoadNextScene == null)
            return;

        Debug.Log("Activating next scene");

        _asyncLoadNextScene.allowSceneActivation = true;
    }
}
