using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendGuardAction : GOAPAction
{
    private bool helpingGuard = false;
    private GameObject guardPoint;
    
    private Transform nearestGuard;
    private Guard[] otherGuards;

    public DefendGuardAction()
    {
        addPrecondition("isArmed", true);
        addPrecondition("injured", false);
        addPrecondition("guardHasOpenedGate", false);

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
        helpingGuard = false;
        target = null;
    }

    public override bool isActionFinished()
    {
        return helpingGuard;
    }

    public override bool requiresInRange()
    {
        return true; // do we need to be near the target for the action?
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        target = player.gameObject;


		if (target != null && visionSensing.guardNeedsHelp)
        {
            threatLevelSensing.suspicionState = ThreatLevelSensing.SuspicionState.Threatened;
            cost = 0.2f;
			return true;
		}
  
        return false;
    }

    public override bool perform(GameObject agent)
    {
        visionSensing.guardNeedsHelp = false;
		return helpingGuard;
    }
}
