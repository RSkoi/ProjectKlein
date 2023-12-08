using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TooltipHoverHandler : MonoBehaviour
{
    private bool _tooltipState = false;

    [Tooltip("The container of the textbox that can contain tooltip links")]
    public GameObject textBoxContainer;
    private TMP_Text _textBox;
    private RectTransform _textBoxRect;
    [Tooltip("The tooltip container")]
    public GameObject tooltipContainer;
    private RectTransform _tooltipRect;
    [Tooltip("The event to be invoked when hovering over a tooltip. Transmits the tooltip ID and mouse position.")]
    public UnityEvent<string, Vector2> OnTooltipHover;
    [Tooltip("The event to be invoked when closing a tooltip.")]
    public UnityEvent OnTooltipClose;

    public void Start()
    {
        _textBox = textBoxContainer.GetComponent<TMP_Text>();
        _textBoxRect = textBoxContainer.GetComponent<RectTransform>();
        _tooltipRect = tooltipContainer.GetComponent<RectTransform>();
    }

    public void Update()
    {
        /* TMP_TextUtilities.IsIntersectingRectTransform(_tooltipRect, mousePos, Camera.current) inside an if-clause
         * gives a minimal performance boost since && is a logical AND, and will shortcircuit
         * => make the call only if we really need it
         * this may or may not be unnecessary in case the compiler does this automatically */
        Vector2 mousePos = Mouse.current.position.ReadValue();
        bool hoverOverTextbox = TMP_TextUtilities.IsIntersectingRectTransform(_textBoxRect, mousePos, Camera.current);
        if (!hoverOverTextbox)
        {
            if (_tooltipState && !TMP_TextUtilities.IsIntersectingRectTransform(_tooltipRect, mousePos, Camera.current))
                OnTooltipClose.Invoke();
            return;
        }

        int link = TMP_TextUtilities.FindIntersectingLink(_textBox, mousePos, Camera.current);
        if (_tooltipState && link == -1 && !TMP_TextUtilities.IsIntersectingRectTransform(_tooltipRect, mousePos, Camera.current))
            OnTooltipClose.Invoke();

        if (link == -1)
            return;

        if (!_tooltipState)
            OnTooltipHover.Invoke(_textBox.textInfo.linkInfo[link].GetLinkID(), mousePos);
    }

    public void SetTooltipState(bool state)
    {
        _tooltipState = state;
    }
}
