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
using StevenUniverse.FanGame.Overworld.Templates; 



//  TODO: We'll have to be able to load existing world data
//       into the system from chunks. Is there a better way than polling all editor instances? Could be really slow on big maps.
namespace StevenUniverse.FanGameEditor.SceneEditing
{
    /// <summary>
    /// A user-friendly map editor window for painting tiles in a scene.
    /// </summary>
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

        // Currently selected tile
        [SerializeField]
        int selectedTileIndex_ = 0;
        // Currently selected group index
        [SerializeField]
        int selectedGroupIndex_ = 0;

        // Currently selected toolbar
        [SerializeField]
        int selectedToolbar_ = 0;
         

        // Scroll Positions, used by the tile selection grids.
        [SerializeField]
        Vector2 tileGUIScrollPos_;
        [SerializeField]
        Vector2 groupGUIScrollPos_;

        /// <summary>
        /// Object in the scene that all map-editor-generated objects will be parented to.
        /// Also holds the tile map data structure
        /// </summary>
        [SerializeField]
        MapEditorSceneObject sceneObject_ = null;

        /// <summary>
        /// RAII access to tile map object. Prevents aggressive game object spawning 
        /// just from having the map editor open even in scenes where we're not editing. 
        /// </summary>
        GameObject Map_//TileMap Map_
        {
            get
            {
                sceneObject_ = GameObject.FindObjectOfType<MapEditorSceneObject>();

                if (sceneObject_ == null)
                    sceneObject_ = new GameObject("MapEditor").AddComponent<MapEditorSceneObject>();

                return null;//sceneObject_.tileMap_;
            }
        }

        [SerializeField]
        int currentElevation_ = 0;

        /// <summary>
        /// Toolbars for our Map Editor.
        /// </summary>
        enum Toolbar
        {
            // For painting individual tiles
            PaintTile,
            // For painting tile groups
            PaintGroup,
        }

        #region INIT
        protected override void OnEnable()
        {
            base.OnEnable();
            titleContent.text = "MapEditor";
            Undo.undoRedoPerformed -= OnUndoRedo;
            Undo.undoRedoPerformed += OnUndoRedo;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Undo.undoRedoPerformed -= OnUndoRedo;
        }

        protected override void OnSceneLoaded()
        {
            base.OnSceneLoaded();
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

            if (button == 0)
            {
                // floor position to grid
                for (int i = 0; i < 2; ++i)
                    cursorWorldPos[i] = Mathf.Floor(cursorWorldPos[i]);


                if( EraseKeyIsBeingHeld() )
                {
                    EraseInstance( cursorWorldPos );

                    return;
                }

                switch ((Toolbar)selectedToolbar_)
                {
                    case Toolbar.PaintTile:
                        if (tilePrefabs_.Count == 0)
                            return;
                        PlaceTile(cursorWorldPos, tilePrefabs_[selectedTileIndex_] );

                        break;
                    case Toolbar.PaintGroup:
                        if (tileGroupPrefabs_.Count == 0)
                            return;

                        PlaceGroup(cursorWorldPos, tileGroupPrefabs_[selectedGroupIndex_] );

                        break;
                }
            }

        }

