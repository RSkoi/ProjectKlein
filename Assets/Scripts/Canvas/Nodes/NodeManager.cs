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

    [HideInInspector]
    public readonly static string PACKAGE_IDENTIFIER = "CA_NodeMap_";
    [HideInInspector]
    public string _travelNextPackageAndSceneName;

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

        if (_flagManager.ContainsFlag(SceneDirector.SCENE_LOADED_FLAG))
            _flagManager.DeleteFlag(SceneDirector.SCENE_LOADED_FLAG);
    }

    public void AddDefaultQuestStates()
    {
        foreach (QuestStateDataType questState in nodeData.defaultQuestStates)
        {
            if (!_journalManager.QuestIsTracked(questState))
            {
                _journalManager.TrackQuest(questState);
                if (questState.quest.dayLimited)
                    questState.quest.dayLimitedLastTick = _dnCycleController.GetCurDayTick();
            }
        }
    }

    public void Explore()
    {
        if (GetQuest())
            return;

        int action = UnityEngine.Random.Range(0, 1);
        switch (action)
        {
            case 0:
                if (SpawnRandomItem())
                    return;
                if (action != 0)
                    break;
                goto case 1;
            case 1:
                if (AddRandomTextToDesc())
                    return;
                if (action != 1)
                    break;
                goto case 0;
        }

        AddToDesc("Seems like there's nothing of interest here.");
    }

    public bool GetQuest()
    {
        foreach ((string questGuidTracked, QuestStateDataType qsdt) in _journalManager.questStates)
            if (CheckQuestTrackedAndUpdate(questGuidTracked, qsdt))
                return true;

        (string questGuid, NodeQuestStateDataType nodeQuestState)
            = nodeData.nodeQuestStates.FirstOrDefault(x => !_journalManager.questStates.ContainsKey(x.Key));

        if (string.IsNullOrEmpty(questGuid))
            return false;

        // quest is not tracked and on node
        AddNewQuest(questGuid, nodeQuestState);
        return true;
    }

    private bool CheckQuestTrackedAndUpdate(string questGuid, QuestStateDataType questInJournal)
    {
        // quest is on node
        if (!nodeData.nodeQuestStates.ContainsKey(questGuid))
            return false;

        // quest on node has the "next" (tracked by journal) state
        int questOnNodeIndex = nodeData.nodeQuestStates[questGuid].states.IndexOf(questInJournal.state);
        if (questOnNodeIndex == -1)
            return false;

        // quest has same state
        //if (!nodeData.nodeQuestStates[questGuid].states.Contains(_journalManager.questStates[questGuid].state)) 
        //    return false;

        // this throws an exception if no manual check for the quest being present is done
        NodeQuestStateDataType questsOnNode = nodeData.nodeQuestStates[questGuid];
        QuestData questOnNode = questsOnNode.quests[questOnNodeIndex];

        // quest can be progressed
        int curDayTick = _dnCycleController.GetCurDayTick();
        if (questInJournal.quest.dayLimited && (questInJournal.quest.dayLimitedLastTick == curDayTick))
            return false;

        // quest is tracked, on node and can be progressed

        // this is the last scene of the quest
        bool lastSceneOfQuest = questInJournal.state == questOnNode.questDescs.Count - 1;
        if (lastSceneOfQuest && !questOnNode.wip)
            _journalManager.questStates.Remove(questGuid);

        if (questInJournal.quest.dayLimited)
            questInJournal.quest.dayLimitedLastTick = curDayTick;

        Debug.Log($"Got quest from journal: {questInJournal.quest.questName} playing scene {questInJournal.state}");
        // empty scene names should be allowed in wip scenes => return false
        if (!string.IsNullOrEmpty(questInJournal.quest.sceneNames[questInJournal.state]))
            FadeAndLoadScene(questInJournal.quest.sceneNames[questInJournal.state++]);
        else
            return false;

        return true;
    }

    private void AddNewQuest(string questGuid, NodeQuestStateDataType nodeQuestState)
    {
        // get the first quest and state from the top of the lists in NodeQuestStateDataType
        QuestStateDataType questState = new(nodeQuestState.quests[0], nodeQuestState.states[0]);
        _journalManager.TrackQuest(questState);
        if (questState.quest.dayLimited)
            questState.quest.dayLimitedLastTick = _dnCycleController.GetCurDayTick();
        FadeAndLoadScene(questState.quest.sceneNames[questState.state++]);
        Debug.Log($"Got new quest: {questState.quest.questName} playing first scene");
    }

    public bool SpawnRandomItem()
    {
        if (nodeData.itemSpawners.Count == 0)
            return false;

        ItemSpawnerDataType itemSpawner = nodeData.itemSpawners[UnityEngine.Random.Range(0, nodeData.itemSpawners.Count - 1)];
        // spawner is empty, tough luck
        if (itemSpawner.spawnerCapacity == 0)
            return false;

        ItemData item = itemSpawner.item;
        int quantityUpperRange = itemSpawner.spawnerCapacity == -1 ? itemSpawner.quantityRangeMax : itemSpawner.spawnerCapacity;
        int quantity = UnityEngine.Random.Range(1, Mathf.Min(itemSpawner.quantityRangeMax, quantityUpperRange));

        if (itemSpawner.spawnerCapacity != -1)
            itemSpawner.spawnerCapacity -= quantity;

        AddItemToInventory(item, quantity);

        return true;
    }

    private void AddItemToInventory(ItemData item, int quantity)
    {
        _inventoryManager.AddToInventory(item, quantity);

        AddToDesc($"Found {item} x {quantity}");
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

        NodeTextType randomTextType = validTextTypes[UnityEngine.Random.Range(0, validTextTypes.Count)];
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
        if (direction >= nodeData.node.nextNodeSceneLists.Count)
            return;

        string nextNodeName = nodeData.node.nextNodeSceneLists[direction].sceneName;
        Travel(nextNodeName);
    }

    public void Travel(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
            return;

        _travelNextPackageAndSceneName = "Unknown";
        int nextNodeNamePackageIndex = sceneName.IndexOf(PACKAGE_IDENTIFIER);
        if (nextNodeNamePackageIndex != -1)
            _travelNextPackageAndSceneName = sceneName.Remove(nextNodeNamePackageIndex, PACKAGE_IDENTIFIER.Length);

        // travel conditions
        List<string> failedConditionsText = ScriptedCondition.CheckConditions(nodeData.node.travelConditions);
        if (failedConditionsText.Count > 0)
        {
            foreach (string text in failedConditionsText)
                if (!string.IsNullOrEmpty(text))
                    AddToDesc(text);

            if (_mapManager.window.activeSelf)
                _mapManager.ToggleWindow();
            return;
        }

        // travel effects
        ScriptedEffect.InvokeEffects(nodeData.node.travelEffects);

        // day night cycle
        if (!_travelNextPackageAndSceneName.StartsWith(nodeData.node.packageName))
            _dnCycleController.Progress();

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
        foreach ((string questGuid, NodeQuestStateDataType nodeQuests) in nodeData.nodeQuestStates)
        {
            QuestData quest = _journalManager.LookupQuest(questGuid);
                // quest is tracked by journal
            if (_journalManager.QuestIsTracked(questGuid)
                // node contains next state of this quest
                && nodeQuests.states.IndexOf(_journalManager.questStates[questGuid].state) != -1
                // quest can be progressed
                && (!quest.dayLimited || quest.dayLimitedLastTick != _dnCycleController.GetCurDayTick())
                // not last scene of wip quest
                && ((_journalManager.questStates[questGuid].state == quest.questDescs.Count - 1) ? !quest.wip : true))
            {
                questIndicator.SetActive(true);
                break;
            }
        }
    }

    private void SetButtonsActive()
    {
        SetCompassButtons();
    }

    private void SetCompassButtons()
    {
        if (nodeData.node.nextNodeSceneLists == null
            || nodeData.node.nextNodeSceneLists.Count == 0)
            return;

        int i = 0;
        foreach (NextNodeListType nextNode in nodeData.node.nextNodeSceneLists)
        {
            if (!string.IsNullOrEmpty(nextNode.sceneName)
                && ScriptedCondition.CheckConditions(nextNode.sceneTravelConditions).Count == 0)
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
