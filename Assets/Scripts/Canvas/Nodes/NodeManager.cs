using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [Tooltip("The scroll view the description is in.")]
    public ScrollRect descScrollRect;
    [Tooltip("The data SO of this node.")]
    public NodeData nodeData;
    [Tooltip("The image blocking input after transition to next node is triggered.")]
    public Image transitionBlockImage;
    [Tooltip("Travel button list (indexes 0-4 == N-E-S-W).")]
    public Button[] compassButtons = new Button[4];
    [Tooltip("Indicator of available quests on this node.")]
    public GameObject questIndicator;

    [Tooltip("Whether the current scene is a VN scene. Node is not initialized if it is.")]
    public bool vn;

    private SettingsController _settingsController;
    private JournalManager _journalManager;
    private InventoryManager _inventoryManager;
    private MapManager _mapManager;
    private DNCycleController _dnCycleController;
    private FlagManager _flagManager;
    private SceneTransition _sceneTransition;
    private AsyncOperation asyncLoadNextScene;

    private float _originalTitleFontSize;
    private float _originalDescFontSize;

    private PlayAnimationFromController viewCharacter;

    public void Awake()
    {
        if (vn)
            return;

        _originalDescFontSize = descTextBox.fontSize;
        _originalTitleFontSize = titleTextBox.fontSize;
    }

    public void Start()
    {
        _inventoryManager = PlayerSingleton.Instance.inventoryManager;
        _journalManager = PlayerSingleton.Instance.journalManager;
        _dnCycleController = PlayerSingleton.Instance.dnCycleController;
        _flagManager = PlayerSingleton.Instance.flagManager;

        // WARNING: persistent quest state set in the editor will be removed if CrossSceneData files exist
        CrossSceneDataSaver.LoadCrossNodeData();
        AddDefaultQuestStates();

        if (vn)
            return;

        _settingsController = PlayerSingleton.Instance.settingsController;
        _sceneTransition = PlayerSingleton.Instance.sceneTransition;
        _mapManager = PlayerSingleton.Instance.mapManager;

        InitViewCharacter();

        SetFontSize(_settingsController.settings.fontSize);
        titleTextBox.text = nodeData.node.title;
        WriteToDesc(nodeData.node.text);
        SetButtonsActive();

        _mapManager.Init();
        // this requires journal data
        SetIconsActive();
        if (_dnCycleController.curCycle.data.newDay)
            _dnCycleController.FadeInDay();
    }

    public void AddDefaultQuestStates()
    {
        foreach (QuestStateDataType questState in nodeData.defaultQuestStates)
        {
            if (!_journalManager.QuestIsTracked(questState))
            {
                _journalManager.TrackQuest(questState.quest.questName, questState);
                if (questState.quest.dayLimited)
                    questState.quest.dayLimitedLastTick = _dnCycleController.GetCurDayTick();
            }
        }
    }

    public void Explore()
    {
        if (GetQuest())
            return;

        if (SpawnRandomItem())
            return;

        if (AddRandomTextToDesc())
            return;

        AddToDesc("Seems like there's nothing of interest here.");
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
        // quest is on node
        if (!nodeData.questStates.ContainsKey(questName))
            return false;

        // quest has same state
        if (_journalManager.questStates[questName].state != nodeData.questStates[questName].state) 
            return false;

        // this throws an exception if no manual check for the quest being present is done
        QuestStateDataType questOnNode = nodeData.questStates[questName];
        QuestStateDataType questInJournal = _journalManager.questStates[questName];
        if (questOnNode.state != questInJournal.state)
            return false;

        // quest can be progressed
        int curDayTick = _dnCycleController.GetCurDayTick();
        if (questInJournal.quest.dayLimited && (questInJournal.quest.dayLimitedLastTick == curDayTick))
            return false;

        // quest is tracked, on node and can be progressed

        // if this is the last scene of the quest
        if (questOnNode.state == questOnNode.quest.questDescs.Count - 1)
            _journalManager.questStates.Remove(questName);

        Debug.Log($"Got quest from journal: {questName} playing scene {questInJournal.state}");
        if (questInJournal.quest.dayLimited)
            questInJournal.quest.dayLimitedLastTick = curDayTick;
        FadeAndLoadScene(questInJournal.quest.sceneNames[questInJournal.state++]);

        return true;
    }

    private void AddNewQuest(string questName)
    {
        // no randomness needed, just get the first one from the list that is not tracked
        QuestStateDataType quest = nodeData.questStates[questName];
        _journalManager.TrackQuest(questName, quest);
        if (quest.quest.dayLimited)
            quest.quest.dayLimitedLastTick = _dnCycleController.GetCurDayTick();
        FadeAndLoadScene(quest.quest.sceneNames[quest.state++]);
        Debug.Log($"Got new quest: {questName} playing first scene");
    }

    public bool SpawnRandomItem()
    {
        ItemSpawnerDataType itemSpawner = nodeData.itemSpawners[Random.Range(0, nodeData.itemSpawners.Count - 1)];
        // spawner is empty, tough luck
        if (itemSpawner.spawnerCapacity == 0)
            return false;

        ItemData item = itemSpawner.item;
        int quantityUpperRange = itemSpawner.spawnerCapacity == -1 ? itemSpawner.quantityRangeMax : itemSpawner.spawnerCapacity;
        int quantity = Random.Range(1, Mathf.Min(itemSpawner.quantityRangeMax, quantityUpperRange));

        if (itemSpawner.spawnerCapacity != -1)
            itemSpawner.spawnerCapacity -= quantity;

        AddItemToInventory(item, quantity);

        return true;
    }

    private void AddItemToInventory(ItemData item, int quantity)
    {
        _inventoryManager.AddToInventory(item, quantity);

        AddToDesc($"Found <color=#{(int)item.rarity:X6}>{item.itemName} [{item.rarity}]</color> x {quantity}");
    }

    private bool AddRandomTextToDesc()
    {
        if (nodeData.randomTextOnExplore.Count == 0)
            return false;

        // filter random text on cur node with flags
        List<NodeTextType> validTextTypes = nodeData.randomTextOnExplore
            .Where(item =>
            {
                if (item.requiresFlag)
                    if (item.flagValue != -1)
                        return _flagManager.ContainsFlag(item.flagId, item.flagValue);
                    else
                        return _flagManager.ContainsFlag(item.flagId);
                return true;
            }).ToList();

        if (validTextTypes.Count == 0)
            return false;

        NodeTextType randomTextType = validTextTypes[Random.Range(0, validTextTypes.Count)];
        AddToDesc(randomTextType.text);
        if (!randomTextType.viewCharAnim.Equals(""))
            viewCharacter.PlayAnimation(randomTextType.viewCharAnim);
        return true;
    }

    public void WriteToDesc(string text)
    {
        if (_descTextBoxBusy)
        {
            StopCoroutine(_descTextBoxWriteCoroutine.Item2);
            SetTextbox(_descTextBoxWriteCoroutine.Item1);
        }

        _descTextBoxWriteCoroutine = (text, StartCoroutine(WriteString(text)));

        StartCoroutine(DescScrollToBottom());
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

        StartCoroutine(DescScrollToBottom());
    }

    private void AddToTextbox(string text, int i, string originalText)
    {
        if (i < 0)
            descTextBox.SetText($"{originalText}\n\n{text}");
        else
            descTextBox.SetText($"{originalText}\n\n{text[..i]}");

        StartCoroutine(DescScrollToBottom());
    }

    public void SetFontSize(float sizeIncreaseFactor)
    {
        descTextBox.fontSize = sizeIncreaseFactor * _originalDescFontSize;
        //titleTextBox.fontSize = sizeIncreaseFactor * _originalTitleFontSize;
    }

    public void TravelToNextNode(int direction)
    {
        if (direction >= nodeData.node.nextNodeSceneNames.Count)
            return;

        string nextNodeName = nodeData.node.nextNodeSceneNames[direction];
        Travel(nextNodeName);
    }

    public void Travel(string sceneName)
    {
        if (sceneName == null || sceneName.Equals(""))
            return;

        // day night cycle on 
        int nextNodeNamePackageIndex = sceneName.IndexOf(DNCycleController.PACKAGE_IDENTIFIER);
        if (nextNodeNamePackageIndex != -1)
        {
            string nextNodeNameCut = sceneName.Remove(nextNodeNamePackageIndex, DNCycleController.PACKAGE_IDENTIFIER.Length);
            if (!nextNodeNameCut.StartsWith(nodeData.node.packageName))
                _dnCycleController.Progress();
        }

        FadeAndLoadScene(sceneName);
    }

    public void FadeAndLoadScene(string nextSceneName)
    {
        transitionBlockImage.enabled = true;
        _sceneTransition.FadeOutScene();

        CrossSceneDataSaver.SaveCrossNodeData();

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

    private void SetIconsActive()
    {
        foreach (QuestStateDataType quest in nodeData.questStates.Values)
        {
            if (_journalManager.QuestIsTracked(quest)
                && _journalManager.questStates[quest.quest.questName].state == quest.state
                && (!quest.quest.dayLimited || quest.quest.dayLimitedLastTick != _dnCycleController.GetCurDayTick()))
            {
                questIndicator.SetActive(true);
                break;
            }
        }
    }

    private void SetButtonsActive()
    {
        //SetAutoTravelButton();
        SetCompassButtons();
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

    private IEnumerator DescScrollToBottom()
    {
        yield return new WaitForEndOfFrame();

        descScrollRect.verticalNormalizedPosition = 0;
    }

    private void InitViewCharacter()
    {
        viewCharacter = GameObject.Find("ViewCharacter").GetComponent<PlayAnimationFromController>();
    }
}
