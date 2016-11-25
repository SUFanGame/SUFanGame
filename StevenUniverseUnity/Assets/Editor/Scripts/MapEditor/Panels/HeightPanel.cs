using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.World;
using UnityEditor;
using StevenUniverse.FanGameEditor.SceneEditing;
using StevenUniverse.FanGame.Util;

public class HeightPanel : MapEditorPanel
{

    public override Rect Area_
    {
        get
        {
            int w = 120;
            int h = Foldout_ ? 16 + 20 * 2 : 22;
            int x = Screen.width - w - 15;
            int y = Screen.height - h - 55;

            return new Rect(x, y, w, h);
        }
    }

    protected override string FoldoutTitle_
    {
        get
        {
            if (Foldout_)
                return "Elevation";
            return string.Format("Elevation [{0}]", MapEditor.CursorHeight_);
        }
    }


    public override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void OnRenderArea(Map map)
    {

        var oldWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 60f;

        MapEditor.CursorHeight_ = EditorGUILayout.IntField("Elevation", MapEditor.CursorHeight_);

        var newType = (CutoffType)EditorGUILayout.EnumPopup("Cutoff", map.cutoffType_);

        if( newType != map.cutoffType_ )
        {
            map.cutoffType_ = newType;
            //map.OnCutoffHeightChanged();
        }

        EditorGUIUtility.labelWidth = oldWidth;
    }


}
