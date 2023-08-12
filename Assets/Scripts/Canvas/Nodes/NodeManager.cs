using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NodeManager : MonoBehaviour
{
    [Tooltip("The title.")]
    public TMP_Text titleTextBox;
    [Tooltip("The description.")]
    public TMP_Text descTextBox;
    private bool _descTextBoxBusy = false;
    private (string, Coroutine) _descTextBoxWriteCoroutine;
    [Tooltip("The data SO of this node.")]
    public NodeData nodeData;
    [Tooltip("The image blocking input after transition to next node is triggered.")]
    public Image transitionBlockImage;
    [Tooltip("Autotravel button.")]
    public Button autotravelButton;
    private TMP_Text _autotravelButtonLabel;
    [Tooltip("Autotravel next direction.")]
    public int autotravelNextDirection = -1;
    [Tooltip("Travel button list (indexes 0-4 == N-E-S-W).")]
    public Button[] compassButtons = new Button[4];

    private SettingsController _settingsController;
    private JournalManager _journalManager;
    private InventoryManager _inventoryManager;
    private SceneTransition _sceneTransition;
    private AsyncOperation asyncLoadNextScene;

    private float _originalTitleFontSize;
    private float _originalDescFontSize;

    public void Awake()
    {
        _originalDescFontSize = descTextBox.fontSize;
        _originalTitleFontSize = titleTextBox.fontSize;
    }

    public void Start()
    {
        _settingsController = PlayerSingleton.Instance.settingsController;
        _sceneTransition = PlayerSingleton.Instance.sceneTransition;
        _journalManager = PlayerSingleton.Instance.journalManager;
        _inventoryManager = PlayerSingleton.Instance.inventoryManager;

        _autotravelButtonLabel = autotravelButton.gameObject.GetComponentInChildren<TMP_Text>();

        SetFontSize(_settingsController.settings.fontSize);
        titleTextBox.text = nodeData.node.title;
        WriteToDesc(nodeData.node.text);
        SetButtonsActive();

        LoadCrossNodeData();
    }

    public void Explore()
    {
        if (GetQuest())
            return;
        
        SpawnRandomItem();
    }

    public bool GetQuest()
    {
        foreach (string questNameTracked in _journalManager.questStates.Keys)
            if (CheckQuestTrackedAndUpdate(questNameTracked))
                return true;

        string questName = "";
        foreach (string questNameNew in nodeData.questStates.Keys)
        {
            if (_journalManager.questStates.ContainsKey(questNameNew))
                continue;

            questName = questNameNew;
            break;
        }
        if (questName.Equals(""))
            return false;

        // quest is not tracked and on node
        AddNewQuest(questName);
        return true;
    }

    private bool CheckQuestTrackedAndUpdate(string questName)
    {
        if (!nodeData.questStates.ContainsKey(questName))
            return false;

        // this throws an exception if no manual check for the quest being present is done
        QuestStateDataType questOnNode = nodeData.questStates[questName];
        QuestStateDataType questInJournal = _journalManager.questStates[questName];
        if (questOnNode.state != questInJournal.state)
            return false;

        // quest is tracked and on node

        // if this is the last scene of the quest
        if (questOnNode.state == questOnNode.quest.questDescs.Count - 1)
            _journalManager.questStates.Remove(questName);

        Debug.Log($"Got quest from journal: {questName} playing scene {questInJournal.state}");
        FadeAndLoadScene(questInJournal.quest.sceneNames[questInJournal.state++]);

        return true;
    }

    private void AddNewQuest(string questName)
    {
        // no randomness needed, just get the first one from the list that is not tracked
        QuestStateDataType quest = nodeData.questStates[questName];
        _journalManager.TrackQuest(questName, quest);
        FadeAndLoadScene(quest.quest.sceneNames[quest.state++]);
        Debug.Log($"Got new quest: {questName} playing first scene");
    }

    public void SpawnRandomItem()
    {
        ItemSpawnerDataType itemSpawner = nodeData.itemSpawners[Random.Range(0, nodeData.itemSpawners.Count - 1)];
        // spawner is empty, tough luck
        if (itemSpawner.spawnerCapacity == 0)
            return;

        ItemData item = itemSpawner.item;
        int quantityUpperRange = itemSpawner.spawnerCapacity == -1 ? itemSpawner.quantityRangeMax : itemSpawner.spawnerCapacity;
        int quantity = Random.Range(1, Mathf.Min(itemSpawner.quantityRangeMax, quantityUpperRange));

        if (itemSpawner.spawnerCapacity != -1)
            itemSpawner.spawnerCapacity -= quantity;

        AddItemToInventory(item, quantity);
    }

    private void AddItemToInventory(ItemData item, int quantity)
    {
        _inventoryManager.AddToInventory(item, quantity);

        AddToDesc($"Found <color=#{(int)item.rarity:X6}>{item.itemName} [{item.rarity}]</color> x {quantity}");
    }

    public void WriteToDesc(string text)
    {
        if (_descTextBoxBusy)
        {
            StopCoroutine(_descTextBoxWriteCoroutine.Item2);
            SetTextbox(_descTextBoxWriteCoroutine.Item1);
        }

        _descTextBoxWriteCoroutine = (text, StartCoroutine(WriteString(text)));
    }

    public void AddToDesc(string text)
    {
        if (_descTextBoxBusy)
        {
            StopCoroutine(_descTextBoxWriteCoroutine.Item2);
            SetTextbox(_descTextBoxWriteCoroutine.Item1);
        }

        _descTextBoxWriteCoroutine = ($"{descTextBox.text}\n\n{text}", StartCoroutine(WriteString(text, true)));
    }

    private IEnumerator WriteString(string text, bool add = false)
    {
        _descTextBoxBusy = true;

        string originalText = descTextBox.text;
        for (int i = 0; i < text.Length + 1; i++)
        {
            // rich text skip
            if (text[i >= text.Length ? text.Length - 1 : i] == '<')
                i = text[i..].IndexOf('>') + i + 1;

            if (add)
                AddToTextbox(text, i, originalText);
            else
                SetTextbox(text, i);

            yield return new WaitForSeconds(nodeData.node.speed);
        }

        _descTextBoxBusy = false;
    }

    private void SetTextbox(string text, int i = -1)
    {
        if (i < 0)
            descTextBox.SetText(text);
        else
            descTextBox.SetText(text[..i]);
    }

    private void AddToTextbox(string text, int i, string originalText)
    {
        if (i < 0)
            descTextBox.SetText($"{originalText}\n\n{text}");
        else
            descTextBox.SetText($"{originalText}\n\n{text[..i]}");
    }

    public void SetFontSize(float sizeIncreaseFactor)
    {
        descTextBox.fontSize = sizeIncreaseFactor * _originalDescFontSize;
        //titleTextBox.fontSize = sizeIncreaseFactor * _originalTitleFontSize;
    }

    public void AutotravelToNextNode()
    {
        if (autotravelNextDirection != -1)
            return;

        TravelToNextNode(autotravelNextDirection);
    }

    public void TravelToNextNode(int direction)
    {
        if (direction >= nodeData.node.nextNodeSceneNames.Count)
            return;

        string sceneName = nodeData.node.nextNodeSceneNames[direction];
        if (sceneName != null && !sceneName.Equals(""))
            FadeAndLoadScene(sceneName);
    }


    private void FadeAndLoadScene(string nextSceneName)
    {
        transitionBlockImage.enabled = true;
        _sceneTransition.FadeOutScene();

        SaveCrossNodeData();

        StartCoroutine(LoadNextScene(nextSceneName));
    }
    private IEnumerator LoadNextScene(string nextSceneName)
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

    private void SaveCrossNodeData()
    {
        JournalCrossSceneDataType saveDataJournal = _journalManager.PrepareJournalEntriesForSave();
        DataSaver.SaveData(saveDataJournal, "journal");

        ItemCrossSceneDataType saveDataInventory = _inventoryManager.PrepareItemEntriesForSave();
        DataSaver.SaveData(saveDataInventory, "inventory");
    }

    private void LoadCrossNodeData()
    {
        _journalManager.questStates.Clear();

        JournalCrossSceneDataType loadedQuestData = DataSaver.LoadData<JournalCrossSceneDataType>("journal");
        foreach (QuestReferenceDataType quest in loadedQuestData.questStates)
        {
            QuestData questData = _journalManager.LookupQuest(quest.guid);
            QuestStateDataType questStateData = new(questData, quest.state);
            _journalManager.questStates.Add(questData.questName, questStateData);
        }


        _inventoryManager.heldItems.Clear();

        ItemCrossSceneDataType loadedItemsData = DataSaver.LoadData<ItemCrossSceneDataType>("inventory");
        foreach (ItemReferenceDataType item in loadedItemsData.items)
        {
            ItemData itemData = _inventoryManager.LookupItem(item.guid);
            ItemQuantityTuple itemTuple = new(itemData, item.quantity);
            _inventoryManager.heldItems.Add(itemTuple);
        }
    }

    private void SetButtonsActive()
    {
        SetAutoTravelButton();
        SetCompassButtons();
    }

    private void SetAutoTravelButton()
    {
        if (autotravelNextDirection == -1)
            return;

        SetButtonInteractable(autotravelButton, _autotravelButtonLabel);
    }

    private void SetCompassButtons()
    {
        if (nodeData.node.nextNodeSceneNames == null
            || nodeData.node.nextNodeSceneNames.Count == 0)
            return;

        int i = 0;
        foreach (string nodeName in nodeData.node.nextNodeSceneNames)
        {
            if (nodeName != "")
                SetButtonInteractable(compassButtons[i], compassButtons[i].gameObject.GetComponentInChildren<TMP_Text>());

            i++;
        }
    }

    private void SetButtonInteractable(Button button, TMP_Text buttonLabel)
    {
        button.interactable = true;
        // faded out color of label is alpha 65
        buttonLabel.color = Color.white;
    }
}
