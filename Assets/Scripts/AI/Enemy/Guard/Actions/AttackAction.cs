using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : GOAPAction
{
    private bool attacking = false;
    public float startAttackingDistance = 3f;
    
    public AttackAction()
    {
        addPrecondition("isArmed", true);
        addPrecondition("visionOnPlayer", true);
        addPrecondition("isThreatened", true);
        

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
        attacking = false;
        target = null;
    }

    public override bool isActionFinished()
    {
        return attacking;
    }

    public override bool requiresInRange()
    {
        return true; // do we need to be near the target for the action?
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        if(target == null) target = GameObject.FindGameObjectWithTag("Player").gameObject;


		if (target != null && PlayerInGuardArea() || threatLevelSensing.suspicionState == ThreatLevelSensing.SuspicionState.Threatened)
        {
            cost = 0.1f;
			return true;
		}
        else animator.SetBool("IsAttacking", false);


        return false;
    }

    private bool PlayerInGuardArea()
    {
        float dist = Vector3.Distance(target.transform.position, transform.position);
        if(dist < startAttackingDistance)
        {
            return true;
        }
        else return false;
    }

    public override bool perform(GameObject agent)
    {
        animator.SetBool("IsAttacking", true);
        if(PlayerIsInAttackRange()) animator.SetTrigger("SwordAttack");
        animator.SetBool("IsAlerted", true);
		attacking = true;
		return attacking;
    }

    private bool PlayerIsInAttackRange()
    {
        float dist = Vector3.Distance(target.transform.position, transform.position);
        if(dist < 2)
        {
            return true;
        }
        else return false;
    }

}
