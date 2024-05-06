using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="ScriptableObj/Guards")]
public class GuardData : ScriptableObject
{
    public string guardType;

    public GameObject prefab = null;
    public GameObject weaponPrefab = null;
    
    [Header("Vitals:")]
    public float maxHealth = 100f;

    [Header("Core:")]
    public float maxHunger = 100f;
    public float maxStamina = 100f;

    [Header("Field Of View:")]
    [Space(10)]
    public float outerVisionConeAngle = 55f;
    public float outerVisionConeRange = 20f;
    public Color outerVisionConeColour = new Color(0f, 1f, 0.9f, 0.25f);
    public float outerVisionDetectionTime = 2f;

    public float innerVisionConeAngle = 30f;
    public float innerVisionConeRange = 13f;
    public Color innerVisionConeColour = new Color(255, 0, 0, 0.25f);
    public float innerVisionDetectionTime = .25f;

    [Header("Proximity Detection:")]
    public float detectionRange = 5f;
    public Color rangeColour = new Color(1f, 1f, 1f, 0.25f);

    [Header("Communication:")]
    public float communicationRange = 13f;

    [Header("Other:")]
    public bool showDebugUI = true;
}
