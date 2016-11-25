using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.World;
using System;

namespace StevenUniverse.FanGameEditor.SceneEditing
{
    public class BrushShapePanel : MapEditorPanel
    {
        BrushesPanel brushesPanel_;

        public override Rect Area_
        {
            get
            {
                var parentArea = brushesPanel_.Area_;
                int x = (int)parentArea.x;
                int y = (int)parentArea.yMax + 3;
                int w = 100;
                int h = 22;

                return new Rect(x, y, w, h);
            }
        }

        protected override string FoldoutTitle_
        {
            get
            {
                return "Shape";
            }
        }

        public BrushShapePanel(BrushesPanel brushesPanel) : base()
        {
            brushesPanel_ = brushesPanel;
        }


        protected override void OnRenderArea(Map map)
        {
        }
    }
}
