using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveController : MonoBehaviour
{
    private SceneDirector _sceneDirector;
    private DialogueController _dialogueController;
    private InventoryManager _inventoryManager;
    private JournalManager _journalManager;

    [Tooltip("Quicksave slot.")]
    public SaveFileData quicksave;

    [Tooltip("Whether the game is currently loading a save.")]
    public bool loading = false;
    private AsyncOperation _asyncLoadingScene;

    public void Start()
    {
        _sceneDirector = PlayerSingleton.Instance.sceneDirectorComponent;
        _dialogueController = PlayerSingleton.Instance.dialogueController;
        _inventoryManager = PlayerSingleton.Instance.inventoryManager;
        _journalManager = PlayerSingleton.Instance.journalManager;
    }

    public void SaveQuick()
    {
        Debug.Log("Quicksaving");

        quicksave.save.sceneName = SceneManager.GetActiveScene().name;
        quicksave.save.dialogueState = _sceneDirector.localization.state - 1 >= 0
            ? _sceneDirector.localization.state - 1 : 0;
        quicksave.save.backgroundState = _sceneDirector.backgrounds.state - 1 >= 0
            ? _sceneDirector.backgrounds.state - 1 : 0;
        quicksave.save.entityState = _sceneDirector.entityHistory.state - 1 >= 0
            ? _sceneDirector.entityHistory.state - 1 : 0;
        quicksave.save.audioEffectsState = _sceneDirector.audioEffects.state - 1 >= 0
            ? _sceneDirector.audioEffects.state - 1 : 0;
        quicksave.save.bgSongsState = _sceneDirector.bgSongs.state - 1 >= 0
            ? _sceneDirector.bgSongs.state - 1 : 0;
        quicksave.save.itemsData = _inventoryManager.PrepareItemEntriesForSave();
        quicksave.save.journalData = _journalManager.PrepareJournalEntriesForSave();

        DataSaver.SaveData(quicksave.save, "quicksave");
    }

    public void LoadQuick()
    {
        Debug.Log("Quickloading");

        loading = true;

        quicksave.save = DataSaver.LoadData<SaveFileDataType>("quicksave");

        if (quicksave.save.sceneName != null && !quicksave.save.sceneName.Equals(""))
            StartCoroutine(LoadNextScene(quicksave.save.sceneName));
        else
            _asyncLoadingScene = null;

        _dialogueController.StopWriting();

        _sceneDirector.localization.state = quicksave.save.dialogueState;
        _sceneDirector.backgrounds.state = quicksave.save.backgroundState;
        _sceneDirector.entityHistory.state = quicksave.save.entityState;
        _sceneDirector.audioEffects.state = quicksave.save.audioEffectsState;
        _sceneDirector.bgSongs.state = quicksave.save.bgSongsState;

        _sceneDirector.FirstExecutionRemoveChecks();
        //_sceneDirector.Progress();

        loading = false;

        if (_asyncLoadingScene != null)
            _asyncLoadingScene.allowSceneActivation = true;
    }

    private IEnumerator LoadNextScene(string nextSceneName)
    {
        _asyncLoadingScene = SceneManager.LoadSceneAsync(nextSceneName);
        _asyncLoadingScene.allowSceneActivation = false;

        while (!_asyncLoadingScene.isDone)
            yield return null;
    }
}
