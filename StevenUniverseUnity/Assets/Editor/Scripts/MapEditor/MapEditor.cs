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

//       Proper support for undo/redo, currently it doesn't update the World's tilemap. Easy solution is to rebuild the tilemap
//       In OnUndoRedo?
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
        List<TileInstanceEditor> tilePrefabs_ = new List<TileInstanceEditor>();
        [SerializeField]
        List<GroupInstanceEditor> tileGroupPrefabs_ = new List<GroupInstanceEditor>();
        
        // Cache of sprites, built whenever our folder changes.
        [SerializeField]
        List<Sprite> sprites_ = new List<Sprite>();

        [SerializeField]
        List<Texture2D> tileTextures_ = new List<Texture2D>();

        // Currently selected tile
        [SerializeField]
        int selectedTileIndex_ = 0;
        [SerializeField]
        int selectedGroupIndex_ = 0;

        // Currently selected toolbar
        [SerializeField]
        int selectedToolbar_ = 0;


        // Scroll Pos, used by the sprite grid.
        [SerializeField]
        Vector2 tileGUIScrollPos_;
        [SerializeField]
        Vector2 groupGUIScrollPos_;

        static MapEditor instance_;

        /// <summary>
        /// Object in the scene that all map-editor-generated objects will be parented to.
        /// </summary>
        [SerializeField]
        World world_ = null;

        [SerializeField]
        int currentElevation_ = 0;

        enum Toolbar
        {
            PaintTile,
            PaintGroup,
        }

        #region INIT
        protected override void OnEnable()
        {
            base.OnEnable();
            titleContent.text = "MapEditor";
            instance_ = this;
        }

        void OnFocus()
        {
            instance_ = this;
        }

        protected override void OnSceneLoaded()
        {
            base.OnSceneLoaded();

            world_ = GameObject.FindObjectOfType<World>();
            VerifyWorld();
        }
        #endregion

        protected override void OnGUI()
        {
            base.OnGUI();

            // Draw our "folder" field. Note that unity doesn't really support folders-as-assets in a natural way. 
            // Could break in future versions.
            var inputFolder = EditorGUILayout.ObjectField("Tiles Folder", currentFolder_, typeof(UnityEditor.DefaultAsset), false);

            if (currentFolder_ != inputFolder)
            {
                currentFolder_ = inputFolder;

                if (currentFolder_ != null)
                {
                    var path = AssetDatabase.GetAssetPath(currentFolder_).Substring(7);

                    GetTilePrefabs( path );
                    GetGroupPrefabs( path );

                }
            }

            if (GUILayout.Button("Clear"))
            {
                Clear();
            }

            selectedToolbar_ = GUILayout.Toolbar(selectedToolbar_, System.Enum.GetNames(typeof(Toolbar)));

            switch ((Toolbar)selectedToolbar_)
            {
                case Toolbar.PaintTile:
                    TileOnGUI();
                    break;
                case Toolbar.PaintGroup:
                    GroupOnGUI();
                    break;
            }
        }

        protected override void OnKeyDown(KeyCode key)
        {
            base.OnKeyDown(key);

            if (Event.current.shift)
            {
                if (key == KeyCode.W)
                {
                    ++currentElevation_;
                }
                else if (key == KeyCode.S)
                {
                    --currentElevation_;
                }
                currentElevation_ = Mathf.Clamp(currentElevation_, 0, int.MaxValue);
            }

            switch ((Toolbar)selectedToolbar_)
            {
                case Toolbar.PaintTile:
                    OnTileKey(key);
                    break;
                case Toolbar.PaintGroup:
                    OnGroupKey(key);
                    break;
            }
        }

        protected override void OnMouseDown(int button, Vector3 cursorWorldPos)
        {
            base.OnMouseDown(button, cursorWorldPos);

            if (tilePrefabs_.Count == 0)
                return;

            if (button == 0)
            {
                // floor position to grid
                for (int i = 0; i < 2; ++i)
                    cursorWorldPos[i] = Mathf.Floor(cursorWorldPos[i]);
                cursorWorldPos.z = 0;


                switch ((Toolbar)selectedToolbar_)
                {
                    case Toolbar.PaintTile:
                        PlaceTile(cursorWorldPos);
                        break;
                    case Toolbar.PaintGroup:
                        PlaceGroup(cursorWorldPos);
                        break;
                }
            }

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



        [DrawGizmo(GizmoType.NotInSelectionHierarchy)]
        static void DrawGizmos(Transform t, GizmoType gizmoType)
        {
            // Bail out if we're not in "edit mode".
            if (!SceneEditorUtil.EditMode_)
                return;
            
            if ( instance_ == null )
                return;

            switch( (Toolbar)instance_.selectedToolbar_ )
            {
                case Toolbar.PaintTile:
                    TileDrawCursor();
                    break;

                case Toolbar.PaintGroup:
                    GroupDrawCursor();
                    break;
            }

            SceneView.currentDrawingSceneView.Repaint();

        }

        static void TileDrawCursor()
        {
            if (instance_.sprites_.Count == 0)
                return;

            var cursorPos = SceneEditorUtil.GetCursorPosition();

            var offset = Vector2.one * .5f;
            Gizmos.DrawWireCube(cursorPos + Vector3.back + (Vector3)offset, Vector3.one );


            // Convert world space to gui space
            var bl = HandleUtility.WorldToGUIPoint(cursorPos);
            var tr = HandleUtility.WorldToGUIPoint(cursorPos + Vector3.right + Vector3.up);
            var labelPos = HandleUtility.WorldToGUIPoint(cursorPos + Vector3.right + (Vector3.up));

            var oldColor = GUI.color;
            var col = GUI.color;

            // Can't use gui functions to draw directly into the scene view.
            Handles.BeginGUI();

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
            EditorGUI.LabelField(new Rect(labelPos.x, labelPos.y, 100f, 100f), "Elevation " + instance_.currentElevation_);

            Handles.EndGUI();
        }

        static void GroupDrawCursor()
        {
            if (instance_.tileGroupPrefabs_.Count == 0 )
                return;
            var selected = instance_.tileGroupPrefabs_[instance_.selectedGroupIndex_];

            Texture2D tex = null;
            while( tex == null )
            {
                tex = AssetPreview.GetAssetPreview(selected.gameObject);
            }

            var cursorPos = SceneEditorUtil.GetCursorPosition();


            var dims = instance_.GetGroupDimensions(selected);

            var offset = ((Vector3)dims.size / 2f);
            Gizmos.DrawWireCube(cursorPos + offset, Vector3.Scale(Vector3.one, dims.size) );

            var bl = HandleUtility.WorldToGUIPoint(cursorPos);
            var tr = HandleUtility.WorldToGUIPoint(cursorPos + (Vector3)dims.size);

            Handles.BeginGUI();

            var area = new Rect(bl, tr - bl);

            // Draw a semi-transparent image of our current tile on the cursor.
            var oldColor = GUI.color;
            var col = GUI.color;
            col.a = .25f;
            GUI.color = col;

            Rect texCoords = new Rect(0, 0, 1, -1);
            GUI.DrawTextureWithTexCoords(area, tex, texCoords);

            GUI.color = oldColor;

            Handles.EndGUI();
        }
 

        void TileOnGUI()
        {
            if (tilePrefabs_.Count == 0)
                return;
            // Draw our sprite grid from our cached list.
            selectedTileIndex_ = SceneEditorUtil.DrawSpriteGrid(
                selectedTileIndex_, sprites_, 50f,
                Screen.height - 75,
                ref tileGUIScrollPos_
                );


            selectedTileIndex_ = Mathf.Clamp(selectedTileIndex_, 0, sprites_.Count - 1);
        }

        void GroupOnGUI()
        {
            if (tileGroupPrefabs_.Count == 0)
                return;

            
            selectedGroupIndex_ = SceneEditorUtil.DrawAssetPreviewGrid(
                selectedGroupIndex_, 
                tileGroupPrefabs_.Select(p=>p.gameObject).ToList(), 
                ref groupGUIScrollPos_);

            selectedGroupIndex_ = Mathf.Clamp(selectedGroupIndex_, 0, tileGroupPrefabs_.Count - 1);
        }
        


        /// <summary>
        /// Place a tile at the given location.
        /// </summary>
        /// <param name="pos"></param>
        void PlaceTile( Vector3 pos )
        {
            // Cache our currently selected tile
            var selected = tilePrefabs_[selectedTileIndex_];


            var map = World.Instance.TileMap;

            var tilesAtPos = map.GetTiles(pos);

            if (tilesAtPos != null)
            {
                for (int i = 0; i < tilesAtPos.Count; ++i)
                {
                    var existing = tilesAtPos[i];
                    //Debug.LogFormat("Instance Layer: {0}, Prefab Layer {1}", existing.TileInstance.TileTemplate.TileLayer.Name, selected.TileInstance.TileTemplate.TileLayer.Name);

                    if (SameLayer(existing, selected))
                    {
                        // If our selected tile is the existing tile's prefab we already know this tile is at the target location
                        // so we can bail out now.
                        if (PrefabUtility.GetPrefabParent(existing) == selected)
                            return;
                        else
                        {
                            map.RemoveTile(pos, existing);
                            Undo.DestroyObjectImmediate(existing.gameObject);
                        }
                    }
                }
            }

            var newTile = (TileInstanceEditor)PrefabUtility.InstantiatePrefab(selected);

            newTile.transform.position = pos;
            newTile.Elevation = currentElevation_;
            pos.z = currentElevation_;
            newTile.name = string.Join(":", new string[] { pos.ToString(), newTile.name });
            newTile.transform.SetParent(World.Instance.transform);
            newTile.Instance.X = (int)pos.x;
            newTile.Instance.Y = (int)pos.y;

            map.AddTile(pos, newTile);
        }

        void PlaceGroup( Vector3 pos )
        {
            var selected = tileGroupPrefabs_[selectedGroupIndex_];


        }

        /// <summary>
        /// Check a and b share the same layer and elevation.
        /// </summary>
        bool SameLayer( TileInstanceEditor a, TileInstanceEditor b )
        {
            var aTemplate = a.TileInstance.TileTemplate;
            var bTemplate = b.TileInstance.TileTemplate;
            return aTemplate.TileLayer == bTemplate.TileLayer && a.Elevation == b.Elevation;
        }



        void OnTileKey( KeyCode key )
        {
            if (Event.current.shift)
            {
                if (key == KeyCode.E)
                {
                    ++selectedTileIndex_;
                }
                else if (key == KeyCode.Q)
                {
                    --selectedTileIndex_;
                }
                selectedTileIndex_ = Mathf.Clamp(selectedTileIndex_, 0, tilePrefabs_.Count - 1);
            }
        }

        void OnGroupKey( KeyCode key )
        {
            if (Event.current.shift)
            {
                if (key == KeyCode.E)
                {
                    ++selectedGroupIndex_;
                }
                else if (key == KeyCode.Q)
                {
                    --selectedGroupIndex_;
                }
                selectedTileIndex_ = Mathf.Clamp(selectedTileIndex_, 0, tilePrefabs_.Count - 1);
            }
        }


        void GetTilePrefabs( string path )
        {
            tilePrefabs_.Clear();
            sprites_.Clear();

            // Get our tile prefabs
            tilePrefabs_ = AssetUtil.GetAssets<TileInstanceEditor>(path);
            tilePrefabs_ = tilePrefabs_.Where((p) => p.TileInstance.TileTemplate.UsableIndividually).ToList();

            tileTextures_.Clear();
            foreach (var p in tilePrefabs_)
            {
                var renderer = p.GetComponent<SpriteRenderer>();
                if (renderer != null && renderer.sprite != null)
                {
                    sprites_.Add(renderer.sprite);
                }
            }
        }

        void GetGroupPrefabs( string path )
        {
            tileGroupPrefabs_.Clear();
            tileGroupPrefabs_ = AssetUtil.GetAssets<GroupInstanceEditor>(path);
        }

        void VerifyWorld()
        {
            if (world_ == null)
            {
                var go = new GameObject("World");
                world_ = go.AddComponent<World>();
            }
        }


        /// <summary>
        /// Clear all instances in the scene and reset the world object.
        /// </summary>
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
                        DestroyImmediate(instances[i].gameObject, false);
                }

                World.Instance.TileMap.Clear();
            }

        }

        [MenuItem("Tools/SUFanGame/MapEditor")]
        static void OpenWindow()
        {
            EditorWindow.GetWindow<MapEditor>();
        }

        Rect GetGroupDimensions( GroupInstanceEditor group )
        {
            Rect r = new Rect();

            r.min = new Vector2(float.MaxValue, float.MaxValue);
            r.max = new Vector2(float.MinValue, float.MinValue);

            var tiles = group.GetComponentsInChildren<TileInstanceEditor>();
            foreach( var t in tiles )
            {
                r.min = Vector2.Min(r.min, t.TileInstance.Position);
                r.max = Vector2.Max(r.max, t.TileInstance.Position + Vector3.one);
            }

            return r;
        }

    }
}
