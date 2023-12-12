using System;
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

    public MainMenuController mainMenuController;

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
    public HistoryController historyController;
    public ChoiceController choiceController;
    public ParticleSystemController particleSystemController;
    public ConfirmationController confirmationController;

    public GameObject manager;
    public NodeManager nodeManager;
    public MapManager mapManager;
    public DNCycleController dnCycleController;
    public JournalManager journalManager;
    public InventoryManager inventoryManager;
    public FlagManager flagManager;
    private void SetupData()
    {
        // TODO: this is really messy man
        player = GameObject.Find("Player");
        player.TryGetComponent(out playerInput);

        try
        {
            controller = GameObject.Find("MainMenu");
            controller.TryGetComponent(out mainMenuController);
            mainMenuController.SetupData(this);
            return;
        }
        catch (NullReferenceException)
        {
            Debug.Log("Not the main menu. Continuing setup.");
        }

        sceneDirector = GameObject.Find("SceneDirector");
        sceneDirector.TryGetComponent(out sceneTransition);
        sceneDirector.TryGetComponent(out sceneDirectorComponent);

        audioDirector = GameObject.Find("AudioDirector");
        audioDirector.TryGetComponent(out audioController);

        settingsManager = GameObject.Find("SettingsManager");
        settingsManager.TryGetComponent(out settingsController);

        saveManager = GameObject.Find("SaveLoad");
        saveManager.TryGetComponent(out saveController);

        controller = GameObject.Find("Controller");
        controller.TryGetComponent(out dialogueController);
        if (sceneDirectorComponent.idle)
            SetupNodeSpecific();
        else
            SetupVNSpecific(controller);

        controller = GameObject.Find("ConfirmationController");
        controller.TryGetComponent(out confirmationController);

        manager = GameObject.Find("FlagManager");
        manager.TryGetComponent(out flagManager);

        manager = GameObject.Find("Journal");
        manager.TryGetComponent(out journalManager);

        manager = GameObject.Find("Inventory");
        manager.TryGetComponent(out inventoryManager);

        manager = GameObject.Find("Node");
        manager.TryGetComponent(out nodeManager);
        manager.TryGetComponent(out mapManager);
        manager.TryGetComponent(out dnCycleController);
    }

    private void SetupVNSpecific(GameObject controller)
    {
        controller.TryGetComponent(out backgroundTransition);
        controller.TryGetComponent(out backgroundController);
        controller.TryGetComponent(out entityController);
        controller.TryGetComponent(out progressController);
        controller.TryGetComponent(out choiceController);
        controller.TryGetComponent(out particleSystemController);

        manager = GameObject.Find("VNHistory");
        manager.TryGetComponent(out historyController);
    }

    private void SetupNodeSpecific()
    {

    }
}
