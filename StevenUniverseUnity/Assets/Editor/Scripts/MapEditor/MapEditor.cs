using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using StevenUniverse.FanGame.OverworldEditor;
using StevenUniverse.FanGame.Overworld;
using StevenUniverse.FanGame.Overworld.Instances;

// TODO: We'll have to be able to load existing world data
//       into the system so we know our world bounds and can poll
//       tiles quickly. Maybe just a button that iterates through all chunks.
//       As tiles are added we need to keep track as well - maybe use coordinated list?
//       We can populate the list via TileEditorInstance.instance. But what about cases
//       where We need to refer back to the TileEditorInstance from an instance in the coordinated list?
//       May need to use a custom system to track editor instances by position, similar to coordinated list
namespace StevenUniverse.FanGameEditor.SceneEditing
{
    public class MapEditor : SceneEditorWindow
    {
        // The folder the map editor will build our list of tiles from
        [SerializeField]
        UnityEngine.Object currentFolder_ = null;
        // Cache of tile instances, built whenever our folder changes.
        [SerializeField]
        List<TileInstanceEditor> instances_ = new List<TileInstanceEditor>();
        
        // Cache of sprites, built whenever our folder changes.
        [SerializeField]
        List<Sprite> sprites_ = new List<Sprite>();

        // Currently selected tile
        [SerializeField]
        int selectedTile_ = 0;

        // Scroll Pos, used by the sprite grid.
        [SerializeField]
        Vector2 scrollPos_;

        static MapEditor instance_;

        CoordinatedList<Instance> tileInstances_ = new CoordinatedList<Instance>();

        /// <summary>
        /// Object in the scene that all map-editor-generated objects will be parented to.
        /// </summary>
        [SerializeField]
        GameObject tileInstanceParent_;

        [SerializeField]
        int currentElevation_ = 0;

        protected override void OnEnable()
        {
            base.OnEnable();
            instance_ = this;

            tileInstanceParent_ = GameObject.Find("MapEditorTiles");
            if (tileInstanceParent_ == null)
                tileInstanceParent_ = new GameObject("MapEditorTiles");
        }

        [MenuItem("Tools/SUFanGame/MapEditor")]
        static void OpenWindow()
        {
            EditorWindow.GetWindow<MapEditor>();
        }

        protected override void OnSceneGUI(SceneView view)
        {
            base.OnSceneGUI(view);
        }

        void OnFocus()
        {
            instance_ = this;
        }

        [DrawGizmo(GizmoType.NotInSelectionHierarchy)]
        static void DrawGizmos(Transform t, GizmoType gizmoType)
        {
            // Bail out if we're not in "edit mode".
            if (!SceneEditorUtil.EditMode_)
                return;

            var cursorPos = SceneEditorUtil.DrawCursor();


            if ( instance_ == null )
                return;


            if (instance_.sprites_.Count == 0)
                return;

            // Convert world space to gui space
            var bl = HandleUtility.WorldToGUIPoint(cursorPos);
            var tr = HandleUtility.WorldToGUIPoint(cursorPos + Vector3.right + Vector3.up);
            var labelPos = HandleUtility.WorldToGUIPoint(cursorPos + Vector3.right + (Vector3.up));

            var oldColor = GUI.color;
            var col = GUI.color;

            // Can't use gui functions to draw directly into the scene view.
            Handles.BeginGUI();

            instance_.selectedTile_ = Mathf.Clamp(instance_.selectedTile_, 0, instance_.sprites_.Count - 1);

            // Draw a semi-transparent image of our current tile on the cursor.
            col.a = .25f;
            GUI.color = col;
            SceneEditorUtil.DrawSprite(Rect.MinMaxRect(bl.x, bl.y, tr.x, tr.y), instance_.sprites_[instance_.selectedTile_]);
            GUI.color = oldColor;

            // Draw a label showing the cursor's current elevation.
            EditorGUI.LabelField(new Rect(labelPos.x, labelPos.y, 100f, 100f), "Elevation " + instance_.currentElevation_ );

            Handles.EndGUI();

        }
        
        protected override void OnGUI()
        {
            base.OnGUI();

            // Draw our "folder" field. Note that unity doesn't really support folders-as-assets in a natural way. 
            // Could break in future versions.
            var inputFolder = EditorGUILayout.ObjectField("Tiles Folder", currentFolder_, typeof(UnityEditor.DefaultAsset), false );

            if ( currentFolder_ != inputFolder )
            {
                currentFolder_ = inputFolder;

                if( currentFolder_ != null )
                {
                    GetTileInstances( AssetDatabase.GetAssetPath(currentFolder_).Substring(7));
                }
            }

            if( instances_.Count > 0 )
            {
                // Draw our sprite grid from our cached list.
                selectedTile_ = SceneEditorUtil.DrawSpriteGrid(
                    selectedTile_, sprites_, 50f, 
                    Screen.height - 75,
                    Color.white,
                    new Color(.25f, .25f, .25f),
                    ref scrollPos_
                    );
            }
        }

        /// <summary>
        /// Get all tile instances at the given path (assumes the given path begins at and excludes the assets folder)
        /// Caches the results in the instances/sprites lists
        /// </summary>
        void GetTileInstances( string path )
        {
            instances_.Clear();
            sprites_.Clear();
            // AssetDatabase doesn't allow us to load all resources in a single folder.
            // So we have to iterate over each file, or use the Resources folder
            // http://answers.unity3d.com/questions/24060/can-assetdatabaseloadallassetsatpath-load-all-asse.html
            // Load all the prefabs in all subfolders of the given path
            var files = Directory.GetFiles(Application.dataPath + "/" + path, "*.prefab", SearchOption.AllDirectories );
            float progress = 0;

            foreach (var file in files)
            {
                EditorUtility.DisplayProgressBar("Loading Tiles", file, progress);

                var filePath = "Assets" + file.Replace(Application.dataPath, "").Replace('\\', '/');

                // Get the actual gameobject and editor instance from our asset
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(filePath);
                var instance = go.GetComponent<TileInstanceEditor>();

                if( instance != null )
                {
                    // Populate our lists if our instance is valid.
                    instances_.Add(instance);
                    var renderer = go.GetComponent<SpriteRenderer>();
                    if( renderer != null && renderer.sprite != null )
                        sprites_.Add( renderer.sprite );
                }

                progress += 1f / (float)files.Length;
            }

            EditorUtility.ClearProgressBar();
        }


        protected override void OnMouseDown(int button, Vector3 cursorWorldPos)
        {
            base.OnMouseDown(button, cursorWorldPos);

            if (instances_.Count == 0)
                return;


        }

        protected override void OnKeyDown(KeyCode key)
        {
            base.OnKeyDown(key);

            if (Event.current.shift && key == KeyCode.W)
            {
                ++currentElevation_;
            }
            else if (Event.current.shift && key == KeyCode.S)
            {
                --currentElevation_;
            }

            currentElevation_ = Mathf.Clamp(currentElevation_, 0, int.MaxValue);

        }

    }
}
