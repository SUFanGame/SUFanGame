using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using StevenUniverse.FanGame.OverworldEditor;
using StevenUniverse.FanGame.Overworld;
using StevenUniverse.FanGame.Overworld.Instances;
using StevenUniverse.FanGameEditor.Tools;
using System.Linq;

/// <summary>
/// A user-friendly map editor window for painting tiles in a scene.
/// </summary>

//  TODO: We'll have to be able to load existing world data
//       into the system from chunks. Is there a better way than polling all editor instances? Could be really slow on big maps.
//       
//       Support for multiple tiles on different layers at the same position/elevation
//             Dust: Just realized I should clarify something about tiles. It's fine to place two tiles in the same position and 
//                   same elevation as long as they're in different tile layers. That's how you get things like flowers on top of grass 
//                   at the same elevation.  So what you really need to check for is tiles at the same position, elevation, and tile layer.
//                   And then I guess we should decide if trying to place a conflicting tile results in an error or if it results in 
//                   replacing the existing tile

//       RE: Serialization of the positionmap. Editorwindows are pretty quirky, they can't serialize monobehaviours that are
//       part of a scene and OnEnable occurs BEFORE serialization happens and the scene is loaded, so any set up in
//       OnEnable is lost. For now I will do set up once when I know a scene is loaded using HierarchyChanged callbacks.
//       Another possible solution for Serializing the PositionMap: Reference gameobjects instead of monobehaviours. Then getcomponent
//       the scripts as needed. Editorwindows seem to be able to serialize gameobjects just fine.
namespace StevenUniverse.FanGameEditor.SceneEditing
{
    public class MapEditor : SceneEditorWindow
    {
        // The folder the map editor will build our list of tiles from
        [SerializeField]
        UnityEngine.Object currentFolder_ = null;
        // Cache of tile instances, built whenever our folder changes.
        // The buttons in the Editor Window correspond back to these
        [SerializeField]
        List<TileInstanceEditor> editorInstances_ = new List<TileInstanceEditor>();
        
        // Cache of sprites, built whenever our folder changes.
        [SerializeField]
        List<Sprite> sprites_ = new List<Sprite>();

        // Currently selected tile
        [SerializeField]
        int selectedTileIndex_ = 0;

        // Scroll Pos, used by the sprite grid.
        [SerializeField]
        Vector2 scrollPos_;

        static MapEditor instance_;

        /// <summary>
        /// Object in the scene that all map-editor-generated objects will be parented to.
        /// </summary>
        [SerializeField]
        GameObject tileInstanceParent_;

        [SerializeField]
        int currentElevation_ = 0;

        //[SerializeField]
        TilePositionMap positionMap_;

        protected override void OnEnable()
        {
            base.OnEnable();
            titleContent.text = "MapEditor";
            instance_ = this;
        }

        protected override void OnSceneLoaded()
        {
            base.OnSceneLoaded();

            tileInstanceParent_ = GameObject.Find("MapEditorTiles");
            if (tileInstanceParent_ == null)
                tileInstanceParent_ = new GameObject("MapEditorTiles");

            var tiles = GameObject.FindObjectsOfType<TileInstanceEditor>();
            positionMap_ = new TilePositionMap(tiles);
        }

        [MenuItem("Tools/SUFanGame/MapEditor")]
        static void OpenWindow()
        {
            EditorWindow.GetWindow<MapEditor>();
        }

