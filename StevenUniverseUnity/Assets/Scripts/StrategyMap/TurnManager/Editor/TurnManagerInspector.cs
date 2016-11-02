using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace StevenUniverse.FanGame.StrategyMap
{
    [CustomEditor(typeof(TurnManager))]
    public class TurnManagerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var tar = target as TurnManager;

            if (tar.ActingPlayer_ != null)
            {
                EditorUtility.SetDirty(target);
                EditorGUILayout.LabelField("Active Player: " + tar.ActingPlayer_.name);
            }
        }
    }
}
