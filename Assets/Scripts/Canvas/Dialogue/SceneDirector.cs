using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneDirector : MonoBehaviour
{
    //private PlayerInput _playerInput;
    private ProgressController _progressController;
    private SceneTransition _sceneTransition;
    private DialogueController _dialogueController;
    private BackgroundController _backgroundController;
    private EntityController _entityController;
    private SaveController _saveController;

    private bool _sceneTransitionFinished = false;
    private bool _lastStringIsFinished = false;
    private bool _bgFadeOutFinished = false;

    [Tooltip("The localisation data of this scene.")]
    public LocalisationData localization;
    [Tooltip("The background data of this scene.")]
    public BackgroundData backgrounds;
    [Tooltip("The entity data of this scene.")]
    public EntityHistoryData entityHistory;

    [Tooltip("The next name of the scene to be loaded after this one.")]
    public string nextSceneName;
    private AsyncOperation asyncLoadNextScene;

    [Tooltip("Set this to true if you don't want the scene director " +
        "triggering effects on its own when the scene starts. Useful for splash screen, credits etc.")]
    public bool idle = false;

    [Tooltip("Event invoked on scene start.")]
    public UnityEvent OnSceneStart = new();
    [Tooltip("Event invoked on scene end.")]
    public UnityEvent OnSceneEnd = new();

    public void Start()
    {
        if (idle)
            return;

        _sceneTransition = PlayerSingleton.Instance.sceneTransition;
        _progressController = PlayerSingleton.Instance.progressController;
        _dialogueController = PlayerSingleton.Instance.dialogueController;
        _backgroundController = PlayerSingleton.Instance.backgroundController;
        _entityController = PlayerSingleton.Instance.entityController;
        _saveController = PlayerSingleton.Instance.saveController;

        // 1. block player input
        // 2.1 set first background
        // 2.2 fade camera in
        // 3. cycle through dialogue, backgrounds, chars...
        // 4. on ending dialogue:
        // - fade camera out
        // - load new scene
        // - fade audio out

        // 1. is implicitly done by not being subscribed to _progressController.OnProgress
        //PreventiveSwitchToBlankMap();

        // 2.1
        //SendBackgroundToCanvas();
        // this will flash a white screen if we don't immediately set the bg
        _backgroundController.image.color = Color.white;
        _backgroundController.ShowBackground(backgrounds.textures[backgrounds.state].texture);

        // 2.2
        _sceneTransition.FadeInScene();

        // 3. handled with _progressController.OnProgress, subscribed in StartScene
        // StartScene is invoked by SceneTransition.OnFadeInSceneDone
        OnSceneStart.Invoke();

        // 4. fadeOutDone triggers EndScene()
    }

    /*private void PreventiveSwitchToBlankMap()
    {
        _playerInput.SwitchCurrentActionMap("BlankMap");
    }*/

    public void StartScene()
    {
        FirstExecutionRemoveChecks();
        Progress();

        _progressController.OnProgress.AddListener(Progress);
        _dialogueController.OnFinishedString.AddListener(LastStringIsWritten);
    }

    public void EndScene()
    {
        OnSceneEnd.Invoke();
        _sceneTransitionFinished = false;

        if (!idle)
        {
            _progressController.OnProgress.RemoveListener(Progress);
            _dialogueController.OnFinishedString.RemoveListener(LastStringIsWritten);

            _dialogueController.HideDialogueContainer();

            _sceneTransition.FadeOutScene();
        }

        if (nextSceneName != null && !nextSceneName.Equals(""))
        {
            StartCoroutine(LoadNextScene());
        }
    }

    private IEnumerator LoadNextScene()
    {
        Debug.Log("Loading next scene");

        asyncLoadNextScene = SceneManager.LoadSceneAsync(nextSceneName);
        asyncLoadNextScene.allowSceneActivation = false;

        while (!asyncLoadNextScene.isDone)
            yield return null;
    }

    public void ActivateNextScene()
    {
        if (asyncLoadNextScene == null)
            return;

        Debug.Log("Activating next scene");

        asyncLoadNextScene.allowSceneActivation = true;
    }

    public void Progress()
    {
        // skip scrolling effect by clicking again
        // block this feature while bg is fading in/out, or else we'll break it by increasing loc.state too fast
        if (!_lastStringIsFinished && _bgFadeOutFinished)
        {
            _dialogueController.StopWriting();
            localization.state = localization.state - 1 >= 0 ? localization.state - 1 : 0;
            SendStringToDialogue(true);
            localization.state++;
            _lastStringIsFinished = true;
            return;
        }

        if (!_sceneTransitionFinished || !_bgFadeOutFinished)
            return;

        Debug.Log($"loc.state: {localization.state}, bg.state: {backgrounds.state}");

        ProgressCycleLoc();
        ProgressCycleBg();
        ProgressCycleEntities();

        // as scene is ended by loc, increase its state at the very end
        localization.state++;
    }

    private void ProgressCycleLoc()
    {
        if (localization.state >= localization.dialogue.Count) {
            // Scene is ended when there is no more dialogue
            EndScene();
            return;
        }

        SendStringToDialogue(false);
    }

    private void ProgressCycleEntities()
    {
        if (entityHistory.state >= entityHistory.indexes.Count)
            return;

        // get index at which the next background should be displayed at
        // backgrounds.state is bg index to be displayed next
        int entitiesAtDialogueIndex = entityHistory.indexes[entityHistory.state];
        if (entitiesAtDialogueIndex != localization.state)
            return;

        SendEntitiesToCanvas();
    }

    private void ProgressCycleBg()
    {
        if (backgrounds.state >= backgrounds.indexes.Count)
            return;

        // get index at which the next background should be displayed at
        // backgrounds.state is bg index to be displayed next
        int bgAtDialogueIndex = backgrounds.indexes[backgrounds.state];
        if (bgAtDialogueIndex != localization.state)
            return;

        // check if we have to fade transition the current bg before setting the new one
        // fading in bg is handled in BackgroundController.ShowBackground
        // do not fade if we are loading a save

        if (!_saveController.loading && backgrounds.textures[backgrounds.state - 1 >= 0 ? backgrounds.state - 1 : 0].fadeTransition)
        {
            _bgFadeOutFinished = false;
            _backgroundController.RegisterFadeOutCallbackAndFade(SendBackgroundToCanvas);
        }
        else
            SendBackgroundToCanvas();
    }

    private void SendStringToDialogue(bool instant)
    {
        _lastStringIsFinished = false;
        int speedIndex = localization.state < localization.dialogueSpeed.Count ? localization.state : 0;
        LocalisationDataType l = localization.dialogue[localization.state];
        _dialogueController.ShowString(
            l.text,
            l.name,
            l.namePos,
            instant ? 0f : localization.dialogueSpeed[speedIndex],
            l.isNarrator,
            l.sizeIncrease
        );
    }

    #region checks for transitions and states
    public void FirstExecutionRemoveChecks()
    {
        LastStringIsWritten();
        SceneTransitionIsDone();
        BackgroundFadeOutIsDone();
    }

    public void LastStringIsWritten()
    {
        _lastStringIsFinished = true;
    }

    public void SceneTransitionIsDone()
    {
        _sceneTransitionFinished = true;
    }

    public void BackgroundFadeOutIsDone()
    {
        _bgFadeOutFinished = true;
    }
    #endregion

    private void SendBackgroundToCanvas()
    {
        _backgroundController.ShowBackground(backgrounds.textures[backgrounds.state].texture);
        backgrounds.state++;
        BackgroundFadeOutIsDone();
    }

    private void SendEntitiesToCanvas()
    {
        _entityController.RenderEntitySlide(entityHistory.history[entityHistory.state].entities);
        entityHistory.state++;
    }
}
