using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.World;
using System;
using StevenUniverse.FanGame.Util;
using UnityEditor;
using System.Linq;

namespace StevenUniverse.FanGameEditor.SceneEditing
{
    //public class BrushShapePanel : MapEditorPanel
    //{
    //    BrushesPanel brushesPanel_;

    //    MapEditorBrush.CursorShapeCallback[] shapes_;

    //    string[] funcNames_;

    //    int shapeIndex_;

    //    public MapEditorBrush.CursorShapeCallback CurrentShape_ { get { return shapes_[shapeIndex_]; } }

    //    public override Rect Area_
    //    {
    //        get
    //        {
    //            var parentArea = brushesPanel_.Area_;
    //            int x = (int)parentArea.x;
    //            int y = (int)parentArea.yMax + 3;
    //            int w = 100;
    //            int h = 22;

    //            return new Rect(x, y, w, h);
    //        }
    //    }

    //    protected override string FoldoutTitle_
    //    {
    //        get
    //        {
    //            return "Shape";
    //        }
    //    }

    //    const string PREFS_BRUSHSHAPEINDEX_NAME = "MEShapePanelIndex";

    //    public BrushShapePanel(BrushesPanel brushesPanel) : base()
    //    {
    //        brushesPanel_ = brushesPanel;

    //        shapeIndex_ = EditorPrefs.GetInt(PREFS_BRUSHSHAPEINDEX_NAME, 0);

    //        //shapes_ = new MapEditorBrush.CursorShapeCallback[]
    //        //{
    //        //    Square,
    //        //    Circle
    //        //};

    //        funcNames_ = shapes_.Select(f => f.Method.Name).ToArray();
    //    }

    //    public override void OnDisable()
    //    {
    //        base.OnDisable();

    //        EditorPrefs.SetInt(PREFS_BRUSHSHAPEINDEX_NAME, shapeIndex_);
    //    }


    //    protected override void OnPanelGUI(Map map)
    //    {
    //        shapeIndex_ = EditorGUILayout.Popup(shapeIndex_, funcNames_);
    //    }
        

    //}
}
