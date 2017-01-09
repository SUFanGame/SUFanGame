using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;


[CustomPropertyDrawer(typeof(Stat))]
public class StatPropertyDrawer : PropertyDrawer
{
    int indent = 15;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.height = EditorGUIUtility.singleLineHeight;

        property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
        //EditorGUI.LabelField(position, label);

        if (!property.isExpanded)
            return;

        int oldIndentLevel = EditorGUI.indentLevel;
        
        //EditorGUI.indentLevel = 0;
        
        position.y += EditorGUIUtility.singleLineHeight;

        var rangePos = DoPrefix(position, "Range", .5f);

        var minLevelValue = property.FindPropertyRelative("minLevelValue_");
        var maxLevelValue = property.FindPropertyRelative("maxLevelValue_");
        
        rangePos = DrawProp(rangePos, label, minLevelValue, maxLevelValue.intValue, "Min" );
        DrawProp(rangePos, label, maxLevelValue, int.MaxValue, "Max" );

        position.y += EditorGUIUtility.singleLineHeight + 2;

        var valuePos = DoPrefix(position, "Value", .5f);

        var currValue = property.FindPropertyRelative("current_");
        var maxValue = property.FindPropertyRelative("max_");

        valuePos = DrawProp(valuePos, label, currValue, maxValue.intValue, "Curr");
        DrawProp(valuePos, label, maxValue, int.MaxValue, "Max");

        position.y += EditorGUIUtility.singleLineHeight + 2;

        var curvePos = DoPrefix(position, "Curve", 1f);
        EditorGUI.PropertyField(curvePos, property.FindPropertyRelative("levellingCurve_"), GUIContent.none);

        EditorGUIUtility.labelWidth = 0;
        EditorGUI.indentLevel = oldIndentLevel;
    }

    Rect DoPrefix(Rect position, string label, float widthScale )
    {

        position.x += indent;
        position = EditorGUI.PrefixLabel(position, new GUIContent(label));
        position.x -= indent;
        position.width *= widthScale;
        return position;
    }

    /// <summary>
    /// Draw one of the stat's property fields.
    /// </summary>
    /// <param name="position">Position to draw the field.</param>
    /// <param name="label">The origin property label.</param>
    /// <param name="prop">The serialized property to draw.</param>
    /// <param name="clampMax">Max value to clamp to.</param>
    /// <param name="fieldLabel">The actual label that will be displayed.</param>
    /// <returns></returns>
    Rect DrawProp( Rect position, GUIContent label, SerializedProperty prop, int clampMax, string fieldLabel )
    {
        var size = GUI.skin.label.CalcSize(new GUIContent(fieldLabel)).x;
        EditorGUIUtility.labelWidth = size + (EditorGUI.indentLevel * 18);
        EditorGUI.BeginProperty(position, label, prop);

        int newValue = EditorGUI.IntField(position, fieldLabel, prop.intValue);
        prop.intValue = Mathf.Clamp(newValue, 0, clampMax);

        EditorGUI.EndProperty();
        EditorGUIUtility.labelWidth = 0;
        position.x += position.width;
        return position;
    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        bool foldout = property.isExpanded;
        float lineHeight = EditorGUIUtility.singleLineHeight + 2;
        return foldout ? 4 * lineHeight : lineHeight;
    }
}


////[CustomPropertyDrawer(typeof(Stat))]
//public class StatPropertyDrawer : PropertyDrawer 
//{
//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        var oldLabelWidth = EditorGUIUtility.labelWidth;

//        EditorGUIUtility.labelWidth = 40f;
//        //base.OnGUI(position, property, label);

//        //Stat s = (Stat)fieldInfo.GetValue(property.serializedObject.targetObject);

//        //position.width = 90;


//        ///EditorGUI.LabelField(position, s.Type_.ToString() );
//        /// 
//        EditorGUI.LabelField(position, property.propertyPath);

//        position.y += 18f;

//        EditorGUI.LabelField(position, "FieldType: " + fieldInfo.FieldType.ToString());

//        position.y += 18f;

//        EditorGUI.LabelField(position, "PropertyPath: " + property.propertyPath );

        

//        position.y += 18f;
//        if ( fieldInfo.FieldType != typeof(Stat) && property.propertyPath.Contains("[") )
//        {
//            int index = int.Parse( property.propertyPath.Split('[', ']')[1] );
//            var arrField = fieldInfo.GetValue(property.serializedObject.targetObject);
//            if( arrField != null )
//            {
//                var collection = arrField as IList<Stat>;

//                if( collection != null  )
//                {
//                    EditorGUI.LabelField(position, collection[index].GetType().ToString() );
//                }




//                position.y += 18f;

//                //var arrVal = fieldInfo.GetValue(arrField);

                
//            }

            
//        }



//        //s.Current_ = EditorGUI.IntField(position, "Curr", s.Current_);

//        //fieldInfo.SetValue(property.serializedObject.targetObject, s );



//        /*
//        var minProp = property.FindPropertyRelative("min_");
//        var maxProp = property.FindPropertyRelative("max_");
//        var currProp = property.FindPropertyRelative("current_");
//        var type = property.FindPropertyRelative("type_");

//        string properName = Stat.ProperName((Stat.Type)type.enumValueIndex);

//        EditorGUI.LabelField(position, properName );

//        EditorGUI.indentLevel++;
//        position.y += 18f;
//        position.height = 16f;
//        position.width = 85;

//        EditorGUIUtility.labelWidth = 40f;

//        EditorGUI.BeginProperty( position, label, property );

//        EditorGUI.PropertyField( position, minProp, new GUIContent("Min"));

//        EditorGUI.EndProperty();

//        EditorGUI.indentLevel--;
//        EditorGUIUtility.labelWidth = oldLabelWidth;
//        */
//    }

//    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//    {
//        return 18f * 4;
//       // return base.GetPropertyHeight(property, label);
//    }
//}
