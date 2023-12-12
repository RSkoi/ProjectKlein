using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveController : ControllerWithWindow
{
    //private readonly (int WIDTH, int HEIGHT) THUMB_SIZE = (800, 600);
    private readonly string SAVE_FORMAT = "yyyy-MM-dd_HH-mm-ss-f";
    private readonly string SAVE_LOCATION = "SaveData";
    private readonly string SAVE_THUMB_LOCATION = "SaveThumb";
    private readonly string SAVE_FILE_FORMAT = ".json";

    private SceneDirector _sceneDirector;
    private DialogueController _dialogueController;
    private InventoryManager _inventoryManager;
    private JournalManager _journalManager;
    private FlagManager _flagManager;
    private DNCycleController _dnCycleController;
    private ConfirmationController _confirmationController;
    private MapManager _mapManager;

    [Tooltip("Quicksave slot.")]
    public SaveFileData quicksave;
    [Tooltip("Quicksave popup text component")]
    public TMP_Text quicksaveText;
    [Tooltip("Quicksave popup animation")]
    public Animation quicksaveAnim;

    [Tooltip("Whether the game is currently loading a save.")]
    public bool loading = false;
    private AsyncOperation _asyncLoadingScene;

    private readonly Dictionary<string, Texture2D> _thumbPool = new();
    private int _curPage = 0;
    private SaveFileDataType _dataToLoad;
    private SaveFileDataType _dataToDelete;

    [Tooltip("The label of the save menu container.")]
    public TMP_Text saveMenuLabel;
    [Tooltip("The input field for the custom save name.")]
    public TMP_InputField saveNameInputField;
    [Tooltip("The container for the save entries.")]
    public GameObject saveEntryContainer;
    [Tooltip("The template for a save entry in the save menu.")]
    public GameObject saveTemplate;

    public void Start()
    {
        _sceneDirector = PlayerSingleton.Instance.sceneDirectorComponent;
        _dialogueController = PlayerSingleton.Instance.dialogueController;
        _inventoryManager = PlayerSingleton.Instance.inventoryManager;
        _journalManager = PlayerSingleton.Instance.journalManager;
        _flagManager = PlayerSingleton.Instance.flagManager;
        _mapManager = PlayerSingleton.Instance.mapManager;
        _dnCycleController = PlayerSingleton.Instance.dnCycleController;
        _confirmationController = PlayerSingleton.Instance.confirmationController;
    }

    public void Save()
    {
        SaveFileDataType save = PrepareSave();
        DataSaver.SaveData(save, save.timestamp, $"{SAVE_LOCATION}/{_curPage}", SAVE_FILE_FORMAT);

        // disabling the UI so we don't take a screenshot of the save menu lol
        window.SetActive(false);
        StartCoroutine(SaveThumb(save.timestamp));
    }

    private IEnumerator SaveThumb(string timestamp)
    {
        yield return new WaitForEndOfFrame();

        // check path to save thumb at
        string tempPath = Path.Combine(Application.persistentDataPath, $"{SAVE_THUMB_LOCATION}/{_curPage}");
        tempPath = Path.Combine(tempPath, $"{timestamp}.jpg");
        if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
            Directory.CreateDirectory(Path.GetDirectoryName(tempPath));

        // construct thumb bytes
        Texture2D thumb = ScreenCapture.CaptureScreenshotAsTexture();
        // resizing messes up the colors a bit, but who cares
        ResizeTool.Resize(thumb, (int)(thumb.width * 0.2), (int)(thumb.height * 0.2), false);
        byte[] thumbBytes = thumb.EncodeToJPG();

        // save thumb
        try
        {
            File.WriteAllBytes(tempPath, thumbBytes);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Save Data to: " + tempPath.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }

        window.SetActive(true);

        Repopulate();
    }

    public void LoadSave(SaveFileDataType data)
    {
        _dataToLoad = data;
        _confirmationController.Confirm(LoadDelegate, DataClearDelegate);
    }

    public void LoadDelegate()
    {
        loading = true;

        SaveFileDataType data = _dataToLoad;

        if (!string.IsNullOrEmpty(data.sceneName))
            StartCoroutine(LoadNextScene(data.sceneName));
        else
            _asyncLoadingScene = null;

        if (_dialogueController != null)
            _dialogueController.StopWriting();

        // TODO: implement backwards compatibility
        if (!data.gameVersion.Equals(Application.version))
            Debug.Log("Game version mismatch on loading save");

        /*_sceneDirector.localization.state = data.dialogueState;
        _sceneDirector.backgrounds.state = data.backgroundState;
        _sceneDirector.entityHistory.state = data.entityState;
        _sceneDirector.audioEffects.state = data.audioEffectsState;
        _sceneDirector.bgSongs.state = data.bgSongsState;
        _sceneDirector.particleSystems.state = data.particleSystemsState;*/

        // journal and item entries are not directly saved to an SO and are node-specific
        // meaning the values loaded here will be overwritten by the default values of the scene/node
        //_inventoryManager.SetItems(quicksave.save.itemsData);
        //_journalManager.SetJournal(quicksave.save.journalData);

        // => save loaded data to CrossSceneData folder; this means a redundant save/read to/from files
        DataSaver.SaveData(data.saveStates, "saveStates");
        DataSaver.SaveData(data.itemsData, "inventory");
        DataSaver.SaveData(data.journalData, "journal");
        DataSaver.SaveData(data.visitedNodes, "visitedNodes");
        _flagManager.SetFlags(data.flagData);
        _dnCycleController.SetCycle(data.dnCycleData);

        _sceneDirector.FirstExecutionRemoveChecks();
        _flagManager.AddFlag(SceneDirector.SCENE_LOADED_FLAG);

        loading = false;

        if (_asyncLoadingScene != null)
            _asyncLoadingScene.allowSceneActivation = true;
    }

    public void DeleteSave(SaveFileDataType data)
    {
        _dataToDelete = data;
        _confirmationController.Confirm(DeleteDelegate, DataClearDelegate);
    }

    private void DeleteDelegate()
    {
        SaveFileDataType data = _dataToDelete;

        // delete data json
        DataSaver.DeleteData(data.timestamp, $"{SAVE_LOCATION}/{_curPage}", SAVE_FILE_FORMAT);
        // delete thumb
        DataSaver.DeleteData(data.timestamp, $"{SAVE_THUMB_LOCATION}/{_curPage}", ".jpg");

        DataClearDelegate();
        Repopulate();
    }

    private void DataClearDelegate()
    {
        _dataToLoad = null;
        _dataToDelete = null;
    }

    private SaveFileDataType PrepareSave()
    {
        string timestamp = DateTime.Now.ToString(SAVE_FORMAT);
        SaveFileDataType save = new()
        {
            gameVersion = Application.version,
            saveName = FormatSaveName(timestamp),
            timestamp = timestamp,
            sceneName = SceneManager.GetActiveScene().name,
            saveStates = PrepareSaveStates(),
            itemsData = _inventoryManager.PrepareItemEntriesForSave(),
            journalData = _journalManager.PrepareJournalEntriesForSave(),
            flagData = _flagManager.PrepareFlagEntriesForSave(),
            visitedNodes = _mapManager.PrepareVisitedNodeEntriesForSave(),
            dnCycleData = _dnCycleController.PrepareCycleForSave(),
        };

        return save;
    }

    private SaveStatesDataType PrepareSaveStates()
    {
        return new(
            _sceneDirector.localization.state - 1 >= 0 ? _sceneDirector.localization.state - 1 : 0,
            _sceneDirector.backgrounds.state - 1 >= 0 ? _sceneDirector.backgrounds.state - 1 : 0,
            _sceneDirector.entityHistory.state - 1 >= 0 ? _sceneDirector.entityHistory.state - 1 : 0,
            _sceneDirector.audioEffects.state - 1 >= 0 ? _sceneDirector.audioEffects.state - 1 : 0,
            _sceneDirector.bgSongs.state - 1 >= 0 ? _sceneDirector.bgSongs.state - 1 : 0,
            _sceneDirector.particleSystems.state - 1 >= 0 ? _sceneDirector.particleSystems.state - 1 : 0);
    }

    public void SwitchSaveMenuPage(int page)
    {
        // pages are zero indexed
        _curPage = page;
        saveMenuLabel.SetText(Regex.Replace(saveMenuLabel.text, "Page [0-9]+", $"Page {page + 1}"));

        Repopulate();
    }

    private bool ShowEntry(string fileDir, int poolIndex)
    {
        string fileName = Path.GetFileNameWithoutExtension(fileDir);

        SaveFileDataType saveData;
        try {
            saveData = DataSaver.LoadData<SaveFileDataType>(fileName, $"{SAVE_LOCATION}/{_curPage}", SAVE_FILE_FORMAT);
        }
        catch (ArgumentException)
        {
            Debug.LogError($"Save file {fileName} corrupted. Could not parse.");
            return false;
        }

        // get thumb
        Texture2D thumb;
        if (_thumbPool.ContainsKey(saveData.timestamp))
            thumb = _thumbPool[saveData.timestamp];
        else
        {
            thumb = new(1, 1);
            string tempPath = Path.Combine(Application.persistentDataPath, $"{SAVE_THUMB_LOCATION}/{_curPage}");
            tempPath = Path.Combine(tempPath, $"{saveData.timestamp}.jpg");
            if (Directory.Exists(Path.GetDirectoryName(tempPath)))
                if (File.Exists(tempPath))
                    thumb.LoadImage(File.ReadAllBytes(tempPath));

            _thumbPool[saveData.timestamp] = thumb;
        }

        Transform saveInPool = saveEntryContainer.transform.GetChild(poolIndex);
        saveInPool.gameObject.SetActive(true);
        saveInPool.gameObject.GetComponent<SaveEntry>().Init(saveData, saveData.saveName, thumb.width != 1 ? thumb : null);

        return true;
    }

    private void PopulateMenu()
    {
        string tempPath = Path.Combine(Application.persistentDataPath, $"{SAVE_LOCATION}/{_curPage}/");
        if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
            return;

        string[] fileDirs = Directory.GetFiles(tempPath, $"*{SAVE_FILE_FORMAT}", SearchOption.TopDirectoryOnly);
        // -1 because of the "+" button
        if (fileDirs.Length > saveEntryContainer.transform.childCount - 1)
            Populate(fileDirs.Length - (saveEntryContainer.transform.childCount - 1));

        for (int i = 1; i < fileDirs.Length + 1; i++)
            if (!ShowEntry(fileDirs[i - 1], i))
                saveEntryContainer.transform.GetChild(i).gameObject.SetActive(false);
    }

    private void Populate(int count)
    {
        for (int i = 0; i < count; i++)
            Instantiate(saveTemplate, saveEntryContainer.transform, false);
    }

    private void SetEntriesInactive()
    {
        for (int i = 0; i < saveEntryContainer.transform.childCount; i++)
        {
            Transform child = saveEntryContainer.transform.GetChild(i);
            if (child.gameObject.name != "AddSaveButton")
                child.gameObject.SetActive(false);
        }
    }

    private void Repopulate()
    {
        SetEntriesInactive();
        PopulateMenu();
    }

    public void SaveQuick()
    {
        Debug.Log("Quicksaving");

        string timestamp = DateTime.Now.ToString(SAVE_FORMAT);

        quicksave.save.gameVersion = Application.version;
        quicksave.save.saveName = FormatSaveName(timestamp);
        quicksave.save.timestamp = DateTime.Now.ToString(SAVE_FORMAT);
        quicksave.save.sceneName = SceneManager.GetActiveScene().name;
        quicksave.save.saveStates = PrepareSaveStates();
        quicksave.save.itemsData = _inventoryManager.PrepareItemEntriesForSave();
        quicksave.save.journalData = _journalManager.PrepareJournalEntriesForSave();
        quicksave.save.flagData = _flagManager.PrepareFlagEntriesForSave();
        quicksave.save.visitedNodes = _mapManager.PrepareVisitedNodeEntriesForSave();
        quicksave.save.dnCycleData = _dnCycleController.PrepareCycleForSave();

        DataSaver.SaveData(quicksave.save, "quicksave");

        quicksaveText.text = "Quicksaved!";
        quicksaveAnim.Play();
    }

    public void LoadQuick()
    {
        Debug.Log("Quickloading");

        quicksaveText.text = "Quickloading...";
        quicksaveAnim.Play();

        loading = true;

        quicksave.save = DataSaver.LoadData<SaveFileDataType>("quicksave");

        if (!string.IsNullOrEmpty(quicksave.save.sceneName))
            StartCoroutine(LoadNextScene(quicksave.save.sceneName));
        else
            _asyncLoadingScene = null;

        if (_dialogueController != null)
            _dialogueController.StopWriting();

        /*_sceneDirector.localization.state = quicksave.save.dialogueState;
        _sceneDirector.backgrounds.state = quicksave.save.backgroundState;
        _sceneDirector.entityHistory.state = quicksave.save.entityState;
        _sceneDirector.audioEffects.state = quicksave.save.audioEffectsState;
        _sceneDirector.bgSongs.state = quicksave.save.bgSongsState;
        _sceneDirector.particleSystems.state = quicksave.save.particleSystemsState;*/

        // journal and item entries are not directly saved to an SO and are node-specific
        // meaning the values loaded here will be overwritten by the default values of the scene/node
        //_inventoryManager.SetItems(quicksave.save.itemsData);
        //_journalManager.SetJournal(quicksave.save.journalData);

        // => save loaded data to CrossSceneData folder; this means a redundant save/read to/from files
        DataSaver.SaveData(quicksave.save.saveStates, "saveStates");
        DataSaver.SaveData(quicksave.save.itemsData, "inventory");
        DataSaver.SaveData(quicksave.save.journalData, "journal");
        DataSaver.SaveData(quicksave.save.visitedNodes, "visitedNodes");
        _flagManager.SetFlags(quicksave.save.flagData);
        _dnCycleController.SetCycle(quicksave.save.dnCycleData);

        _sceneDirector.FirstExecutionRemoveChecks();
        _flagManager.AddFlag(SceneDirector.SCENE_LOADED_FLAG);

        loading = false;

        if (_asyncLoadingScene != null)
            _asyncLoadingScene.allowSceneActivation = true;
    }

    public void DeleteQuick()
    {
        Debug.Log("Deleting quicksave");

        DataSaver.DeleteData("quicksave");
    }

    private IEnumerator LoadNextScene(string nextSceneName)
    {
        _asyncLoadingScene = SceneManager.LoadSceneAsync(nextSceneName);
        _asyncLoadingScene.allowSceneActivation = false;

        while (!_asyncLoadingScene.isDone)
            yield return null;
    }

    private string FormatSaveName(string fallback)
    {
        string customName = saveNameInputField.text;
        customName = customName.Replace("<timestamp>", fallback);
        // escaping special chars is not needed if the name is not used as a filename, and save
        // file supports unicode
        //customName = string.Join("_", customName.Split(Path.GetInvalidFileNameChars()));
        return customName.Equals("") ? fallback : customName;
    }

    public void ToggleSaveMenu()
    {
        if (window.activeSelf)
        {
            SetEntriesInactive();
            window.SetActive(false);
        }
        else
        {
            window.SetActive(true);
            PopulateMenu();
        }
    }

    public override void ToggleWindow()
    {
        ToggleSaveMenu();
    }
}
