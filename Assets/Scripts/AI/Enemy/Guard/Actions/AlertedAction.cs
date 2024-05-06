using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertedAction : GOAPAction
{
    private bool alerted = false;
    private GameObject guardPoint;

    public float visionTimeOnPlayer = 0;
    public float timeForSuspicion = 4f;
    private bool suspicionOnPlayer = false;
    
    public AlertedAction()
    {
        addPrecondition("visionOnPlayer", true);
        addPrecondition("hasSuspicion", false);
        addPrecondition("isThreatened", false);

        addEffect("guardArea", true);
        addEffect("survive", true);

        cost = 1f;
    }

    public override void Awake()
    {
        base.Awake();
    }


    public override void Reset()
    {
        alerted = false;
        target = null;
    }

    public override bool isActionFinished()
    {
        return alerted;
    }

    public override bool requiresInRange()
    {
        return true; // do we need to be near the target for the action?
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        Guard guard = agent.GetComponent<Guard> ();
        guardPoint = guard.GuardingPoint;
        target = guardPoint;

        if(threatLevelSensing.suspicionState == ThreatLevelSensing.SuspicionState.Threatened) return false;


		if (target != null && visionSensing.HasVisionOnPlayer())
        {
    
            if(suspicionOnPlayer || Vector3.Distance(guard.transform.position, player.transform.position) <= 6)
            {
                threatLevelSensing.suspicionState = ThreatLevelSensing.SuspicionState.Suspicious;
                return false;
            }
			return true;
		}
        else 
        {
            visionTimeOnPlayer = 0;
            animator.SetBool("IsAlerted", false);
        }
        
        return false;
    }

    public override bool perform(GameObject agent)
    {
        animator.SetBool("IsAlerted", true);
        if(!suspicionOnPlayer) threatLevelSensing.suspicionState = ThreatLevelSensing.SuspicionState.Alarmed;
        LookAtPlayer(agent);
        StartCoroutine(StartSuspicionWaitTime());
		alerted = true;
		return alerted;
    }

    private IEnumerator StartSuspicionWaitTime()
    {
        yield return new WaitForSeconds(timeForSuspicion);
        suspicionOnPlayer = true;
        threatLevelSensing.suspicionState = ThreatLevelSensing.SuspicionState.Suspicious;
    }

    private void LookAtPlayer(GameObject agent)
    {
        Vector3 v3LookDirection = player.transform.position - agent.transform.position;
        v3LookDirection.y = 0;
        Quaternion qRotation = Quaternion.LookRotation(v3LookDirection);
        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, qRotation, 0.05f);
    }
}
