using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TooltipController : MonoBehaviour
{
    [Tooltip("The lookup table for the tooltips")]
    public TooltipLookupData tooltipLookupData;
    [Tooltip("The tooltip container to be moved")]
    public GameObject tooltipPositioner;
    [Tooltip("The tooltip container to be filled")]
    public GameObject tooltipContainer;
    [Tooltip("The textbox inside the tooltip container")]
    public TMP_Text tooltipTextbox;
    private Animator _tooltipAnimator;
    [Tooltip("The name of the animated state to be used when closing tooltip")]
    public string tooltipAnimatedStateClose = "TooltipClose";
    [Tooltip("The name of the animated state to be used when opening tooltip")]
    public string tooltipAnimatedStateOpen = "TooltipOpen";
    [Tooltip("The event that is invoked when notifying the TooltipHoverHandler of a state change")]
    public UnityEvent<bool> OnTooltipStateChange;
    private Coroutine _tooltipCloseCleanupRoutine;

    public void Start()
    {
        _tooltipAnimator = tooltipContainer.GetComponent<Animator>();
        _tooltipAnimator.enabled = false;
    }

    public void SpawnTooltip(string id, Vector2 pos)
    {
        OnTooltipStateChange.Invoke(true);

        if (_tooltipCloseCleanupRoutine != null)
        {
            StopCoroutine(_tooltipCloseCleanupRoutine);
            _tooltipAnimator.StopPlayback();
            _tooltipCloseCleanupRoutine = null;
        }

        tooltipContainer.SetActive(true);
        _tooltipAnimator.enabled = true;
        _tooltipAnimator.Play(tooltipAnimatedStateOpen);

        string tooltipText = tooltipLookupData.tooltips[id];
        tooltipTextbox.SetText(tooltipText);

        Vector3 tooltipPos = tooltipPositioner.transform.position;
        tooltipPositioner.transform.position = new Vector3(pos.x, pos.y, tooltipPos.z);
    }

    public void CloseTooltip()
    {
        OnTooltipStateChange.Invoke(false);

        _tooltipAnimator.Play(tooltipAnimatedStateClose);
        _tooltipCloseCleanupRoutine = StartCoroutine(CloseTooltipCleanup());
    }

    private IEnumerator CloseTooltipCleanup()
    {
        yield return new WaitForSeconds(_tooltipAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length);

        _tooltipAnimator.StopPlayback();
        _tooltipAnimator.enabled = false;
        tooltipContainer.SetActive(false);
    }
}
