using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HistoryController : ControllerWithWindow
{
    [Tooltip("The history textbox.")]
    public TMP_Text textBox;
    [Tooltip("The history scroll rect.")]
    public ScrollRect historyScrollRect;

    public void AddLine(string line)
    {
        textBox.text += $"{line}\n\n";

        StartCoroutine(HistoryScrollToBottom());
    }

    public void AddLine(string name, string line)
    {
        if (name.Equals(""))
        {
            AddLine(line);
            return;
        }

        textBox.text += $"{name}: {line}\n\n";

        StartCoroutine(HistoryScrollToBottom());
    }

    private IEnumerator HistoryScrollToBottom()
    {
        yield return new WaitForEndOfFrame();

        historyScrollRect.verticalNormalizedPosition = 0;
    }

    public override void ToggleWindow()
    {
        if (window.activeSelf)
        {
            window.SetActive(false);
            // TODO: doesn't scroll to bottom entirely for some reason
            StartCoroutine(HistoryScrollToBottom());
        }
        else
            window.SetActive(true);
    }
}
