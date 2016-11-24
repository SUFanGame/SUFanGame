using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.World;
using UnityEditor;
using StevenUniverse.FanGameEditor.SceneEditing;
using StevenUniverse.FanGame.Util;

public class HeightPanel : MapEditorPanel
{
    bool foldOut_ = true;

    const string PREFS_HEIGHTPANELFOLDOUT_NAME = "MEHeightPanelFoldout";
    //const string PREFS_HEIGHTPANELCUTOFFTYPE_NAME = "MEHeightPanelCutoffType";

    public override Rect Area_
    {
        get
        {
            int w = 120;
            int h = foldOut_ ? 16 + 20 * 2 : 22;
            int x = Screen.width - w - 15;
            int y = Screen.height - h - 55;

            return new Rect(x, y, w, h);
        }
    }

    public HeightPanel() : base()
    {
        foldOut_ = EditorPrefs.GetBool(PREFS_HEIGHTPANELFOLDOUT_NAME, true);
        //cutoffType_ = (CutoffType)EditorPrefs.GetInt(PREFS_HEIGHTPANELCUTOFFTYPE_NAME, 0);
        EditorGUI.indentLevel++;


        EditorGUI.indentLevel--;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        EditorPrefs.SetBool(PREFS_HEIGHTPANELFOLDOUT_NAME, foldOut_);
        //EditorPrefs.SetInt(PREFS_HEIGHTPANELCUTOFFTYPE_NAME, (int)cutoffType_);
    }

    protected override void OnRenderArea(Map map)
    {

        var oldWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 60f;

        foldOut_ = EditorGUILayout.Foldout(foldOut_, "Height");

        MapEditor.CursorHeight_ = EditorGUILayout.IntField("Elevation", MapEditor.CursorHeight_);

        var newType = (CutoffType)EditorGUILayout.EnumPopup("Cutoff", map.cutoffType_);

        if( newType != map.cutoffType_ )
        {
            map.cutoffType_ = newType;
            map.OnCutoffHeightChanged();
        }

        EditorGUIUtility.labelWidth = oldWidth;
    }


}
