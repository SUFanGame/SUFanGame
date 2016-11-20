using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using StevenUniverse.FanGame.World;

namespace StevenUniverse.FanGameEditor.SceneEditing
{
    public class MapEditor : SceneEditorWindow
    {

        MapEditorCursor cursor_ = null;
        Map map_;

        [MenuItem("Tools/SUFanGame/MapEditor")]
        static void OpenWindow()
        {
            EditorWindow.GetWindow<MapEditor>();
        }


        protected override void OnSceneGUI(SceneView view)
        {
            base.OnSceneGUI(view);

            LayersPane.OnSceneGUI();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            titleContent.text = "MapEditor";

            LayersPane.OnEnable( map_ );
        }
        

        protected override void OnDisable()
        {
            base.OnDisable();

            LayersPane.OnDisable();
        }
    }
}
