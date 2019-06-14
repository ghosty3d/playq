using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ArrowEffectsController))]
public class ArrowEffectsControllerEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        var controller = (ArrowEffectsController) target;
        
        EditorGUILayout.Space();

        if (GUILayout.Button("Play Effect", GUILayout.ExpandWidth(true), GUILayout.Height(32))) {
            controller.playEffect();
        }
        
        if (GUILayout.Button("Reset Effect", GUILayout.ExpandWidth(true), GUILayout.Height(32))) {
            controller.resetEffect();
        }
    }
}