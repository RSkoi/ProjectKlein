using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
#endif

public class InputProxy : MonoBehaviour
{
    [Header("Player Input Values")]
    public UnityEvent progress = new();

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;

#if ENABLE_INPUT_SYSTEM
    public void OnProgress(InputValue value)
    {
        progress.Invoke();
    }

    public void OnProgressMouse(InputValue value)
    {
        var hitObject = UIRaycast(ScreenPosToPointerData(new(Input.mousePosition.x, Input.mousePosition.y)));
        bool hit = hitObject != null && hitObject.layer == LayerMask.NameToLayer("UI");
        if (!hit)
            progress.Invoke();
    }
#endif

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private static GameObject UIRaycast(PointerEventData pointerData)
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results.Count < 1 ? null : results[0].gameObject;
    }

    private static PointerEventData ScreenPosToPointerData(Vector2 screenPos)
       => new(EventSystem.current) { position = screenPos };
}
