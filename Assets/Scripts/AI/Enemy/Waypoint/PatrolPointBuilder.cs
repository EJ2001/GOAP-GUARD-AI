using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPointBuilder : MonoBehaviour
{
    public Color patrolRouteColor = Color.red;

    public List<GameObject> patrolPoints = new List<GameObject>();

    GameObject patrolPoint;
    private Guard guard;

    private int count = 0;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void BuildPatrolRoute()
    {
        count++;
        patrolPoint = new GameObject ("PatrolPoint " + count);
        patrolPoint.transform.parent = this.transform;
        patrolPoint.AddComponent<Point>();
        patrolPoints.Add(patrolPoint);
    }

    public void ClearRoute()
    {
        foreach(GameObject obj in patrolPoints)
        {
            DestroyImmediate(obj);
        }

        patrolPoints.Clear();
    }

    public List<GameObject> CreatedRoute
    {
        get { return patrolPoints; }
    }

    void OnDrawGizmos()
    {
        for (var i = 1; i < patrolPoints.Count; i++)
        {
            Gizmos.color = patrolRouteColor;
            Gizmos.DrawLine (patrolPoints[i - 1].transform.position, patrolPoints[i].transform.position);
        }
    }
    
}
