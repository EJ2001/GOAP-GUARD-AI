using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class RestAction : GOAPAction
{
    private bool resting = false;
    
    private NavMeshAgent agent;
    private GameObject guardPoint;
    [SerializeField]
    private GameObject restingBarracks;

    [SerializeField]
    private float timeToRest = 5f;


    public RestAction()
    {
      
        addPrecondition("visionOnPlayer", false);
        addPrecondition("isTired", true);
        addPrecondition("otherGuardsGuarding", true);
        addPrecondition("leftGuardingPosition", true);

        addEffect("isTired", false);

        cost = 1f;
    }

    public override void Awake()
    {
        base.Awake();
        restingBarracks = GameObject.Find("GuardRestingArea");
    }


    public override void Reset()
    {
        resting = false;
        target = null;
    }

    public override bool isActionFinished()
    {
        return resting;
    }

    public override bool requiresInRange()
    {
        return true; // do we need to be near the target for the action?
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
		guard = agent.GetComponent<Guard> ();
        visionSensing = guard.GetComponent<VisionSensing>();

        cost = guard.hunger / 150;

        if(restingBarracks != null)
        {
            target = restingBarracks;
            return true;
        }

        return false;

    }

    public override bool perform(GameObject agent)
    {
        StartCoroutine(RestDelay());
		return resting;
    }

    private IEnumerator RestDelay()
    {
        resting = false;
        yield return new WaitForSeconds(timeToRest); 
        resting = true;
        guard.stamina = guard.guardType.maxStamina;
    }


}


