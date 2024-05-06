using UnityEngine;
using UnityEditor;


// Class for drawing serialized properties from a scriptable object to a window

public class AIEditorWindow : EditorWindow
{

    protected SerializedObject serializedObject;
    protected SerializedProperty serializedProperty;

    protected GuardData[] guards;
    protected string selectedPropertyPach;
    protected string selectedProperty;


    [MenuItem("Window/AIManager/Guard Manager")]
    protected static void ShowWindow()
    {
        GetWindow<AIEditorWindow>("Guard Manager");
    }

    private void OnGUI()
    {
        guards = GetAllInstances<GuardData>();
        serializedObject = new SerializedObject(guards[0]);
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(130), GUILayout.ExpandHeight(true));

        DrawSliderBar(guards);

        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));

        if (selectedProperty != null)
        {
            for (int i = 0; i < guards.Length; i++)
            {
                if (guards[i].guardType == selectedProperty)
                {
                    serializedObject = new SerializedObject(guards[i]);
                    serializedProperty = serializedObject.GetIterator();
                    serializedProperty.NextVisible(true);
                    DrawProperties(serializedProperty);

                    // If there is no guard prefab grab the guard base prefab in the resources folder
                    if(guards[i].prefab == null)
                    {
                        GameObject guard = Resources.Load("Prefabs/BaseGuardPrefab") as GameObject;
                        if(guard == null) Debug.Log("<color=red>BaseGuardPrefab must be in Resources/Prefabs folder</color>");
                        guards[i].prefab = guard;
                    } 
                    
                    // Button to instantiate the guard
                    if (GUILayout.Button("Spawn Guard"))
                    {
                        GameObject guard = Instantiate(guards[i].prefab) as GameObject;
                        guard.name = guards[i].guardType;
                        guard.GetComponent<Guard>().guardType = guards[i];
      
                        
                    }

                }
            }
        }
        else
        {
            EditorGUILayout.LabelField("select a guard from the list");
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();



        Apply();
    }

    public static T[] GetAllInstances<T>() where T : GuardData
    {
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
        T[] a = new T[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
        }

        return a;

    }

    protected void DrawProperties(SerializedProperty p)
    {
        while (p.NextVisible(false))
        {
            EditorGUILayout.PropertyField(p, true);
        }
    }

    protected void DrawSliderBar(GuardData[] prop)
    {
        foreach (GuardData p in prop)
        {
            if (GUILayout.Button(p.guardType))
            {
                selectedPropertyPach = p.guardType;
            }
        }

        if (!string.IsNullOrEmpty(selectedPropertyPach))
        {
            selectedProperty = selectedPropertyPach;
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Create new guard"))
        {
            GuardData newGuard = GuardData.CreateInstance<GuardData>();
            CreateGuardWindow newGuardWindow = GetWindow<CreateGuardWindow>("New Guard");
            newGuardWindow.newGuard = newGuard;

        }
    }

    protected void Apply()
    {
        serializedObject.ApplyModifiedProperties();
    }

}