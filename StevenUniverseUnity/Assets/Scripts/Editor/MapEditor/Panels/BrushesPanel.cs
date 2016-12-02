using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using SUGame.World;

namespace SUGame.SUGameEditor.MapEditing.Panels
{
    public class BrushesPanel : MapEditorPanel
    {
        List<MapEditorBrush> brushes_;
        Texture2D[] brushIcons_;

        int selectedBrush_ = 0;
        public MapEditorBrush SelectedBrush_ { get { return brushes_[selectedBrush_]; } }

        const string PREFS_BRUSHINDEX_NAME = "MEBrushIndex";
        const string PREFS_BRUSHFOLDOUT_NAME = "MeBrushFoldout";

        static int brushColumns_ = 2;

        public int BrushCount_ { get { return brushes_.Count; } }

        public override Rect Area_
        {
            get
            {
                int x = 15;
                int y = 15;

                int brushWidth = 24 + brushColumns_ * 32 + brushColumns_ * 4;

                int foldoutLabelSize = (int)GUI.skin.label.CalcSize(new GUIContent(FoldoutTitle_)).x + 20;


                int w = Foldout_ ? (int)Mathf.Max( foldoutLabelSize, brushWidth ) : foldoutLabelSize;
                int h = Foldout_ ? 65 : 22;
                return new Rect(x, y, w, h);
            }
        }

        protected override string FoldoutTitle_
        {
            get
            {
                return "Brushes => Current: " + SelectedBrush_.Name_;
            }
        }


        public BrushesPanel( List<MapEditorBrush> brushes ) : base()
        {
            brushes_ = brushes;
            brushIcons_ = brushes.Select(b => b.BrushIconTexture_).ToArray();
            selectedBrush_ = EditorPrefs.GetInt( PREFS_BRUSHINDEX_NAME, 0);
            //Debug.LogFormat("Loading brushes panel, SelectedBrush: {0}", selectedBrush_);
            //brushIcons_ = new Texture2D[brushes.Count];
        }


        protected override void OnPanelGUI(Map map)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            
            selectedBrush_ = CustomGUI.SelectionGrids.FromTextures(selectedBrush_, brushIcons_, 10, 32,32);

            GUILayout.EndHorizontal();
        }

        public override void OnKeyDown(KeyCode key)
        {
            base.OnKeyDown(key);
            if (!Event.current.shift)
                return;

            if( key == KeyCode.Q )
            {
                selectedBrush_ = 0;
            }

            if( key == KeyCode.E )
            {
                selectedBrush_ = 1;
            }
            
        }

        public override void OnDisable()
        {
            base.OnDisable();

            //Debug.LogFormat("Disabling brushes panel, selected brush: {0}", selectedBrush_);
            EditorPrefs.SetInt(PREFS_BRUSHINDEX_NAME, selectedBrush_);

            foreach (var brush in brushes_)
                brush.OnDisable();
        }

    }
}
