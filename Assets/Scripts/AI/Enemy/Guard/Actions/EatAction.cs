using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EatAction : GOAPAction
{
    private bool eating = false;
    
    private NavMeshAgent agent;
    private GameObject guardPoint;

    [SerializeField] private float timeToWaitAtTarget = 3f;
    [SerializeField] private GameObject eatingArea;


    public EatAction()
    {
        addPrecondition("guardHasOpenedGate", true);
        addPrecondition("isHungry", true);
        addPrecondition("otherGuardsGuarding", true);
        addPrecondition("leftGuardingPosition", true);
        addPrecondition("visionOnPlayer", false);
        addPrecondition("isThreatened", false);

        addEffect("survive", true);
        addEffect("guardArea", true);
        addEffect("isHungry", false);

        cost = 1f;
    }


    public override void Awake()
    {
        base.Awake();
        eatingArea = GameObject.Find("GuardFoodArea");
    }


    public override void Reset()
    {
        eating = false;
        target = null;
    }

    public override bool isActionFinished()
    {
        return eating;
    }

    public override bool requiresInRange()
    {
        return true; // do we need to be near the target for the action?
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        target = eatingArea;
        
        if (eatingArea != null)
        {
            cost = guard.hunger / 150;
			return true;
		}
        else return false;

    }

    // When reached target will start eating
    public override bool perform(GameObject agent)
    {
        StartCoroutine(EatDelay());
		return eating;
    }

    private IEnumerator EatDelay()
    {
        eating = false;
        yield return new WaitForSeconds(timeToWaitAtTarget); 
        eating = true;
        guard.hunger = guard.guardType.maxHunger;
    }

}


