using System.Collections;
using System.Collections.Generic;
using Booble.Interactables.Dialogues;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Dialogue))]
public class DialogueEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Break Apart"))
        {
            ((Dialogue)target).BreakApart();
        }
    }
}