        protected override void OnMouseDrag(int button, Vector3 cursorWorldPos)
        {
            base.OnMouseDrag(button, cursorWorldPos);

            if( button == 0 )
            {
                // floor position to grid
                for (int i = 0; i < 2; ++i)
                    cursorWorldPos[i] = Mathf.Floor(cursorWorldPos[i]);


                if( EraseKeyIsBeingHeld() )
                {
                    EraseInstance(cursorWorldPos);
                    return;
                }


                switch( (Toolbar)selectedToolbar_)
                {
                    case Toolbar.PaintTile:
                        if (tilePrefabs_.Count == 0)
                            return;
                        PlaceTile(cursorWorldPos, tilePrefabs_[selectedTileIndex_]);
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
            GUILayout.Label("To Erase: Hold control");


            Handles.EndGUI();
        }



        [DrawGizmo(GizmoType.NotInSelectionHierarchy)]
        static void DrawGizmos(Transform t, GizmoType gizmoType)
        {
            // Bail out if we're not in "edit mode".
            if (!SceneEditorUtil.EditMode_)
                return;

            // Hilariously this seems to be how unity checks if an editor window is open. Sure, why not.
            // https://github.com/MattRix/UnityDecompiled/blob/master/UnityEditor/UnityEditor/EditorWindow.cs#L598-L600
            MapEditor[] arr = Resources.FindObjectsOfTypeAll<MapEditor>();
            var instance = (arr.Length <= 0) ? null : (arr[0] as MapEditor);
            
            if ( instance == null )
                return;

            var cursorPos = SceneEditorUtil.GetCursorPosition();
            var labelPos = HandleUtility.WorldToGUIPoint(cursorPos + Vector3.right + (Vector3.up));

            // Draw our elevation label.
            Handles.BeginGUI();
            EditorGUI.LabelField(new Rect(labelPos.x, labelPos.y, 100f, 100f), "Elevation " + instance.currentElevation_);
            Handles.EndGUI();

            // Check if our "erase key" is being held down
            if( EraseKeyIsBeingHeld() )
            {
                EraseDrawCursor();
            }
            else
            {
                switch ((Toolbar)instance.selectedToolbar_)
                {
                    case Toolbar.PaintTile:
                        TileDrawCursor( instance );
                        break;

                    case Toolbar.PaintGroup:
                        GroupDrawCursor( instance );
                        break;
                }
            }

            SceneView.currentDrawingSceneView.Repaint();

        }

        /// <summary>
        /// Draw the "erase tile" cursor.
        /// </summary>
        static void EraseDrawCursor()
        {
            var cursorPos = SceneEditorUtil.GetCursorPosition();

            var oldColor = Gizmos.color;

            Gizmos.color = Color.red;
            var offset = Vector2.one * .5f;
            Gizmos.DrawWireCube(cursorPos + Vector3.back + (Vector3)offset, Vector3.one);

            Gizmos.color = oldColor;
        }

        /// <summary>
        /// Draw a semi-transparent cursor of our currently selected tile.
        /// </summary>
        static void TileDrawCursor( MapEditor instance )
        {
            if (instance.sprites_.Count == 0)
                return;

            var cursorPos = SceneEditorUtil.GetCursorPosition();

            var offset = Vector2.one * .5f;
            Gizmos.DrawWireCube(cursorPos + Vector3.back + (Vector3)offset, Vector3.one );

            // Convert world space to gui space
            var bl = HandleUtility.WorldToGUIPoint(cursorPos);
            var tr = HandleUtility.WorldToGUIPoint(cursorPos + Vector3.right + Vector3.up);

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
                instance.sprites_[instance.selectedTileIndex_],
                false, true);
            GUI.color = oldColor;

            Handles.EndGUI();
        }

        /// <summary>
        /// Draw a semi-transparent cursor of our currently selected tile group.
        /// </summary>
        static void GroupDrawCursor( MapEditor instance )
        {
            if (instance.tileGroupPrefabs_.Count == 0 )
                return;
            var selected = instance.tileGroupPrefabs_[instance.selectedGroupIndex_];

            // GetAssetPreview runs asynchronously and seems to destroy and recreate the textures
            // arbitrarily (meaning they can't be cached). Best way seems to be to block until we 
            // get a valid texture. Seems to perform alright, it might explode
            // if it's trying to draw a LOT of tile groups??
            Texture2D tex = null;
            while( tex == null )
            {
                tex = AssetPreview.GetAssetPreview(selected.gameObject);
            }

            var cursorPos = SceneEditorUtil.GetCursorPosition();

            var size = instance.GetTileGroupSize(selected);

            var offset = ((Vector3)size / 2f);
            Gizmos.DrawWireCube(cursorPos + offset, Vector3.Scale(Vector3.one, size) );

            var bl = HandleUtility.WorldToGUIPoint(cursorPos);
            var tr = HandleUtility.WorldToGUIPoint(cursorPos + (Vector3)size);

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
 
        /// <summary>
        /// Draw the editor window gui for the tile painting toolbar.
        /// </summary>
        void TileOnGUI()
        {
            if (tilePrefabs_.Count == 0)
                return;

            EditorGUILayout.LabelField("Layer: " + tilePrefabs_[selectedTileIndex_].TileInstance.TileTemplate.TileLayerName);

            // Draw our sprite grid from our cached list.
            selectedTileIndex_ = SceneEditorUtil.DrawSpriteGrid(
                selectedTileIndex_, sprites_, 50f,
                Screen.height - 75,
                ref tileGUIScrollPos_
                );


            selectedTileIndex_ = Mathf.Clamp(selectedTileIndex_, 0, sprites_.Count - 1);
        }

        /// <summary>
        /// Draw the editor window gui for the tile group paining toolbar.
        /// </summary>
        void GroupOnGUI()
        {
            if (tileGroupPrefabs_.Count == 0)
                return;
            var group = tileGroupPrefabs_[selectedGroupIndex_];

            EditorGUILayout.LabelField(group.name);
            
            selectedGroupIndex_ = SceneEditorUtil.DrawAssetPreviewGrid(
                selectedGroupIndex_, 
                tileGroupPrefabs_.Select(p=>p.gameObject).ToList(), 
                ref groupGUIScrollPos_);

            selectedGroupIndex_ = Mathf.Clamp(selectedGroupIndex_, 0, tileGroupPrefabs_.Count - 1);
        }
        
        /// <summary>
        /// Erase an editor instance at the given position. Allows undo. Currently erases ALL layers at the given position.
        /// How do we pick a sensible layer??
        /// </summary>
        void EraseInstance( Vector2 pos )
        {
            var layers = TileTemplate.Layer.Instances;
            var map = Map_;

            Undo.SetCurrentGroupName("Create Tile Group");

            int undoIndex = Undo.GetCurrentGroup();
            
            foreach ( var layer in layers )
            {
                var index = new TileIndex(pos, currentElevation_, layer);

                var existing = map.RemoveAt(index);

                if( existing != null )
                    Undo.DestroyObjectImmediate(existing.gameObject);
            }

            Undo.CollapseUndoOperations( undoIndex );
        }

        /// <summary>
        /// Place a tile at the given location.
        /// </summary>
        /// <param name="pos"></param>
        void PlaceTile( Vector2 pos, TileInstanceEditor tile )
        {
            var map = Map_;

            var layer = tile.TileInstance.TileTemplate.TileLayer;
            
            var index = new TileIndex(pos, currentElevation_, layer);
            var existing = map.RemoveAt( index);

            if (existing != null)
                Undo.DestroyObjectImmediate(existing.gameObject);

            var newTile = (TileInstanceEditor)PrefabUtility.InstantiatePrefab(tile);
            Undo.RegisterCreatedObjectUndo(newTile.gameObject, "Place Tile");

            newTile.transform.position = index.Position;
            newTile.Instance.Position = index.Position;
            
            newTile.Elevation = index.Elevation;
            newTile.name = string.Join(":", new string[] { pos.ToString(), newTile.name });
            // Note we've called Map_ above so this shouldn't be null at this point.
            newTile.transform.SetParent(sceneObject_.transform);

            map.AddInstance(index, newTile);
        }

        /// <summary>
        /// Place a tile group from a prefab.
        /// </summary>
        void PlaceGroup( Vector3 pos, GroupInstanceEditor groupPrefab )
        {
            var map = Map_;

            Undo.SetCurrentGroupName("Create Tile Group");

            int undoIndex = Undo.GetCurrentGroup();

            var group = (GroupInstanceEditor)PrefabUtility.InstantiatePrefab(groupPrefab);

            group.transform.position = pos;
            group.Instance.Position = pos;
            group.Elevation = currentElevation_;

            group.name = string.Join(":", new string[] { pos.ToString(), group.name });

            // Note we called Map_ above so sceneObject should not be null
            Undo.SetTransformParent(group.transform, sceneObject_.transform, "Parent group to world");

            Undo.RegisterCreatedObjectUndo(group.gameObject, "Create tile group");

            var indices = TileMap.GroupTileIndices(group);

            // Iterate through all indices in this group and add a reference to the group at each index
            foreach (var index in indices )
            {
                var existing = map.RemoveAt(index);

                // Remove any existing instance at the position.
                if( existing != null )
                {
                    Undo.DestroyObjectImmediate(existing.gameObject);
                }

                // Add our group reference.
                map.AddInstance(index, group);
            }

            Undo.CollapseUndoOperations(undoIndex);
        }

        /// <summary>
        /// For cycling through the currently selected tiles in the map editor window.
        /// </summary>
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

        /// <summary>
        /// For cycling through the currently selected groups in the map editor window.
        /// </summary>
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

        /// <summary>
        /// Refresh our list of tile prefabs.
        /// </summary>
        /// <param name="path"></param>
        void GetTilePrefabs( string path )
        {
            tilePrefabs_.Clear();
            sprites_.Clear();

            // Get our tile prefabs
            tilePrefabs_ = AssetUtil.GetAssets<TileInstanceEditor>(path);
            tilePrefabs_ = tilePrefabs_.Where((p) => p.TileInstance.TileTemplate.UsableIndividually).ToList();

            foreach (var p in tilePrefabs_)
            {
                var renderer = p.GetComponent<SpriteRenderer>();
                if (renderer != null && renderer.sprite != null)
                {
                    sprites_.Add(renderer.sprite);
                }
            }
        }

        /// <summary>
        /// Refresh our list of tile group prefabs.
        /// </summary>
        void GetGroupPrefabs( string path )
        {
            tileGroupPrefabs_.Clear();
            tileGroupPrefabs_ = AssetUtil.GetAssets<GroupInstanceEditor>(path);
        }

        /// <summary>
        /// Clear all instances in the scene and reset the world object. Could easily make this undoable if needed.
        /// </summary>
        void Clear()
        {
            if (!EditorUtility.DisplayDialog(
                "Destroy all Tile Instances",
                "Are you sure you want to destroy ALL TILE INSTANCES in the current scene? You can't undo this.",
                "Yes", "No"))
                return;

            var instances = FindObjectsOfType<InstanceEditor>();
            
            if( instances.Length > 0 )
            {
                for (int i = instances.Length - 1; i >= 0; --i)
                {
                    if (instances[i] != null && instances[i].gameObject != null)
                        DestroyImmediate(instances[i].gameObject, false);
                }

                Map_.Clear();
            }

        }
        
        static bool EraseKeyIsBeingHeld()
        {
            return Event.current.control;
        }

        [MenuItem("Tools/SUFanGame/MapEditor")]
        static void OpenWindow()
        {
            EditorWindow.GetWindow<MapEditor>();
        }

        /// <summary>
        /// Gets the tile count on each axis of the given tile group.
        /// </summary>
        Vector2 GetTileGroupSize( GroupInstanceEditor group )
        {
            Vector2 size = Vector2.zero;

            var tiles = group.GetComponentsInChildren<TileInstanceEditor>();
            foreach( var t in tiles )
            {
                size = Vector2.Max(size, t.TileInstance.Position + Vector3.one);
            }

            return size;
        }

        /// <summary>
        /// Handle undo redo by just rebuilding the entire tile map each time. Not exactly elegant but it works.
        /// </summary>
        void OnUndoRedo()
        {
            var map = Map_;
            map.Clear();
            var tiles = GameObject.FindObjectsOfType<TileInstanceEditor>();

            foreach( var t in tiles )
            {
                var index = new TileIndex(t.transform.position, t.Elevation, t.TileInstance.TileTemplate.TileLayer);

                var group = t.transform.parent.GetComponent<GroupInstanceEditor>();
                if (group != null)
                {
                    map.AddInstance(index, group);
                }
                else
                {
                    map.AddInstance(index, t);
                }
            }
        }

    }
}
