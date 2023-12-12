using UnityEngine;

public class CrossSceneDataSaver : MonoBehaviour
{
    public static void SaveCrossNodeData()
    {
        // saveStates not relevant here

        JournalManager journalManager = PlayerSingleton.Instance.journalManager;
        InventoryManager inventoryManager = PlayerSingleton.Instance.inventoryManager;
        MapManager mapManager = PlayerSingleton.Instance.mapManager;

        JournalCrossSceneDataType saveDataJournal = journalManager.PrepareJournalEntriesForSave();
        DataSaver.SaveData(saveDataJournal, "journal");

        ItemCrossSceneDataType saveDataInventory = inventoryManager.PrepareItemEntriesForSave();
        DataSaver.SaveData(saveDataInventory, "inventory");

        MapNodeVisitedDictionary saveDataVisitedNodes = mapManager.PrepareVisitedNodeEntriesForSave();
        DataSaver.SaveData(saveDataVisitedNodes, "visitedNodes");
    }

    public static void LoadCrossNodeData()
    {
        // saveStates not relevant here

        JournalManager journalManager = PlayerSingleton.Instance.journalManager;
        InventoryManager inventoryManager = PlayerSingleton.Instance.inventoryManager;
        MapManager mapManager = PlayerSingleton.Instance.mapManager;

        JournalCrossSceneDataType loadedQuestData = DataSaver.LoadData<JournalCrossSceneDataType>("journal");
        if (loadedQuestData != null)
            journalManager.SetJournal(loadedQuestData);

        ItemCrossSceneDataType loadedItemsData = DataSaver.LoadData<ItemCrossSceneDataType>("inventory");
        if (loadedItemsData != null)
            inventoryManager.SetItems(loadedItemsData);

        MapNodeVisitedDictionary loadedVisitedNodesData = DataSaver.LoadData<MapNodeVisitedDictionary>("visitedNodes");
        if (loadedVisitedNodesData != null)
            mapManager.SetVisistedNodes(loadedVisitedNodesData);
    }

    public static void DeleteCrossNodeData()
    {
        DataSaver.DeleteData("saveStates");
        DataSaver.DeleteData("journal");
        DataSaver.DeleteData("inventory");
        DataSaver.DeleteData("visitedNodes");
    }
}
