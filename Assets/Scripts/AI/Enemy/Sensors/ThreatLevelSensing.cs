using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreatLevelSensing : MonoBehaviour
{
    private DebugAI debugAI = null;

    public enum SuspicionState
    {
        Unaware,
        Alarmed,
        Suspicious,
        Threatened,
    } 

    public SuspicionState suspicionState;
        
    private void Start()
    {
        debugAI = GetComponent<DebugAI>();
        suspicionState = SuspicionState.Unaware;
    }

    private void Update()
    {
        switch(suspicionState)
        {
            case SuspicionState.Unaware:
                break;
            case SuspicionState.Alarmed:
                break;
            case SuspicionState.Suspicious:
                break;
            case SuspicionState.Threatened:
                break; 
        }
    }


}
