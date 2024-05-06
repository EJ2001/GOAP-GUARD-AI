using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PatrolPointBuilder))]
public class PatrolPointEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PatrolPointBuilder patrolBuilder = (PatrolPointBuilder)target;

        if(GUILayout.Button("Create Patrol Point"))
        {
            patrolBuilder.BuildPatrolRoute();
        }

        GUILayout.Space(10);

        if(GUILayout.Button("Clear Path"))
        {
            patrolBuilder.ClearRoute();
        }
        
    }
}
