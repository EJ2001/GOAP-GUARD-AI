using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LeaveGuardPositionAction : GOAPAction
{
    private bool leftPosition = false;
    
    private NavMeshAgent agent;
    private GameObject guardPoint;

    public LeaveGuardPositionAction()
    {
        addPrecondition("otherGuardsGuarding", true);
        
        addEffect("leftGuardingPosition", true);

        cost = 1f;
    }

    public override void Reset()
    {
        leftPosition = false;
        target = null;
    }

    public override bool isActionFinished()
    {
        return leftPosition;
    }

    public override bool requiresInRange()
    {
        return true; // do we need to be near the target for the action?
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {

        if (guard != null)
        {
            target = guard.gameObject;
			return true;
		}
        else return false;

    }

    public override bool perform(GameObject agent)
    {
        animator.SetBool("IsWalking", true);
        guard.leftGuardPosition = true;
        leftPosition = true;
		return leftPosition;
    }

}
