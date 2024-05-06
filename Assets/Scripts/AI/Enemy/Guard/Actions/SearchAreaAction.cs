using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class SearchAreaAction : GOAPAction
{
    public float timeToLookAround = 8f;
    public float timeToWaitAtEachPoint = 3f;
    public float radiusSizeAroundLastSeenPosition = 3f;

    private bool searchingArea = false;
    private bool isWaiting = false;

    private GameObject guardPoint;
    private Vector3 randomSearchPosition = Vector3.zero;
    private DebugAI debugAI;

    public SearchAreaAction()
    {
        addPrecondition("isArmed", true);
        addPrecondition("visionOnPlayer", false);
        addPrecondition("hasMemoryOnPlayerPosition", true);
        addPrecondition("shouldPatrolArea", true);
        


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
        searchingArea = false;
        target = null;
    }

    public override bool isActionFinished()
    {
        return searchingArea;
    }

    public override bool requiresInRange()
    {
        return true; // do we need to be near the target for the action?
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        if (guard.patrolPointBuilder == null) return false;

        guardPoint = guard.GuardingPoint;

        if (visionSensing.HasVisionOnPlayer()) return false;

        return CanSearchArea();

    }

    // Check to see if player moved out of vision if there was a last seen position 
    private bool CanSearchArea()
    {
        if (visionSensing.lastSeenPosition != Vector3.zero && !visionSensing.HasVisionOnPlayer() && !isWaiting)
        {
            cost = 0.8f;
            guard.MoveToObject.transform.position = visionSensing.lastSeenPosition;
            threatLevelSensing.suspicionState = ThreatLevelSensing.SuspicionState.Suspicious;
            target = guard.MoveToObject;
            animator.SetBool("IsWalking", true);
            return true;
        }
        else return false;
    }

    public override bool perform(GameObject agent)
    {
        Debug.Log("SearchingArea");
        searchingArea = true;

        StartCoroutine(StartSuspicionWaitTime());

        StartCoroutine(WaitAtPointDelay());

		return searchingArea;
    }

    private IEnumerator StartSuspicionWaitTime()
    {
        yield return new WaitForSeconds(timeToLookAround);
        visionSensing.lastSeenPosition = Vector3.zero;
    }
    private IEnumerator WaitAtPointDelay()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeToWaitAtEachPoint);
            CheckAroundArea();
        }

    }
    private IEnumerator IsWaitingDelay()
    {
        isWaiting = true;
        yield return new WaitForSeconds(1.5f); 
        isWaiting = false;
    }

    // Patrol around last seen position
    private void CheckAroundArea()
    {
        if(navAgent == null) navAgent = GetComponent<NavMeshAgent>();
        
        if(!isWaiting)
        {
            Vector3 randPos = visionSensing.lastSeenPosition + RandomPointOnCircleEdge(radiusSizeAroundLastSeenPosition);

            Debug.DrawRay(randPos, Vector3.up, Color.cyan, 2f);
            guard.MoveToObject.transform.position = randPos;
            navAgent.isStopped = false;
            navAgent.SetDestination(guard.MoveToObject.transform.position);

            StartCoroutine(IsWaitingDelay());
        }

    }

    // Get random point on a circle edge to patrol around searched area
    private Vector3 RandomPointOnCircleEdge(float radius)
    {
        var vector2 = UnityEngine.Random.insideUnitCircle.normalized * radius;
        return new Vector3(vector2.x, 0, vector2.y);
    }

}



