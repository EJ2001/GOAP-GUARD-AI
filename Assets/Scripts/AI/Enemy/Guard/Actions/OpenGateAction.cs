using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OpenGateAction : GOAPAction
{
    private bool openedGate = false;

    [SerializeField]
    private GameObject[] gateLevers;
    private Gate gate;

    public GameObject closestLever;

    public OpenGateAction()
    {
        addPrecondition("leftGuardingPosition", true);

        addEffect("isGateOpen", true);
        addEffect("guardHasOpenedGate", true);

        cost = 1f;
    }

    
    public override void Awake()
    {
        base.Awake();
        gateLevers = GameObject.FindGameObjectsWithTag("GateLever");
        gate = GameObject.Find("CastleGate").GetComponent<Gate>();
    }

    public override void Reset()
    {
        openedGate = false;
        target = null;
    }

    public override bool isActionFinished()
    {
        return openedGate;
    }

    public override bool requiresInRange()
    {
        return true; // do we need to be near the target for the action?
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
		if (gateLevers != null)
        {
            target = NearestLever();
			return true;
		}

        return false;
    }

    private GameObject NearestLever()
    {
        GameObject[] levers = GameObject.FindGameObjectsWithTag("GateLever");
        GameObject closestLever = null;
        float minDist = Mathf.Infinity;

        foreach(GameObject lever in levers)
        {
            float dist = Vector3.Distance(lever.transform.position, transform.position);
            if(dist < minDist)
            {
                closestLever = lever;
                minDist = dist;
            }

        }
        return closestLever;
    }

    public override bool perform(GameObject agent)
    {
        openedGate = true;
        guard.guardOpenedGate = true;
        gate.Open();
		return openedGate;
    }

}