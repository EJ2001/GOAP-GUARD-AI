using UnityEngine;
using UnityEditor;

// Class to draw when creating a new guard which saves data entry

public class CreateGuardWindow : EditorWindow
{
    private SerializedObject serializedObject;
    private SerializedProperty serializedProperty;

    protected GuardData[] guards;
    public GuardData newGuard;

    private void OnGUI()
    {

        serializedObject = new SerializedObject(newGuard);
        serializedProperty = serializedObject.GetIterator();
        serializedProperty.NextVisible(true);
        DrawProperties(serializedProperty);

        if (GUILayout.Button("save"))
        {
            guards = GetAllInstances<GuardData>();
            if (newGuard.guardType == null)
            {
                newGuard.guardType = "guard" + (guards.Length + 1);
            }

            AssetDatabase.CreateAsset(newGuard, "Assets/Scripts/AI/GuardData/" + newGuard.guardType + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Close();
        }

        Apply();
    }

    protected void DrawProperties(SerializedProperty p)
    {

        while (p.NextVisible(false))
        {
            EditorGUILayout.PropertyField(p, true);

        }
    }


    // Retrieve guard datas
    
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

    protected void Apply()
    {
        serializedObject.ApplyModifiedProperties();
    }
}