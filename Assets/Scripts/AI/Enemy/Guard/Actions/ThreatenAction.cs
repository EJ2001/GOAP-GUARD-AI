using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreatenAction : GOAPAction
{
    private bool threaten = false;
    private bool threatenAnimationPlayed = false;

    public float threatRange = 3f;
    public float timeToAggro = 6f;

    public ThreatenAction()
    {
        addPrecondition("isArmed", true);
        addPrecondition("hasSuspicion", true);
        addPrecondition("visionOnPlayer", true);

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
        threaten = false;
        target = null;
    }

    public override bool isActionFinished()
    {
        return threaten;
    }

    public override bool requiresInRange()
    {
        return false; // do we need to be near the target for the action?
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {

        target = player;

        if(player != null)
        {
            cost = 0.6f; 
            if(!PlayerInThreatRange() && threatLevelSensing.suspicionState == ThreatLevelSensing.SuspicionState.Suspicious)
            {
                animator.SetBool("IsAlerted", true);
                return true;
            }
            
            if(PlayerInThreatRange() || threatLevelSensing.suspicionState == ThreatLevelSensing.SuspicionState.Threatened)
            {
                threatLevelSensing.suspicionState = ThreatLevelSensing.SuspicionState.Threatened;
                cost = 0.2f;
                return false;
            }
        }
        return false;
    }

    public override bool perform(GameObject agent)
    {
        agent.GetComponent<DebugAI>().UpdateActionText(this.ActionName);
        navAgent.isStopped = true;
        LookAtPlayer(agent);
        if(!threatenAnimationPlayed) animator.Play("Threaten");
        threatenAnimationPlayed = true;

        if(visionSensing.HasVisionOnPlayer())
        {
            StartCoroutine(PlayerStillInArea());
        }


        return false;
    }

    private IEnumerator PlayerStillInArea()
    {
        yield return new WaitForSeconds(timeToAggro);
        threatLevelSensing.suspicionState = ThreatLevelSensing.SuspicionState.Threatened;
    }

    private bool PlayerInRange()
    {
        if(Vector3.Distance(transform.position, target.transform.position) <= 13)
        {
            return true;
        }
        else return false;
    }

    private bool PlayerInThreatRange()
    {
        if(Vector3.Distance(transform.position, target.transform.position) <= threatRange)
        {
            return true;
        }
        else return false;
    }

    private void LookAtPlayer(GameObject agent)
    {
        Vector3 v3LookDirection = player.transform.position - agent.transform.position;
        v3LookDirection.y = 0;
        Quaternion qRotation = Quaternion.LookRotation(v3LookDirection);
        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, qRotation, 0.05f);
    }

}
