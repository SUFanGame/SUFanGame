using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using StevenUniverse.FanGame.World;

namespace StevenUniverse.FanGameEditor.SceneEditing
{
    public class BrushesPanel : MapEditorPanel
    {
        static bool foldout_ = false;
        LayersPanel layersPanel_;
        List<MapEditorBrush> brushes_;
        Texture2D[] brushIcons_;

        int selectedBrush_ = 0;
        public MapEditorBrush SelectedBrush_ { get { return brushes_[selectedBrush_]; } }

        const string PREFS_BRUSHINDEX_NAME = "MEBrushIndex";
        const string PREFS_BRUSHFOLDOUT_NAME = "MeBrushFoldout";

        static int brushColumns_ = 2;

        public int BrushCount_ { get { return brushes_.Count; } }

        bool skipGUI_ = false;

        public override Rect Area_
        {
            get
            {
                int x = 15;
                int y = 15;

                int brushWidth = 24 + brushColumns_ * 32 + brushColumns_ * 4;

                int foldoutLabelSize = (int)GUI.skin.label.CalcSize(new GUIContent(FoldoutTitle_)).x + 20;


                int w = foldout_ ? (int)Mathf.Max( foldoutLabelSize, brushWidth ) : foldoutLabelSize;
                int h = foldout_ ? 65 : 22;
                return new Rect(x, y, w, h);
            }
        }

        string FoldoutTitle_
        {
            get
            {
                return "Brushes => Current: " + SelectedBrush_.Name_;
            }
        }


        public BrushesPanel( LayersPanel layersPanel, List<MapEditorBrush> brushes ) : base()
        {
            layersPanel_ = layersPanel;
            brushes_ = brushes;
            brushIcons_ = brushes.Select(b => b.Texture_).ToArray();
            selectedBrush_ = EditorPrefs.GetInt( PREFS_BRUSHINDEX_NAME, 0);
            //Debug.LogFormat("Loading brushes panel, SelectedBrush: {0}", selectedBrush_);
            foldout_ = EditorPrefs.GetBool(PREFS_BRUSHFOLDOUT_NAME, true );
            //brushIcons_ = new Texture2D[brushes.Count];
        }


        protected override void OnRenderArea(Map map)
        {
            //Debug.LogFormat("Rendering area for brushes panel {0}", Area_);

            foldout_ = EditorGUILayout.Foldout(foldout_, FoldoutTitle_ );

            if (foldout_)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20f);

                
                    //selectedBrush_ = GUILayout.SelectionGrid(selectedBrush_, brushIcons_, 10);
                selectedBrush_ = CustomGUI.SelectionGrids.FromTextures(selectedBrush_, brushIcons_, 10, 32,32);
                //foreach (var brush in brushes_)
                //{
           
                //    var tex = brush.Texture_;
                //    var area = GUILayoutUtility.GetRect(tex.width, tex.height);
                //    GUI.DrawTexture(area, tex);
                //}

                GUILayout.EndHorizontal();
            }
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
            EditorPrefs.SetBool(PREFS_BRUSHFOLDOUT_NAME, foldout_);

            foreach (var brush in brushes_)
                brush.OnDisable();
        }

    }
}
