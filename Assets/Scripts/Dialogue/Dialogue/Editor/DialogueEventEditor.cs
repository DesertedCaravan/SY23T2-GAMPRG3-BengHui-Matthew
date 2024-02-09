using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogueEvent))]
public class DialogueEventEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DialogueEvent responseEvents = (DialogueEvent)target;

        if (GUILayout.Button("Refresh"))
        {
            responseEvents.OnValidate();
        }
    }
}
