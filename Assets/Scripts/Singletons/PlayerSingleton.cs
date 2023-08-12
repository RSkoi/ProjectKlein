using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSingleton : MonoBehaviour
{
    public static PlayerSingleton Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        SetupData();
    }

    public GameObject player;
    public PlayerInput playerInput;

    public GameObject sceneDirector;
    public SceneDirector sceneDirectorComponent;
    public SceneTransition sceneTransition;

    public GameObject audioDirector;
    public AudioController audioController;

    public GameObject settingsManager;
    public SettingsController settingsController;

    public GameObject saveManager;
    public SaveController saveController;

    public GameObject controller;
    public BackgroundTransition backgroundTransition;
    public DialogueController dialogueController;
    public ProgressController progressController;
    public BackgroundController backgroundController;
    public EntityController entityController;

    public JournalManager journalManager;
    public InventoryManager inventoryManager;
    private void SetupData()
    {
        // TODO: make these all persistent/link in editor?
        player = GameObject.Find("Player");
        player.TryGetComponent(out playerInput);

        sceneDirector = GameObject.Find("SceneDirector");
        sceneDirector.TryGetComponent(out sceneTransition);
        sceneDirector.TryGetComponent(out sceneDirectorComponent);

        audioDirector = GameObject.Find("AudioDirector");
        audioDirector.TryGetComponent(out audioController);

        settingsManager = GameObject.Find("SettingsManager");
        settingsManager.TryGetComponent(out settingsController);

        saveManager = GameObject.Find("SaveManager");
        saveManager.TryGetComponent(out saveController);

        controller = GameObject.Find("Controller");
        controller.TryGetComponent(out dialogueController);
        if (!sceneDirectorComponent.idle)
        {
            controller.TryGetComponent(out backgroundTransition);
            controller.TryGetComponent(out backgroundController);
            controller.TryGetComponent(out entityController);
            controller.TryGetComponent(out progressController);
        }

        GameObject jMG = GameObject.Find("JournalManager");
        jMG.TryGetComponent(out journalManager);

        GameObject iMG = GameObject.Find("InventoryManager");
        iMG.TryGetComponent(out inventoryManager);
    }
}
