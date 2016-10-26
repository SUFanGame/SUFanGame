using UnityEditor;
using UnityEngine;
using StevenUniverse.FanGame.Characters;
using StevenUniverse.FanGame.Overworld;
using StevenUniverse.FanGame.Overworld.Templates;

namespace StevenUniverse.FanGameEditor.Tools
{
    public static class ToolUtilities
    {
        public static void CancelOperation(string error)
        {
            EditorUtility.ClearProgressBar();
            throw new UnityException(error);
        }

        public static void ClearAllJsonCaches()
        {
            Chunk.ClearJsonCache();
            Template.ClearJsonCache();
            TileTemplate.ClearJsonCache();
            GroupTemplate.ClearJsonCache();
            CharacterData.ClearJsonCache();
            //XCharacter.ClearJsonCache();
            //PlayableCharacter.ClearJsonCache();
        }
    }
}