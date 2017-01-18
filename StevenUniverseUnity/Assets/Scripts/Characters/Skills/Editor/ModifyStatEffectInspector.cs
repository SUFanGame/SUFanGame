using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using SUGame.SUGameEditor.CustomGUI;

[CustomEditor(typeof(ModifyStatEffect))]
public class ModifyStatEffectInspector : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        //CustomEditorGUI.DrawScriptField(serializedObject);

        //serializedObject.Update();

        //tarStatProp.intValue = EditorGUILayout.Popup("Target Stat", tarStatProp.intValue, primaryStatNames_);

        //EditorGUILayout.PropertyField(modValueProp, true);

        //serializedObject.ApplyModifiedProperties();
    }

}
