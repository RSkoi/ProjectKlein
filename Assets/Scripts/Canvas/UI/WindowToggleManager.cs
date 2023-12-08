using System.Collections.Generic;
using UnityEngine;

public class WindowToggleManager : MonoBehaviour
{
    private Dictionary<string, ControllerWithWindow> windows = new();

    public void Start()
    {
        if (PlayerSingleton.Instance.historyController != null)
            windows.Add("history", PlayerSingleton.Instance.historyController);
        if (PlayerSingleton.Instance.settingsController != null)
            windows.Add("settings",PlayerSingleton.Instance.settingsController);
        if (PlayerSingleton.Instance.saveController != null)
            windows.Add("save", PlayerSingleton.Instance.saveController);
        if (PlayerSingleton.Instance.inventoryManager != null)
            windows.Add("inventory", PlayerSingleton.Instance.inventoryManager);
        if (PlayerSingleton.Instance.journalManager != null)
            windows.Add("journal", PlayerSingleton.Instance.journalManager);
        if (PlayerSingleton.Instance.mapManager != null)
            windows.Add("map", PlayerSingleton.Instance.mapManager);
    }

    public void ToggleWindow(string type)
    {
        foreach (string key in windows.Keys)
        {
            ControllerWithWindow controller = windows[key];
            GameObject window = controller.GetWindow();
            if (key.Equals(type))
            {
                controller.ToggleWindow();
                continue;
            }

            if (window.activeSelf)
                controller.ToggleWindow();
        }
    }
}
