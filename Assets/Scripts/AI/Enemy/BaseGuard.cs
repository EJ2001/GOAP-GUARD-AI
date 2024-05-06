using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Base class for guards

[RequireComponent(typeof(VisionSensing))]
[RequireComponent(typeof(ThreatLevelSensing))]
[RequireComponent(typeof(GOAPAgent))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(DebugAI))]
[RequireComponent(typeof(Animator))]
public abstract class BaseGuard : MonoBehaviour, IGOAP
{
    public GuardData guardType;

    [Header("Vitals:")]
    public float currentHealth;

    [Header("Core:")]
    public float hunger;
    public float hungerModifier = 0;
    public float stamina;
    public float staminaModifier = 0;



    [Header("Combat:")]
    public GameObject weaponHolder;
    protected GameObject player;
    public Transform EyeLocation;
    protected VisionSensing visionSensing;
    private List<BaseGuard> guards = new List<BaseGuard>();
    [HideInInspector] public bool isArmed = false;
    public bool leftGuardPosition = false;

    public enum MovementState
    {
        idle,
        walking,
        running,
    } 
    [Header("Movement:")]
    public MovementState movementState;
    public float minDistToReachTarget = 3f;
    public GameObject MoveToObject;
    private GameObject moveToObjectHolder;


    protected Gate gate;

    [HideInInspector]
    public bool guardOpenedGate = false;
    public bool IsOnPatrol = false;


    private NavMeshAgent agent;
    private DebugAI debugAI;
 
    public GameObject waypointManager;
    [HideInInspector]
    public PatrolPointBuilder patrolPointBuilder;
    private ThreatLevelSensing threatLevelSensing;
    private Canvas canvas = null;



    private void Start()
    {
        movementState = MovementState.idle;
    }

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").gameObject;
        guards.AddRange(GameObject.FindObjectsOfType<BaseGuard>());
        guards.Remove(this);

        currentHealth = guardType.maxHealth;   
        hunger = guardType.maxHunger;
        stamina = guardType.maxStamina;
        hunger -= Random.Range(0, 30);
        stamina -= Random.Range(0, 20);

        visionSensing = GetComponent<VisionSensing>();
        threatLevelSensing = GetComponent<ThreatLevelSensing>();
        agent = GetComponent<NavMeshAgent>();
        debugAI = GetComponent<DebugAI>();
        
        moveToObjectHolder = GameObject.Find("GuardAIManager").transform.Find("MoveToObjectsHolder").gameObject;
        MoveToObject = new GameObject(this.gameObject.name + " MoveToObject");
        MoveToObject.transform.SetParent(moveToObjectHolder.transform);

