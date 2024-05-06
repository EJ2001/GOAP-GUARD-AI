using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FindMedicAction : GOAPAction
{
    private bool findingMedic = false;

    [SerializeField]
    private GameObject medicArea;

    public FindMedicAction()
    {
        addPrecondition("injured", true);   
        addPrecondition("isGateOpen", true);
        addPrecondition("guardHasOpenedGate", true);

        addEffect("survive", true);
        addEffect("injured", false);

        cost = 1f;
    }
        
    public override void Awake()
    {
        base.Awake();
        medicArea = GameObject.Find("MedicArea");
    }

    public override void Reset()
    {
        findingMedic = false;
        target = null;
    }

    public override bool isActionFinished()
    {
        return findingMedic;
    }

    public override bool requiresInRange()
    {
        return true; // do we need to be near the target for the action?
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {

		if (medicArea != null)
        {
            cost = guard.currentHealth / 100;
            target = medicArea;
			return true;
		}
        else return false;

    }

    public override bool perform(GameObject agent)
    {
        findingMedic = true;
        guard.currentHealth = guard.guardType.maxHealth;

		return findingMedic;
    }

}