        protected override void OnSceneGUI(SceneView view)
        {
            base.OnSceneGUI(view);

            if (!SceneEditorUtil.EditMode_)
                return;

            Handles.BeginGUI();
            
            GUILayout.Label("Raise/Lower Elevation: Shift W/S");
            GUILayout.Label("Next/Previous Tile: Shift E/Q");

            Handles.EndGUI();
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

            instance_.selectedTileIndex_ = Mathf.Clamp(instance_.selectedTileIndex_, 0, instance_.sprites_.Count - 1);

            // Draw a semi-transparent image of our current tile on the cursor.
            col.a = .25f;
            GUI.color = col;
            // Vertical UVs are flipped in the scene...?
            SceneEditorUtil.DrawSprite(
                Rect.MinMaxRect(bl.x, bl.y, tr.x, tr.y), 
                instance_.sprites_[instance_.selectedTileIndex_], 
                false, true);
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

            if( GUILayout.Button("WriteToJSON") )
            {
                Debug.Log("Whoops, I'm not implemented yet!");
            }

            if( GUILayout.Button("Debug"))
            {
                positionMap_.Print();
            }

            if ( GUILayout.Button("Clear") )
            {
                Clear();
            }

            if ( editorInstances_.Count > 0 )
            {
                // Draw our sprite grid from our cached list.
                selectedTileIndex_ = SceneEditorUtil.DrawSpriteGrid(
                    selectedTileIndex_, sprites_, 50f, 
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
            editorInstances_.Clear();
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
                    editorInstances_.Add(instance);
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

            if (editorInstances_.Count == 0)
                return;

            if( button ==  0 )
            {
                // floor position to grid
                for (int i = 0; i < 2; ++i)
                    cursorWorldPos[i] = Mathf.Floor(cursorWorldPos[i]);
                cursorWorldPos.z = 0;

                // Get the list of instances at our cursor position
                var listOfInstances = positionMap_.Get(cursorWorldPos);
                // Cache our currently selected tile
                var selected = editorInstances_[selectedTileIndex_];


                // If there's a list
                if (listOfInstances != null)
                {
                    //Debug.LogFormat("Found list of tiles at location {0}", cursorWorldPos);

                    //foreach ( var t in listOfInstances )
                    //{
                    //    if (t == null)
                    //        Debug.LogFormat("Existing tile reference at location {0} is null...", cursorWorldPos);
                    //}

                    //var tileStrings = listOfInstances.Select(
                    //    t => string.Format("Tile {0} at Pos {1}, Elevation {2}", 
                    //    t.name, 
                    //    t.transform.position.ToString(), 
                    //    t.Elevation.ToString()) ).ToArray();

                    //Debug.LogFormat("Tiles at {0}", cursorWorldPos);
                    //foreach ( var str in tileStrings )
                    //{
                    //    Debug.Log(str);
                    //}

                    // There is probably an easier/more efficient way to do this.
                    System.Predicate<TileInstanceEditor> match = (a) => a.Elevation == currentElevation_;
                    // Then we check to see if any tiles exist at our target elevation.
                    var atElevation = listOfInstances.Find(match);
                    if (atElevation != null)
                    {
                        //Debug.LogFormat("Destroying existing tiles at {0}, Elevation {1}", cursorWorldPos, currentElevation_);
              
                        Undo.DestroyObjectImmediate(atElevation.gameObject);
                        positionMap_.RemoveAll(cursorWorldPos, match);
                    }
                }

                //Debug.LogFormat("Creating tile {0} at {1}", selected.name, cursorWorldPos);
                GameObject instanceGO = (GameObject)PrefabUtility.InstantiatePrefab(selected.gameObject);
                Undo.RegisterCreatedObjectUndo(instanceGO, "PaintedTileInstance");

                var instance = instanceGO.GetComponent<TileInstanceEditor>();
                instance.transform.position = cursorWorldPos;
                instance.Elevation = currentElevation_;
                cursorWorldPos.z = currentElevation_;
                instance.name = string.Join( ":", new string[] { cursorWorldPos.ToString(), instance.name } );
                instance.transform.SetParent(tileInstanceParent_.transform);
                instance.Instance.X = (int)cursorWorldPos.x;
                instance.Instance.Y = (int)cursorWorldPos.y;

                positionMap_.AddValue(cursorWorldPos, instance);
            }

        }

        protected override void OnKeyDown(KeyCode key)
        {
            base.OnKeyDown(key);

            if( Event.current.shift )
            {
                if (key == KeyCode.W)
                {
                    ++currentElevation_;
                }
                else if (key == KeyCode.S)
                {
                    --currentElevation_;
                }
                else if ( key == KeyCode.E )
                {
                    ++selectedTileIndex_;
                }
                else if ( key == KeyCode.Q )
                {
                    --selectedTileIndex_;
                }
            }

            currentElevation_ = Mathf.Clamp(currentElevation_, 0, int.MaxValue);
            selectedTileIndex_ = Mathf.Clamp(selectedTileIndex_, 0, editorInstances_.Count - 1);
            
        }

        // Clear all tile instances in the current scene. Right now the position map is not serializable so there's
        // no way for unity to rebuild it if we tried to undo this.
        void Clear()
        {
            if (!EditorUtility.DisplayDialog(
                "Destroy all Tile Instances",
                "Are you sure you want to destroy ALL TILE INSTANCES in the current scene? You can't undo this.",
                "Yes", "No"))
                return;

            var instances = FindObjectsOfType<TileInstanceEditor>();
            
            if( instances.Length > 0 )
            {
                for (int i = instances.Length - 1; i >= 0; --i)
                {
                    if (instances[i] != null && instances[i].gameObject != null)
                        DestroyImmediate(instances[i].transform.root.gameObject, false);
                }

                if (positionMap_ != null)
                    positionMap_.Clear();
            }

        }

    }
}
