using UnityEngine;

public class DNCycleController : MonoBehaviour
{
    public readonly static string SPENT_DAYS_FLAG_NAME = "SPENT_DAYS_AMOUNT";

    private FlagManager _flagManager;

    [Tooltip("The day/night cycle data")]
    public DnCycleData curCycle;
    [Tooltip("The animation that should be played on day transition")]
    public Animation dayTransitionFadeAnim;

    public void Start()
    {
        _flagManager = PlayerSingleton.Instance.flagManager;
    }

    public void SkipToNextDay()
    {
        if (!_flagManager.IncrementFlag(SPENT_DAYS_FLAG_NAME))
            _flagManager.AddFlag(SPENT_DAYS_FLAG_NAME, 1);
        curCycle.data.newDay = true;
    }

    public void Progress()
    {
        // TODO: implement night version of nodes?
        SkipToNextDay();
    }

    public void FadeInDay()
    {
        dayTransitionFadeAnim.Play();
        curCycle.data.newDay = false;
    }

    public void SetCycle(DNCycleDataType cycle)
    {
        curCycle.data.isDay = cycle.isDay;
        curCycle.data.newDay = cycle.newDay;
    }

    public DNCycleDataType PrepareCycleForSave()
    {
        return curCycle.data;
    }

    public int GetCurDayTick()
    {
        int i = _flagManager.GetFlagIndex(SPENT_DAYS_FLAG_NAME);
        if (i == -1)
            return 0;
        else
            return _flagManager.GetFlag(i).value;
    }
}
