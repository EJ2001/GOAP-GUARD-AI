using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HoldingGuardPositionAction : GOAPAction
{
    private bool guarding = false;
    
    private NavMeshAgent agent;
    private GameObject guardPoint;

    public HoldingGuardPositionAction()
    {
        addPrecondition("isTired", false);
        addPrecondition("isHungry", false);

        addPrecondition("guardHasOpenedGate", false);
        addPrecondition("isArmed", true);
        addPrecondition("visionOnPlayer", false);
        addPrecondition("shouldPatrolArea", false);

        addEffect("guardArea", true);
        addEffect("survive", true);

        cost = 1f;
    }

    public override void Awake()
    {
        base.Awake();
    }

    void Update()
    {

    }

    public override void Reset()
    {
        guarding = false;
        target = null;
    }

    public override bool isActionFinished()
    {
        return guarding;
    }

    public override bool requiresInRange()
    {
        return true; // do we need to be near the target for the action?
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        guardPoint = guard.GuardingPoint;
        target = guardPoint;

		if (guardPoint != null && !visionSensing.HasVisionOnPlayer() && !guard.IsOnPatrol)
        {
			return true;
		}
        else return false;

    }

    public override bool perform(GameObject agent)
    {
        guard.leftGuardPosition = false;
        animator.SetBool("IsWalking", false);
        guarding = true;

        // Check to see if player moves out of vision after detection
        if(threatLevelSensing.suspicionState == ThreatLevelSensing.SuspicionState.Suspicious && !visionSensing.HasVisionOnPlayer())
        {
            StartCoroutine(PlayerStillNotInVision());
        }
		return guarding;
    }

    // Guard unaware of player after delay
    private IEnumerator PlayerStillNotInVision()
    {
        yield return new WaitForSeconds(5);
        threatLevelSensing.suspicionState = ThreatLevelSensing.SuspicionState.Unaware;
    }


}
