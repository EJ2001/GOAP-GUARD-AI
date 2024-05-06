using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Class for updating the actions and threat level on the player canvas
public class DebugAI : MonoBehaviour
{
    public TMP_Text currentActionText;
    public TMP_Text threatLevelStateText;

    private Transform canvas;

    void Start()
    {
        if(currentActionText == null) Debug.Log("<color=red>Current action text needs to be dragged into the DebugAI script</color> ");
        if(threatLevelStateText == null) Debug.Log("<color=red>Threat level text needs to be dragged into the DebugAI script</color> ");
    }

    public void UpdateActionText(string text)
    {
        currentActionText.text = "CurrentAction: " + text;
    }

    public void UpdateThreatLevelText(string text)
    {
        threatLevelStateText.text = "Threat Level: " + text;
    }
}

