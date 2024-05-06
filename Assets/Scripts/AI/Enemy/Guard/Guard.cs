using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Guard : BaseGuard
{
    [HideInInspector]
    public GameObject target;

    public bool HideStationedPoint = false;
    [HideInInspector]
    public GameObject GuardingPoint;



    // Reference variables for the editor

    public float OuterVisionConeAngle => guardType.outerVisionConeAngle;
    public float OuterVisionConeRange => guardType.outerVisionConeRange;
    public Color OuterVisionConeColour => guardType.outerVisionConeColour;

    
    public float InnerVisionConeAngle => guardType.innerVisionConeAngle;
    public float InnerVisionConeRange => guardType.innerVisionConeRange;
    public Color InnerVisionConeColour => guardType.innerVisionConeColour;
    
    public float DetectionRange => guardType.detectionRange;
    public Color DetectionColour => guardType.rangeColour;

    //


    DebugAI debugger;

    private void Start()
    {
        GuardingPoint = new GameObject(gameObject.name + " StationedGuardPosition");
        GuardingPoint.transform.position = this.transform.position;
        GuardingPoint.transform.SetParent(null);
        DontDestroyOnLoad(GuardingPoint);
        if(HideStationedPoint) GuardingPoint.hideFlags = HideFlags.HideInHierarchy;
    }

    public override void Update()
    {
        base.Update();

        if(PlayerInProximityRadius()) visionSensing.PlayerInProximityRadius = true;
        else visionSensing.PlayerInProximityRadius = false;
        
        if(IsGateBlockingGuard() && !gate.isOpened)
        {
            guardOpenedGate = true;
            gate.Open();
        }

        if(IsGateBlockingGuard() && gate.isOpened)
        {
            guardOpenedGate = true;
        }
    }

    // Function to create goals for the guard 

    public override HashSet<KeyValuePair<string, object>> createGoalState()
    {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>> ();


        goal.Add(new KeyValuePair<string, object>("guardArea", true));
        goal.Add(new KeyValuePair<string, object>("survive", true));
        
		return goal;
    }

    public void MoveObject(Vector3 pos)
    {
        MoveToObject.transform.position = pos;
    }


    private bool PlayerInProximityRadius()
    {
        return Vector3.Distance(transform.position, player.transform.position) < DetectionRange ? true : false;
    }

    // Editor to draw vision cone 

    [CustomEditor(typeof(Guard))]
    public class EnemyAIEditor : Editor
    {
        public void OnSceneGUI()
        {
            var ai = target as Guard;

            if(ai.guardType == null) return;
            
            // Detection range
            Handles.color = ai.DetectionColour;
            Handles.DrawWireDisc(ai.transform.position, Vector3.up, ai.DetectionRange);

            // Vision cone
            Vector3 startPoint = Mathf.Cos(-ai.OuterVisionConeAngle * Mathf.Deg2Rad) * ai.transform.forward + Mathf.Sin(-ai.OuterVisionConeAngle * Mathf.Deg2Rad) * ai.transform.right;
            Handles.color = ai.OuterVisionConeColour;
            Handles.DrawSolidArc(ai.EyeLocation.position, Vector3.up, startPoint, ai.OuterVisionConeAngle * 2f, ai.OuterVisionConeRange);   

            // Inner Vision cone
            Vector3 innerStartPoint = Mathf.Cos(-ai.guardType.innerVisionConeAngle * Mathf.Deg2Rad) * ai.transform.forward + Mathf.Sin(-ai.guardType.innerVisionConeAngle * Mathf.Deg2Rad) * ai.transform.right;
            Handles.color = ai.guardType.innerVisionConeColour;
            Handles.DrawSolidArc(ai.EyeLocation.position, Vector3.up, innerStartPoint, ai.guardType.innerVisionConeAngle * 2f, ai.InnerVisionConeRange);        

        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, guardType.communicationRange);
    }

    public Vector2 RandomPosition()
    {
        return Random.insideUnitCircle * 5;
    }
    
    private bool IsGateBlockingGuard()
    {
        RaycastHit hit;

        if (Physics.Raycast(EyeLocation.transform.position, EyeLocation.transform.forward, out hit, 5f))
        {   
            bool isGate = hit.transform.gameObject.CompareTag("Gate");
            if(isGate)
            {
                return true;
            }

        }
        return false;

    } 

}
