using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneDirector : MonoBehaviour
{
    public readonly static string SCENE_LOADED_FLAG = "SCENE_LOADED_FLAG";

    //private PlayerInput _playerInput;
    private ProgressController _progressController;
    private SceneTransition _sceneTransition;
    private DialogueController _dialogueController;
    private ChoiceController _choiceController;
    private BackgroundController _backgroundController;
    private EntityController _entityController;
    private SaveController _saveController;
    private AudioController _audioController;
    private ParticleSystemController _particleSystemController;
    private FlagManager _flagManager;

    private bool _sceneTransitionFinished = false;
    private bool _lastStringIsFinished = false;
    private bool _bgFadeOutFinished = false;

    private Coroutine _autoProgressRoutine;

    [Tooltip("The chapter popup container")]
    public GameObject chapterPopupContainer;

    [Tooltip("The localisation data of this scene.")]
    public LocalisationData localization;
    private LocalisationData _choiceNestedLoc;
    [Tooltip("The background data of this scene.")]
    public BackgroundData backgrounds;
    [Tooltip("The entity data of this scene.")]
    public EntityHistoryData entityHistory;
    [Tooltip("The bg song data of this scene.")]
    public BgSongData bgSongs;
    [Tooltip("The audio effect data of this scene.")]
    public AudioEffectData audioEffects;
    [Tooltip("The particle system data of this scene.")]
    public ParticleSystemData particleSystems;

    [Tooltip("The next name of the scene to be loaded after this one.")]
    public string nextSceneName;
    public bool nextSceneDecidedByFlagManager = false;
    private AsyncOperation asyncLoadNextScene;

    [Tooltip("Set this to true if you don't want the scene director " +
        "triggering effects on its own when the scene starts. Useful for splash screen, credits etc.")]
    public bool idle = false;

    [Tooltip("The time between progress calls on auto-progress")]
    public float autoprogressTimeoutSec = 0.05f;

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
        _choiceController = PlayerSingleton.Instance.choiceController;
        _choiceController.OnChoiceMade.AddListener(ChoiceMadeInject);
        _backgroundController = PlayerSingleton.Instance.backgroundController;
        _entityController = PlayerSingleton.Instance.entityController;
        _saveController = PlayerSingleton.Instance.saveController;
        _audioController = PlayerSingleton.Instance.audioController;
        _particleSystemController = PlayerSingleton.Instance.particleSystemController;
        _flagManager = PlayerSingleton.Instance.flagManager;

        PreloadSceneAudioClips();

        if (_flagManager.ContainsFlag(SCENE_LOADED_FLAG))
            ReadSaveStates();
        else
            ResetStates();

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

        // debug: reset all disabled particle systems
        _particleSystemController.ResetSO(particleSystems.particleHistory);
        // in case particle system should be immediately visible (i.e. index 0)
        ProgressCycleParticleEffects();

        // 2.2
        _sceneTransition.Setup();
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
        // loading CrossSceneData is handled by NodeManager

        FirstExecutionRemoveChecks();
        if (_flagManager.ContainsFlag(SCENE_LOADED_FLAG))
        {
            GetStatesUpToDate();
            _flagManager.DeleteFlag(SCENE_LOADED_FLAG);
        }
        else
            Progress();

        _progressController.OnProgress.AddListener(Progress);
        _progressController.OnAutoProgress.AddListener(AutoProgress);
        _dialogueController.OnFinishedString.AddListener(LastStringIsWritten);
    }

    public void EndScene()
    {
        OnSceneEnd.Invoke();
        _sceneTransitionFinished = false;

        _particleSystemController.ResetAll();

        if (!idle)
        {
            // this assumes NodeManager.vn is set to true, else CrossSceneDataSaver.SaveCrossNodeData() will run twice
            CrossSceneDataSaver.SaveCrossNodeData();

            _progressController.OnProgress.RemoveListener(Progress);
            _progressController.OnAutoProgress.RemoveListener(AutoProgress);
            _dialogueController.OnFinishedString.RemoveListener(LastStringIsWritten);

            _dialogueController.HideDialogueContainer();

            _sceneTransition.FadeOutScene();
        }

        string nextScene = nextSceneName;
        if (nextSceneDecidedByFlagManager)
            nextScene = _flagManager.GetNextSceneByFlag(nextScene);
        if (!string.IsNullOrEmpty(nextScene))
            StartCoroutine(LoadNextScene(nextScene));
    }

    private IEnumerator LoadNextScene(string sceneName)
    {
        Debug.Log("Loading next scene");

        asyncLoadNextScene = SceneManager.LoadSceneAsync(sceneName);
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

    /*public void BypassProgressCycles()
    {
        // loc doesn't need bypassing, as loc.state doesn't depend on other states or indeces
        ProgressCycleLoc();

        if (backgrounds.textures.Count > 0)
            SendBackgroundToCanvas();
        if (entityHistory.history.Count > 0)
            SendEntitiesToCanvas();
        if (bgSongs.songs.Count > 0)
            SendBgSongToAudio();
        if (audioEffects.effects.Length > 0)
            SendEffectToAudio();
        if (particleSystems.particleHistory.Count > 0)
            SendParticleSystemsToCanvas();

        localization.state++;
    }*/

    public void AutoProgress(bool inputStarted)
    {
        if (inputStarted && _autoProgressRoutine == null)
            _autoProgressRoutine = StartCoroutine(AutoProgressRoutine(false));
        else if (_autoProgressRoutine != null)
        {
            StopCoroutine(_autoProgressRoutine);
            _autoProgressRoutine = null;
        }
    }

    private IEnumerator AutoProgressRoutine(bool injected)
    {
        while (true)
        {
            if (injected)
                ProgressInjected();
            else
                Progress();

            yield return new WaitForSeconds(autoprogressTimeoutSec);
        }
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

        // I hate this
        ProgressCycleLoc();
        ProgressCycleBg();
        ProgressCycleEntities();
        ProgressCycleBgSongs();
        ProgressCycleAudioEffects();
        ProgressCycleParticleEffects();

        // as scene is ended by loc, increase its state at the very end
        localization.state++;
    }

    public void AutoProgressInjected(bool inputStarted)
    {
        if (inputStarted && _autoProgressRoutine == null)
            _autoProgressRoutine = StartCoroutine(AutoProgressRoutine(true));
        else if (_autoProgressRoutine != null)
        {
            StopCoroutine(_autoProgressRoutine);
            _autoProgressRoutine = null;
        }
    }

    public void ChoiceMadeInject(ChoiceDataType choice)
    {
        if (choice.hasNestedLoc)
        {
            _choiceNestedLoc = choice.nestedLoc;
            _choiceNestedLoc.state = 0;
            _progressController.OnProgress.RemoveListener(Progress);
            _progressController.OnAutoProgress.RemoveListener(AutoProgress);
            _progressController.OnProgress.AddListener(ProgressInjected);
            _progressController.OnAutoProgress.AddListener(AutoProgressInjected);

            ProgressInjected();
        }
        else
            Progress();
    }

    public void ProgressInjected()
    {
        // skip scrolling effect by clicking again
        if (!_lastStringIsFinished)
        {
            _dialogueController.StopWriting();
            _choiceNestedLoc.state = _choiceNestedLoc.state - 1 >= 0 ? _choiceNestedLoc.state - 1 : 0;
            SendStringToDialogueInjected(true);
            _choiceNestedLoc.state++;
            _lastStringIsFinished = true;
            return;
        }

        if (_choiceNestedLoc.state >= _choiceNestedLoc.dialogue.Count)
        {
            // return to normal progress
            _choiceNestedLoc = null;
            _progressController.OnProgress.RemoveListener(ProgressInjected);
            _progressController.OnAutoProgress.RemoveListener(AutoProgressInjected);
            _progressController.OnProgress.AddListener(Progress);
            _progressController.OnAutoProgress.AddListener(AutoProgress);
            Progress();
            return;
        }

        SendStringToDialogueInjected(false);

        _choiceNestedLoc.state++;
    }

    private void ProgressCycleLoc()
    {
        if (localization.state >= localization.dialogue.Count) {
            // Scene is ended when there is no more dialogue
            EndScene();
            return;
        }

        LocalisationDataType curLocData = localization.dialogue[localization.state];
        if (curLocData.hasChoices)
            _choiceController.ShowChoice(curLocData.choices);
        if (curLocData.hasChapterPopup)
        {
            // text
            chapterPopupContainer.GetComponent<TMP_Text>().text = curLocData.chapterPopup.text;
            // pos
            Vector3 pos = chapterPopupContainer.transform.localPosition;
            pos.y += curLocData.chapterPopup.ChapterPopupPosEnumToYOffset();
            chapterPopupContainer.transform.SetLocalPositionAndRotation(pos, chapterPopupContainer.transform.localRotation);
            // audio clip
            _audioController.PlayNewEffect(curLocData.chapterPopup.popupClip);
            // animation
            chapterPopupContainer.GetComponent<Animation>().Play();
        }

        SendStringToDialogue(false);

        ScriptedEffect.InvokeEffects(curLocData.scriptedEffects);
    }

    private void ProgressCycleEntities()
    {
        if (entityHistory.state >= entityHistory.indexes.Count)
            return;

        // get index at which the next entities should be displayed at
        int entitiesAtDialogueIndex = entityHistory.indexes[entityHistory.state];
        if (entitiesAtDialogueIndex != localization.state)
            return;
        
        SendEntitiesToCanvas();
    }

    private void ProgressCycleBg()
    {
        if (backgrounds.state >= backgrounds.indexes.Count)
            return;

        // do not fade if we are loading a save
        if (_saveController.loading)
            return;

        // get index at which the next background should be displayed at
        // backgrounds.state is bg index to be displayed next
        int bgAtDialogueIndex = backgrounds.indexes[backgrounds.state];
        if (bgAtDialogueIndex != localization.state)
            return;

        // check if we have to fade transition the current bg before setting the new one
        // fading in bg is handled in BackgroundController.RegisterFadeOutCallbackAndFade
        BackgroundDataType curBgData = backgrounds.textures[backgrounds.state - 1 >= 0 ? backgrounds.state - 1 : 0];
        if (curBgData.fadeBlackTransition)
        {
            _bgFadeOutFinished = false;
            _backgroundController.RegisterFadeOutCallbackAndFade(SendBackgroundToCanvas);
        }
        else if (curBgData.fadeAlphaTransition)
        {
            _backgroundController.TransitionBackground(backgrounds.textures[backgrounds.state].texture);
            backgrounds.state++;
        }
        else
            SendBackgroundToCanvas();
    }

    private void ProgressCycleBgSongs()
    {
        if (bgSongs.state >= bgSongs.indexes.Count)
            return;

        int bgsAtDialogueIndex = bgSongs.indexes[bgSongs.state];
        if (bgsAtDialogueIndex != localization.state)
            return;

        SendBgSongToAudio();
    }

    private void ProgressCycleAudioEffects()
    {
        if (audioEffects.state >= audioEffects.indexes.Count)
            return;

        int effectAtDialogueIndex = audioEffects.indexes[audioEffects.state];
        if (effectAtDialogueIndex != localization.state)
            return;

        SendEffectToAudio();
    }

    private void ProgressCycleParticleEffects()
    {
        if (particleSystems.state >= particleSystems.indexes.Count)
            return;

        int particlesAtDialogueIndex = particleSystems.indexes[particleSystems.state];
        if (particlesAtDialogueIndex != localization.state)
            return;

        SendParticleSystemsToCanvas();
    }

    private void SendStringToDialogue(bool instant, int index = -1)
    {
        if (index == -1)
            index = localization.state;

        _lastStringIsFinished = false;
        int speedIndex = index < localization.dialogueSpeed.Count ? index : 0;
        LocalisationDataType l = localization.dialogue[index];
        bool isConditional = l.isConditional && 
            (l.conditionalText.flagValue == -1
            ? _flagManager.ContainsFlag(l.conditionalText.flagId)
            : _flagManager.ContainsFlag(l.conditionalText.flagId, l.conditionalText.flagValue));

        _dialogueController.ShowString(
            isConditional ? l.conditionalText.text : l.text,
            l.name,
            l.namePos,
            instant ? 0f : localization.dialogueSpeed[speedIndex],
            l.isNarrator,
            l.sizeIncrease
        );
    }

    private void SendStringToDialogueInjected(bool instant, int index = -1)
    {
        if (index == -1)
            index = _choiceNestedLoc.state;

        _lastStringIsFinished = false;
        int speedIndex = index < _choiceNestedLoc.dialogueSpeed.Count ? index : 0;
        LocalisationDataType l = _choiceNestedLoc.dialogue[index];
        bool isConditional = l.isConditional &&
            (l.conditionalText.flagValue == -1
            ? _flagManager.ContainsFlag(l.conditionalText.flagId)
            : _flagManager.ContainsFlag(l.conditionalText.flagId, l.conditionalText.flagValue));

        _dialogueController.ShowString(
            isConditional ? l.conditionalText.text : l.text,
            l.name,
            l.namePos,
            instant ? 0f : _choiceNestedLoc.dialogueSpeed[speedIndex],
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
        SendBackgroundToCanvas(-1);
    }
    private void SendBackgroundToCanvas(int index = -1)
    {
        if (index == -1)
            index = backgrounds.state;

        _backgroundController.ShowBackground(backgrounds.textures[index].texture);
        BackgroundFadeOutIsDone();
        backgrounds.state++;
    }

    private void SendEntitiesToCanvas(int index = -1)
    {
        if (index == -1)
            index = entityHistory.state;

        _entityController.RenderEntitySlide(entityHistory.history[index].entities);
        entityHistory.state++;
    }

    private void SendBgSongToAudio(int index = -1)
    {
        if (index == -1)
            index = bgSongs.state;

        _audioController.SwitchBgSong(bgSongs.songs[index]);
        bgSongs.state++;
    }

    
    private void SendEffectToAudio(int index = -1)
    {
        if (index == -1)
            index = audioEffects.state;

        AudioEffectDataTypeCollection effects = audioEffects.effects[index];
        int PlayEffectsCallback(AudioEffectDataTypeCollection effects)
        {
            _audioController.PlayEffect(effects);
            return 0;
        }

        if (effects.stopAllLoopingEffects)
            _audioController.StopAllLoopingEffectsWithFade(PlayEffectsCallback, effects);
        else if (effects.effects.Length > 0)
            _audioController.PlayEffect(effects);
        audioEffects.state++;
    }

    private void SendParticleSystemsToCanvas(int index = -1)
    {
        if (index == -1)
            index = particleSystems.state;

        _particleSystemController.Spawn(particleSystems.particleHistory[index]);
        particleSystems.state++;
    }

    private void PreloadSceneAudioClips()
    {
        // thank you unity, very cool
        foreach (AudioEffectDataTypeCollection col in audioEffects.effects)
            foreach (AudioEffectDataType aedt in col.effects)
                if (aedt.clip != null)
                    _audioController.PreloadClip(aedt.clip);
        foreach (LocalisationDataType loc in localization.dialogue)
            if (loc.hasChapterPopup && loc.chapterPopup.popupClip != null)
                _audioController.PreloadClip(loc.chapterPopup.popupClip.clip);
    }

    public static (int spawnedAtIndex, int indexPos) FindIndex(List<int> indexes, int slideIndex)
    {
        /* Context: things like entities and audio effects are spawned at specific slide indexes
         * but are not required to align with them. This results in two parallel arrays for (e.g.)
         * entities: the entity slides themselves and the list of indexes these slides are supposed to
         * be applied at. FindIndex()'s job is to find the loc index at which entities will be spawned
         * at (spawnedAtIndex) and the index of the array element itself (indexPos) */

        // this is probably the reason why the Editor displays the first entry on index 0, even if
        // it will not be spawned on loc index 0
        int result = indexes[0];
        int resultIndex = 0;

        for (int i = 0; i < indexes.Count; i++)
        {
            if (indexes[i] <= slideIndex)
            {
                result = indexes[i];
                resultIndex = i;
            }
            else
                break;
        }

        return (result, resultIndex);
    }

    public void ResetStates()
    {
        Debug.Log("Resetting states");

        localization.state = 0;
        backgrounds.state = 0;
        entityHistory.state = 0;
        bgSongs.state = 0;
        audioEffects.state = 0;
        particleSystems.state = 0;
    }

    private void ReadSaveStates()
    {
        Debug.Log("Reading save states");

        SaveStatesDataType saveStates = DataSaver.LoadData<SaveStatesDataType>("saveStates");
        localization.state = saveStates.dialogueState;
        backgrounds.state = saveStates.backgroundState;
        entityHistory.state = saveStates.entityState;
        bgSongs.state = saveStates.bgSongsState;
        audioEffects.state = saveStates.audioEffectsState;
        particleSystems.state = saveStates.particleSystemsState;
    }

    private void GetStatesUpToDate()
    {
        ProgressCycleLoc();

        if (backgrounds.textures.Count > 0)
            SendBackgroundToCanvas(FindIndex(backgrounds.indexes, localization.state).indexPos);
        if (entityHistory.history.Count > 0)
            SendEntitiesToCanvas(FindIndex(entityHistory.indexes, localization.state).indexPos);
        if (bgSongs.songs.Count > 0)
            SendBgSongToAudio(FindIndex(bgSongs.indexes, localization.state).indexPos);
        if (audioEffects.effects.Length > 0)
            SendEffectToAudio(FindIndex(audioEffects.indexes, localization.state).indexPos);
        if (particleSystems.particleHistory.Count > 0)
            SendParticleSystemsToCanvas(FindIndex(particleSystems.indexes, localization.state).indexPos);

        localization.state++;
    }
}
