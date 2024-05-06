using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CloseGateAction : GOAPAction
{
    private bool closedGate = false;

    [SerializeField]
    private GameObject gateLever;
    private Gate gate;

    public CloseGateAction()
    {
        addEffect("isGateOpen", false);
        addEffect("guardHasOpenedGate", false);

        cost = 1f;
    }

    public override void Awake()
    {
        base.Awake();
        gateLever = GameObject.Find("GateLever");
        gate = GameObject.Find("CastleGate").GetComponent<Gate>();
    }

    public override void Reset()
    {
        closedGate = false;
        target = null;
    }

    public override bool isActionFinished()
    {
        return closedGate;
    }

    public override bool requiresInRange()
    {
        return true; // do we need to be near the target for the action?
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
		if (gateLever != null)
        {
            target = gateLever;
            return true;
		}
        else return false;
    }

    public override bool perform(GameObject agent)
    {
        closedGate = true;
        guard.guardOpenedGate = false;
        gate.Close();
		return closedGate;
    }

}