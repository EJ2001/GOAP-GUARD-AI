using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class PatrolAction : GOAPAction
{
    private bool patrolling = false;
    [HideInInspector] public bool isWaiting = false;
    
    private NavMeshAgent agent;
    private GameObject guardPoint;
    private int nextPatrolPoint = 0;
    [SerializeField] private float timeToWaitAtPoint = 3f;

    public PatrolAction()
    {
        addPrecondition("guardHasOpenedGate", false);
        addPrecondition("isArmed", true);
        addPrecondition("hasMemoryOnPlayerPosition", false);
        addPrecondition("visionOnPlayer", false);  
        addPrecondition("shouldPatrolArea", true);
        addPrecondition("otherGuardNeedsHelp", false);
        addPrecondition("isThreatened", false);


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
        patrolling = false;
        target = null;
    }

    public override bool isActionFinished()
    {
        return patrolling;
    }

    public override bool requiresInRange()
    {
        return true; // do we need to be near the target for the action?
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        guardPoint = guard.GuardingPoint;

        if (guard.patrolPointBuilder == null) return false;

        if (guard.patrolPointBuilder == null && guard.IsOnPatrol)
        {
            Debug.Log(guard.name + "<color=red> does not have a Waypoint Manager</color>");
            return false;
        }

        if (!guard.IsOnPatrol && guard.patrolPointBuilder != null)
        {
            Debug.Log(guard.name + "<color=red> has waypoint builder but IsOnPatrol is false</color>");
            return false;
        }

        return StartPatrolling();

    }

    private bool StartPatrolling()
    {
        if (!isWaiting && guard.patrolPointBuilder.patrolPoints.Count >= 1)
        {
            cost = 0.7f;
            PatrolToNextPoint();
            return true;
        }
        else return false;
    }

    public override bool perform(GameObject agent)
    {
        //Arrived at patrol point
        guard.leftGuardPosition = false;
        guard.movementState = BaseGuard.MovementState.walking;
        StartCoroutine(PatrolPointWaitDelay());

        patrolling = true;
		return patrolling;
    }



    // ** Patrolling ** //
    
    public void CreateWaypointManager()
    {    
        guard = GetComponent<Guard>();
        guard.waypointManager = new GameObject (gameObject.name + " - WaypointManager");
        guard.waypointManager.transform.parent = null;
        guard.waypointManager.AddComponent<PatrolPointBuilder>();
        guard.IsOnPatrol = true;
    }

    private void PatrolToNextPoint()
    {
        if(guard.patrolPointBuilder.patrolPoints.Count == 0)
        return;

        target = guard.patrolPointBuilder.patrolPoints[nextPatrolPoint].gameObject;
        animator.SetBool("IsWalking", true);
        guard.movementState = BaseGuard.MovementState.walking;
        nextPatrolPoint = (nextPatrolPoint + 1) % guard.patrolPointBuilder.patrolPoints.Count;
    }

    private IEnumerator PatrolPointWaitDelay()
    {
        isWaiting = true;
        animator.SetBool("IsWalking", false);
        yield return new WaitForSeconds(timeToWaitAtPoint);
        isWaiting = false;
        animator.SetBool("IsWalking", true);
    }

}

// Editor for creating patrol managers for each guard

[CustomEditor(typeof(PatrolAction))]
public class PatrolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PatrolAction guard = (PatrolAction)target;

        if(GUILayout.Button("Create Waypoint Manager"))
        {
            guard.CreateWaypointManager();
        }
    }
}
