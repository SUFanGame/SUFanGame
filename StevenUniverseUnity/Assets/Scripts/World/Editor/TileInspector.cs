using UnityEngine;
using UnityEditor;
using System.IO;
using SUGame.World;

namespace SUGame.SUGameEditor.World.Inspector
{
    [CustomEditor(typeof(Tile))]
    public class TileInspector : Editor
    {
        //static string tilePrefabPath_ = "Prefabs/Tiles";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var tar = target as Tile;

            var sprite = tar.Sprite_;

            if (sprite == null)
                return;

            //SceneEditorUtil.DrawSprite(sprite, 150f);
        }

        [MenuItem("Create Tile", menuItem = "Assets/Create/Tile", priority = 230)]
        static void CreateTile()
        {
            string path = EditorUtility.SaveFilePanel("Create Tile", "Prefabs/Tiles", "Tile", "prefab");

            var fileName = Path.GetFileNameWithoutExtension(path);

            if (string.IsNullOrEmpty(path))
                return;

            Debug.LogFormat("Path: {0}", path);

            var go = new GameObject(fileName);
            go.AddComponent<Tile>();

            PrefabUtility.CreatePrefab("Assets/Prefabs/Tiles/" + fileName + ".prefab", go, ReplacePrefabOptions.Default);

            DestroyImmediate(go);
        }

    }

}