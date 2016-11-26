using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.World;
using UnityEditor;
using StevenUniverse.FanGameEditor.SceneEditing;
using StevenUniverse.FanGame.Util;

public class HeightPanel : MapEditorPanel
{
    float cutoffAlpha_ = .45f;
    public float CutoffAlpha_ { get { return cutoffAlpha_; } }

    const string PREFS_CUTOFFALPHA_NAME = "ME_HEIGHTPANEL_CUTOFFALPHA";

    public override Rect Area_
    {
        get
        {
            int w = 120;
            int h = Foldout_ ? 16 + 20 * 3 : 22;
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
        EditorPrefs.SetFloat( PREFS_CUTOFFALPHA_NAME, cutoffAlpha_);
    }

    public HeightPanel() : base()
    {
        cutoffAlpha_ = EditorPrefs.GetFloat(PREFS_CUTOFFALPHA_NAME, .65f);
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
            map.SetCutoffAlphaOnChunks((byte)(cutoffAlpha_ * 255f));
        }

        float newAlpha = EditorGUILayout.Slider("CutoffAlpha", cutoffAlpha_, 0, 1f);

        newAlpha = Mathf.Clamp01(newAlpha);

        if( newAlpha != cutoffAlpha_ )
        {
            //Debug.Log("NewALpha:" + ((byte)(newAlpha * 255f)).ToString()   );
            if ( map.cutoffType_ != CutoffType.NONE )
            {
                map.SetCutoffAlphaOnChunks((byte)(newAlpha * 255f));
                EditorUtility.SetDirty(map.gameObject);
            }
        }

        cutoffAlpha_ = newAlpha;

        EditorGUIUtility.labelWidth = oldWidth;
    }


}
