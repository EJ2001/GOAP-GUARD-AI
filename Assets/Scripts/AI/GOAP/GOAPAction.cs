
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public abstract class GOAPAction : MonoBehaviour
{
	
	private HashSet<KeyValuePair<string,object>> preconditions;
	private HashSet<KeyValuePair<string,object>> effects;

	private bool inRange = false;

	// Cost to perform an action 
	// Can be changed to make different actions more obvious for the planner to use
	public float cost = 1f;

	//Object for action to perform on
	[HideInInspector]
	public GameObject target;

    public bool finishedAction = false;
	
    public string ActionName;


	protected Animator animator;
	protected GameObject player;
	[HideInInspector]
	public Guard guard;
	protected ThreatLevelSensing threatLevelSensing;
	protected VisionSensing visionSensing;
	protected NavMeshAgent navAgent;

	public GOAPAction()
	{
		preconditions = new HashSet<KeyValuePair<string, object>> ();
		effects = new HashSet<KeyValuePair<string, object>> ();
	}

	public virtual void Awake()
	{
		guard = GetComponent<Guard>();
		navAgent = GetComponent<NavMeshAgent>();
		threatLevelSensing = GetComponent<ThreatLevelSensing>();
		animator = GetComponent<Animator>();
		player = GameObject.FindGameObjectWithTag("Player").gameObject;
		visionSensing = GetComponent<VisionSensing>();
	}

	public void ResetAction()
	{
		inRange = false;
		target = null;
		Reset ();
	}

	// Reset variables before planning happens again
	public abstract void Reset();

	public abstract bool isActionFinished();

	// Checks if the action can be run
	public abstract bool checkProceduralPrecondition(GameObject agent);

	// Returns true if the action has a successful outcome
	public abstract bool perform(GameObject agent);

	public abstract bool requiresInRange ();
	

	public bool isInRange ()
	{
		return inRange;
	}
	
	public void setInRange(bool inRange)
	{
		this.inRange = inRange;
	}


	public void addPrecondition(string key, object value)
	{
		preconditions.Add (new KeyValuePair<string, object>(key, value) );
	}


	public void removePrecondition(string key)
	{
		KeyValuePair<string, object> remove = default(KeyValuePair<string,object>);
		foreach (KeyValuePair<string, object> kvp in preconditions)
		{
			if (kvp.Key.Equals (key)) 
				remove = kvp;
		}
		if ( !default(KeyValuePair<string,object>).Equals(remove) )
			preconditions.Remove (remove);
	}


	public void addEffect(string key, object value)
	{
		effects.Add (new KeyValuePair<string, object>(key, value) );
	}


	public void removeEffect(string key)
	{
		KeyValuePair<string, object> remove = default(KeyValuePair<string,object>);
		foreach (KeyValuePair<string, object> kvp in effects)
		{
			if (kvp.Key.Equals (key)) 
				remove = kvp;
		}
		if ( !default(KeyValuePair<string,object>).Equals(remove) )
			effects.Remove (remove);
	}

	
	public HashSet<KeyValuePair<string, object>> GetPreConditions {
		get {
			return preconditions;
		}
	}

	public HashSet<KeyValuePair<string, object>> GetEffects {
		get {
			return effects;
		}
	}
}