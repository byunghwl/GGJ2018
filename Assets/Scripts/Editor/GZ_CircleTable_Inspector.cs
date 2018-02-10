using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GZ_CircleTable))]
public class GZ_CircleTable_Inspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var myTarget = (GZ_CircleTable)target;

        if (GUILayout.Button("Spawn"))
        {
            myTarget.SpawnTable(3,3);
        }
    }
}
