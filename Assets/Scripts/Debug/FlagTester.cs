using TMPro;
using UnityEditor;
using UnityEngine;

public class FlagTester : MonoBehaviour
{
    public FlagManager flagManager;
    public TMP_InputField flagVal;

    public void Start()
    {
        flagManager = PlayerSingleton.Instance.flagManager;
    }

    public void AddTestFlags()
    {
        for (int i = 0; i < 5; i++)
            flagManager.AddFlag(System.Guid.NewGuid().ToString());
    }

    public void AddTestFlag()
    {
        flagManager.AddFlag("TestFlag", int.Parse(flagVal.text), FlagUpdateBehaviours.LogFlagChange);
    }
}
