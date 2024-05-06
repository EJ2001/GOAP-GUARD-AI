using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallForHelpAction : GOAPAction
{
    private bool calledForHelp = false;
    private GameObject guardPoint;
    
    private Transform nearestGuard;
    private Guard[] otherGuards;

    public CallForHelpAction()
    {
        addPrecondition("visionOnPlayer", true);
        addPrecondition("injured", true);

        addEffect("guardArea", true);
        addEffect("survive", true);

        cost = 1f;
    }


    public override void Reset()
    {
        calledForHelp = false;
        target = null;
    }

    public override bool isActionFinished()
    {
        return calledForHelp;
    }

    public override bool requiresInRange()
    {
        return true; // do we need to be near the target for the action?
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        Guard guard = agent.GetComponent<Guard> ();
        target = guard.gameObject;


		if (target != null && visionSensing.HasVisionOnPlayer() && guard.currentHealth <= 50)
        {
            if(GuardIsAlone())
            {
                cost = 0.05f;
            }
        
			return true;
		}
        
        return false;
    }

    private bool GuardIsAlone()
    {
        otherGuards = GameObject.FindObjectsOfType<Guard>();
        foreach(Guard otherGuard in otherGuards)
        {
            if(Vector3.Distance(guard.transform.position, otherGuard.transform.position) > 5)
            {
                return true;
            }
        }
        return false;
    }


    public override bool perform(GameObject agent)
    {
        LookAtNearestGuard();
        CallForGuardsInRadius();
		return calledForHelp;
    }
    
    private void CallForGuardsInRadius()
    {
        animator.Play("CallForHelp");
        otherGuards = GameObject.FindObjectsOfType<Guard>();
        foreach(Guard otherGuard in otherGuards)
        {
            if(Vector3.Distance(guard.transform.position, otherGuard.transform.position) <= 30)
            {
                otherGuard.GetComponent<ThreatLevelSensing>().suspicionState = ThreatLevelSensing.SuspicionState.Threatened;
                otherGuard.GetComponent<VisionSensing>().guardNeedsHelp = true;
            }
        }
        calledForHelp = true;
    }

    private void LookAtNearestGuard()
    {
        foreach(Guard otherGuard in otherGuards)
        {
            float minDist = Mathf.Infinity;
            float guardDist = Vector3.Distance(otherGuard.transform.position, guard.transform.position);
            if(guardDist < minDist)
            {
                nearestGuard = otherGuard.transform;
                minDist = guardDist;
            }
        }

        Vector3 v3LookDirection = nearestGuard.transform.position - guard.transform.position;
        v3LookDirection.y = 0;
        Quaternion qRotation = Quaternion.LookRotation(v3LookDirection);
        guard.transform.rotation = Quaternion.Slerp(guard.transform.rotation, qRotation, 0.05f);
    }
}
