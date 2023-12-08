using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : ControllerWithWindow
{
    [Tooltip("The item lookup table data. Associates an item SO with a GUID.")]
    public ItemLookupData itemDataForLookup;
    [Tooltip("The items currently held by the inventory.")]
    public List<ItemQuantityTuple> heldItems;

    [Tooltip("The inventory item entry container")]
    public GameObject entryContainer;
    [Tooltip("Prefab template for the items displayed in the inventory.")]
    public GameObject itemEntryPrefab;

    public void SetItems(ItemCrossSceneDataType loadedItemsData)
    {
        heldItems.Clear();
        foreach (ItemReferenceDataType item in loadedItemsData.items)
        {
            ItemData itemData = LookupItem(item.guid);
            ItemQuantityTuple itemTuple = new(itemData, item.quantity);
            heldItems.Add(itemTuple);
        }
    }

    public void AddToInventory(ItemData item, int quantity)
    {
        foreach (ItemQuantityTuple iqt in heldItems)
        {
            if (!iqt.item.itemName.Equals(item.itemName))
                continue;

            iqt.quantity += quantity;

            if (window.activeSelf)
                Repopulate();
            return;
        }

        // item not in inventory
        heldItems.Add(new(item, quantity));

        if (window.activeSelf)
            Repopulate();
    }

    public void RemoveFromInventory(ItemData item, int quantity)
    {
        ItemQuantityTuple tuple = null;
        foreach (ItemQuantityTuple iqt in heldItems)
        {
            if (!iqt.item.itemName.Equals(item.itemName))
                continue;

            tuple = iqt;
            break;
        }
        if (tuple == null)
            return;

        if (tuple.quantity - quantity <= 0)
            heldItems.Remove(tuple);
        else
            tuple.quantity -= quantity;

        if (window.activeSelf)
            Repopulate();
    }

    public bool HasQuantityOfItem(ItemData item, int quantity)
    {
        foreach (ItemQuantityTuple iqt in heldItems)
        {
            if (!iqt.item.itemName.Equals(item.itemName))
                continue;

            if (iqt.quantity >= quantity)
                return true;
            break;
        }

        return false;
    }

    public ItemCrossSceneDataType PrepareItemEntriesForSave()
    {
        List<ItemReferenceDataType> itemsToSave = new();
        foreach (ItemQuantityTuple irdt in heldItems)
            itemsToSave.Add(new(
                itemDataForLookup.lookupTable.FirstOrDefault(x => x.Value.GetInstanceID() == irdt.item.GetInstanceID()).Key,
                irdt.quantity
            ));

        return new(itemsToSave.ToArray());
    }

    public ItemData LookupItem(string guid)
    {
        return itemDataForLookup.lookupTable[guid];
    }

    public void Populate()
    {
        foreach (ItemQuantityTuple iqt in heldItems)
        {
            GameObject entry = Instantiate(itemEntryPrefab, entryContainer.transform);
            // this will potentially create problems if the prefab changes
            entry.transform.GetChild(0).GetComponent<TMP_Text>().text = iqt.item.itemName;
            entry.transform.GetChild(1).GetComponent<Image>().sprite = iqt.item.itemSprite;
            entry.transform.GetChild(2).GetComponent<TMP_Text>().text = $"x {iqt.quantity}";
        }
    }

    public void Depopulate()
    {
        // TODO: make more efficient?
        for (int i = 0; i < entryContainer.transform.childCount; i++)
            Destroy(entryContainer.transform.GetChild(i).gameObject);
    }

    public void Repopulate()
    {
        Depopulate();
        Populate();
    }

    public override void ToggleWindow()
    {
        window.SetActive(!window.activeSelf);

        if (window.activeSelf)
            Populate();
        else
            Depopulate();
    }
}
