using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveController : MonoBehaviour
{
    private SceneDirector _sceneDirector;
    private DialogueController _dialogueController;

    [Tooltip("Quicksave slot.")]
    public SaveFileDataType quicksave;

    [Tooltip("Whether the game is currently loading a save.")]
    public bool loading = false;

    public void Start()
    {
        _sceneDirector = PlayerSingleton.Instance.sceneDirectorComponent;
        _dialogueController = PlayerSingleton.Instance.dialogueController;
    }

    public void SaveQuick()
    {
        Debug.Log("Quicksaving");

        quicksave.sceneName = SceneManager.GetActiveScene().name;
        quicksave.dialogueState = _sceneDirector.localization.state - 1 >= 0
            ? _sceneDirector.localization.state - 1 : 0;
        quicksave.backgroundState = _sceneDirector.backgrounds.state - 1 >= 0
            ? _sceneDirector.backgrounds.state - 1 : 0;
        quicksave.entityState = _sceneDirector.entityHistory.state - 1 >= 0
            ? _sceneDirector.entityHistory.state - 1 : 0;
    }

    public void LoadQuick()
    {
        Debug.Log("Quickloading");

        loading = true;

        if (quicksave.sceneName == null || quicksave.sceneName.Equals(""))
            return;

        if (SceneManager.GetActiveScene().name != quicksave.sceneName)
            SceneManager.LoadScene(quicksave.sceneName);

        _dialogueController.StopWriting();

        _sceneDirector.localization.state = quicksave.dialogueState;
        _sceneDirector.backgrounds.state = quicksave.backgroundState;
        _sceneDirector.entityHistory.state = quicksave.entityState;

        _sceneDirector.FirstExecutionRemoveChecks();
        _sceneDirector.Progress();

        loading = false;
    }
}
