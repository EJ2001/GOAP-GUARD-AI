using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// AI manager to keep in scene which can be an alternative option to access the manager window through GUI
public class GuardAIManager : MonoBehaviour
{

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}

[CustomEditor(typeof(GuardAIManager))]
public class AIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GuardAIManager guard = (GuardAIManager)target;

        if(GUILayout.Button("Open Guard AI Manager"))
        {
            AIEditorWindow window = (AIEditorWindow) EditorWindow.GetWindow( typeof(AIEditorWindow), false, "Guard Manager" );
            window.Show();
        }
    }
}
