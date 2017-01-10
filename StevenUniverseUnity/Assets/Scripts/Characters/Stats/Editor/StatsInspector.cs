using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using SUGame.Characters.Customization;

//[CustomEditor(typeof(Stats))]
public class StatsInspector : Editor
{
    SerializedProperty statsArrayProp_;
    Stats tar_;

    void OnEnable()
    {
        statsArrayProp_ = serializedObject.FindProperty("stats_");
        tar_ = target as Stats;
    }

    // Note : Most of the work related to this is done in Stats.OnValidate
    public override void OnInspectorGUI()
    {
        //if( DrawDefaultInspector() )
        //{
        //    tar_.OnValidate();
        //    //EditorUtility.SetDirty(tar_);
        //}

        //foreach( var stat in tar_.Stats_)
        //{
        //    EditorGUI.BeginChangeCheck();

        //    //var newStat = EditorGUI.

        //    if (EditorGUI.EndChangeCheck() )
        //    {

        //    }
        //}

        serializedObject.Update();

        for (int i = 0; i < statsArrayProp_.arraySize; ++i)
        {
            var statProp = statsArrayProp_.GetArrayElementAtIndex(i);
            var type = (Stat.Type)i;
            string statName = Stat.GetProperName_(type);

            EditorGUILayout.PropertyField(statProp, new GUIContent(statName) );
        }


        //serializedObject.Update();

        //for( int i = 0; i < Stat.TypeCount_; ++i )
        //{
        //    var statProp = statsArrayProp_.GetArrayElementAtIndex(i);
        //    var type = (Stat.Type)i;
        //    string statName = Stat.GetProperName_(type);
        //    var currProp = statProp.FindPropertyRelative("current_");

        //    if (type == Stat.Type.XP || type == Stat.Type.DEF )
        //        continue;

        //    EditorGUILayout.LabelField(statName);

        //    EditorGUI.indentLevel++;
        //    //if( type == Stat.Type.HP )
        //    //{
        //    //    var vitMultProp = serializedObject.FindProperty("vitalityMultiplier_");
        //    //    EditorGUILayout.PropertyField(vitMultProp);
        //    //    CurrPropLabel(currProp);
        //    //}
        //    //else
        //    {
        //        var maxProp = statProp.FindPropertyRelative("max_");
        //        var curveProp = statProp.FindPropertyRelative("curve_");
        //        var baseProp = statProp.FindPropertyRelative("base_");

        //        // Draw our property fields.
        //        EditorGUILayout.PropertyField(baseProp);
        //        EditorGUILayout.PropertyField(maxProp);
        //        EditorGUILayout.PropertyField(curveProp);

        //        CurrPropLabel(currProp);
        //    }

        //    EditorGUI.indentLevel--;
        //}

        serializedObject.ApplyModifiedProperties();


    }

    int GetPropValue( Stat.Type type )
    {
        return statsArrayProp_.GetArrayElementAtIndex((int)type).FindPropertyRelative("current_").intValue;
    }

    void CurrPropLabel( SerializedProperty currProp )
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Current");
        EditorGUILayout.LabelField(currProp.intValue.ToString());
        EditorGUILayout.EndHorizontal();
    }

}