        if(!waypointManager) waypointManager = GameObject.Find(gameObject.name + " - WaypointManager");
        if(waypointManager) patrolPointBuilder = waypointManager.GetComponent<PatrolPointBuilder>();
        canvas = transform.Find("Canvas").GetComponent<Canvas>();
        if(!guardType.showDebugUI && canvas != null) canvas.gameObject.SetActive(false); 
        if(weaponHolder.transform.childCount > 0) isArmed = true;
        gate = GameObject.Find("CastleGate").GetComponent<Gate>();

    }

    public virtual void Update()
    {
        HasWeapon();

        ReduceCoreStats();
    }

    // Goap Methods
    // ---------------------------------------------------------------------------------------

    public HashSet<KeyValuePair<string, object>> getWorldState()
    {
        //All of the possible world states for the guards

        HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();

        worldData.Add(new KeyValuePair<string, object>("canPatrolArea", patrolPointBuilder != null));
        worldData.Add(new KeyValuePair<string, object>("shouldPatrolArea", IsOnPatrol));
        worldData.Add(new KeyValuePair<string, object>("visionOnPlayer", visionSensing.HasVisionOnPlayer()));
        worldData.Add(new KeyValuePair<string, object>("injured", (currentHealth <= 50)));
        worldData.Add(new KeyValuePair<string, object>("hasMemoryOnPlayerPosition", (visionSensing.lastSeenPosition != Vector3.zero)));
        worldData.Add(new KeyValuePair<string, object>("isAlarmed", (threatLevelSensing.suspicionState == ThreatLevelSensing.SuspicionState.Alarmed)));
        worldData.Add(new KeyValuePair<string, object>("hasSuspicion", (threatLevelSensing.suspicionState == ThreatLevelSensing.SuspicionState.Suspicious)));
        worldData.Add(new KeyValuePair<string, object>("isThreatened", (threatLevelSensing.suspicionState == ThreatLevelSensing.SuspicionState.Threatened)));
        worldData.Add(new KeyValuePair<string, object>("unAware", (threatLevelSensing.suspicionState == ThreatLevelSensing.SuspicionState.Unaware)));
        worldData.Add(new KeyValuePair<string, object>("isArmed", (isArmed)));
        worldData.Add(new KeyValuePair<string, object>("otherGuardNeedsHelp", (visionSensing.guardNeedsHelp)));
        worldData.Add(new KeyValuePair<string, object>("isGateOpen", (gate.isOpened)));
        worldData.Add(new KeyValuePair<string, object>("guardHasOpenedGate", (guardOpenedGate == true)));
        worldData.Add(new KeyValuePair<string, object>("isHungry", (hunger <= 60)));
        worldData.Add(new KeyValuePair<string, object>("otherGuardsGuarding", (IsAnotherGuardGuarding())));
        worldData.Add(new KeyValuePair<string, object>("leftGuardingPosition", (leftGuardPosition)));
        worldData.Add(new KeyValuePair<string, object>("isTired", (stamina <= 40)));
   
        return worldData;
    }

    public abstract HashSet<KeyValuePair<string, object>> createGoalState();


    public void planFailed(HashSet<KeyValuePair<string, object>> failedGoal)
    {
    }

    public void planFound(HashSet<KeyValuePair<string, object>> goal, Queue<GOAPAction> actions)
    {
        Debug.Log(this.gameObject.name + "<color=blue> - Plan found</color> " + GOAPAgent.DebugMessage(actions));
    }

    public void actionsFinished()
    {
        Debug.Log("<color=green>Actions completed</color>");
    }

    public void planAborted(GOAPAction failedAction)
    {
        GetComponent<GOAPAgent>().GetDataProviderInterface().actionsFinished();
        failedAction.Reset();
        failedAction.ResetAction();

        Debug.Log("<color=red>Plan Aborted</color> " + GOAPAgent.DebugMessage(failedAction));
    }


    // Move agent to next action's target
    public bool moveAgent(GOAPAction nextAction)
    {
        debugAI.UpdateActionText(nextAction.ActionName);
        debugAI.UpdateThreatLevelText(threatLevelSensing.suspicionState.ToString());

        agent.isStopped = false;
        agent.SetDestination(nextAction.target.transform.position);

        float dist = Vector3.Distance( nextAction.target.transform.position, transform.position);

        //If arrived
        if (dist <= minDistToReachTarget)
        {
            nextAction.setInRange(true);
            return true;
        }
        else
        {
            return false;
        }
    }
    
    // Other
    // ---------------------------------------------------------------------------------------
    private void HasWeapon()
    {
        if(weaponHolder.transform.childCount <= 0) isArmed = false;
        else isArmed = true;
    }

    private void ReduceCoreStats()
    {
        hunger -= Time.deltaTime * hungerModifier;
        stamina -= Time.deltaTime * staminaModifier;

        switch (movementState)
        {
            case MovementState.idle:
                hungerModifier = 0.85f;
                staminaModifier = 0.5f;
                break;
            case MovementState.walking:
                hungerModifier = 1f;
                staminaModifier = 2f;
                break;
            case MovementState.running:
                hungerModifier = 4f;
                staminaModifier = 6f;
                break;
        }
    }


    private bool IsAnotherGuardGuarding()
    {
        foreach(BaseGuard guard in guards)
        {
            if(guard.leftGuardPosition)
            {
                return false;
            }
            else 
            {
                return true;
            }
        }
        return false;
    }


    private bool IsAlone()
    {
        foreach(BaseGuard guard in guards)
        {
            if(Vector3.Distance(transform.position, guard.transform.position) > 8)
            {
                return true;
            }
            else return false;
        }
        return false;
    }
   


}
