using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class VisionSensing : MonoBehaviour
{
    private Guard guard;
    private Player player;
    [SerializeField] private LayerMask DetectionMask = 0;

    public Vector3 lastSeenPosition = Vector3.zero;

    public bool otherGuardsHaveVision = false;
    public bool guardNeedsHelp = false;


    [SerializeField] private float timeOnOuterVision = 0f;
    [SerializeField] private float timeOnInnerVision = 0f;

    private ThreatLevelSensing threatLevelSensing;

    public bool PlayerInProximityRadius = false;

    private void Start()
    {
        threatLevelSensing = GetComponent<ThreatLevelSensing>();
        guard = GetComponent<Guard>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        timeOnOuterVision = guard.guardType.outerVisionDetectionTime;
        timeOnInnerVision = guard.guardType.innerVisionDetectionTime;
    }

    public bool HasVisionOnPlayer()
    {
        //if(otherGuardsHaveVision) return true;
        if(PlayerInProximityRadius) return true;

        var vectorToTarget = player.transform.position - guard.EyeLocation.position;

        RaycastHit hit;

        if (Physics.Raycast(guard.EyeLocation.position, vectorToTarget, out hit, guard.InnerVisionConeRange) && IsTargetInVisionCone())
        {
            if (hit.collider.GetComponentInParent<Player>())
            {
                timeOnInnerVision -= Time.deltaTime;
                if(timeOnInnerVision <= 0)
                {
                    Debug.DrawLine(guard.EyeLocation.position, player.transform.position, Color.green);
                    lastSeenPosition = player.transform.position;
                    CommunicateToOtherGuardsInRadius();
                    return true;
                }
            }
            else timeOnInnerVision = guard.guardType.innerVisionDetectionTime;
        }      
        
        if (Physics.Raycast(guard.EyeLocation.position, vectorToTarget, out hit, guard.OuterVisionConeRange) && IsTargetInVisionCone())
        {
            if (hit.collider.GetComponentInParent<Player>())
            {
                timeOnOuterVision -= Time.deltaTime;
                if(timeOnOuterVision <= 0)
                {
                    Debug.DrawLine(guard.EyeLocation.position, player.transform.position, Color.green);
                    lastSeenPosition = player.transform.position;
                    CommunicateToOtherGuardsInRadius();    
                    return true;
                }

            }
            else timeOnOuterVision = guard.guardType.outerVisionDetectionTime;

        }

        return false;
    }

    private bool IsTargetInVisionCone()
    {
        var vectorToTarget = player.transform.position - guard.EyeLocation.position;
         
        if (Vector3.Dot(vectorToTarget, guard.EyeLocation.transform.forward) < guard.OuterVisionConeAngle)
            return true;

        return false;
    }

    public bool ReceiveHelpCommunication()
    {
        return guardNeedsHelp;
    }

    public void ReceiveCommunicationOnHostile()
    {
        otherGuardsHaveVision = true;
        StartCoroutine(Delay());
    }

    private void CommunicateToOtherGuardsInRadius()
    {   
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, guard.guardType.communicationRange);
        foreach (var hitCollider in hitColliders)
        {
            VisionSensing guard = hitCollider.GetComponent<VisionSensing>();
            if(guard)
            {
                guard.ReceiveCommunicationOnHostile();
                //Debug.Log("Communicated with " + guard.name);
                StartCoroutine(Delay());
            }
        }

    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(4f);
        otherGuardsHaveVision = false;
    }
  
}
