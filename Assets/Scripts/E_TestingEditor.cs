using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(S_Testing))]
public class E_TestingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        S_Testing testing = (S_Testing)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("PopulateTable"))
        {
            testing.PopulateTable();
        }
    }
}
