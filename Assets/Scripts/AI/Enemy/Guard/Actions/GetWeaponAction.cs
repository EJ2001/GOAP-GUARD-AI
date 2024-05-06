using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GetWeaponAction : GOAPAction
{
    private bool hasWeapon = false;

    [SerializeField]
    private GameObject weaponRack;

    public GetWeaponAction()
    {
        addPrecondition("isGateOpen", true);
        addPrecondition("isArmed", false);

        addEffect("survive", true);
        addEffect("isArmed", true);

        cost = 1f;
    }

    public override void Awake()
    {
        base.Awake();
        weaponRack = GameObject.Find("WeaponRack");
    }

    public override void Reset()
    {
        hasWeapon = false;
        target = null;
    }

    public override bool isActionFinished()
    {
        return hasWeapon;
    }

    public override bool requiresInRange()
    {
        return true; // do we need to be near the target for the action?
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {

		if (weaponRack != null)
        {
            target = weaponRack;
			return true;
		}
        else return false;

    }

    public override bool perform(GameObject agent)
    {
        hasWeapon = true;
        
        if(guard != null)
        {
            Instantiate(guard.guardType.weaponPrefab, guard.weaponHolder.transform);
            guard.isArmed = true;
        }

		return hasWeapon;
    }

}