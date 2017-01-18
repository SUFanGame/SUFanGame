using UnityEngine;
using System.Collections;
using UnityEditor;
using SUGame.PropertyAttributes;


// By Bunny83: http://answers.unity3d.com/answers/394174/view.html

namespace SUGame.PropertyAttributes.SUGameEditor
{
    [CustomPropertyDrawer(typeof(BitMaskAttribute))]
    public class EnumBitMaskPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            // var typeAttr = attribute as BitMaskAttribute;
            // Add the actual int value behind the field name
            label.text = label.text + "(" + prop.intValue + ")";
            // Modified from original, get type directly from fieldInfo, new in Unity 4
            prop.intValue = DrawBitMaskField(position, prop.intValue, fieldInfo.FieldType, label);
        }

        int DrawBitMaskField(Rect aPosition, int aMask, System.Type aType, GUIContent aLabel)
        {
            var itemNames = System.Enum.GetNames(aType);
            // Note the cast to int. If the underlying type of the enum is NOT an int, this will fail
            var itemValues = System.Enum.GetValues(aType) as int[];

            int val = aMask;
            int maskVal = 0;
            for (int i = 0; i < itemValues.Length; i++)
            {
                if (itemValues[i] != 0)
                {
                    if ((val & itemValues[i]) == itemValues[i])
                        maskVal |= 1 << i;
                }
                else if (val == 0)
                    maskVal |= 1 << i;
            }
            int newMaskVal = EditorGUI.MaskField(aPosition, aLabel, maskVal, itemNames);
            int changes = maskVal ^ newMaskVal;

            for (int i = 0; i < itemValues.Length; i++)
            {
                if ((changes & (1 << i)) != 0)            // has this list item changed?
                {
                    if ((newMaskVal & (1 << i)) != 0)     // has it been set?
                    {
                        if (itemValues[i] == 0)           // special case: if "0" is set, just set the val to 0
                        {
                            val = 0;
                            break;
                        }
                        else
                            val |= itemValues[i];
                    }
                    else                                  // it has been reset
                    {
                        val &= ~itemValues[i];
                    }
                }
            }
            return val;
        }
    }
}