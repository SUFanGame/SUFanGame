using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using StevenUniverse.FanGame.Util;
using StevenUniverse.FanGameEditor.Wizards;

namespace StevenUniverse.FanGameEditor.Tools
{
    public static class OtherTools
    {
        [MenuItem("Tools/SUFanGame/Other/Replace Prefab")]
        public static void CreateReplacePrefabWizard()
        {
            ScriptableWizard.DisplayWizard<ReplacePrefabWizard>("Replace Prefab", "Replace", "Cancel");
        }

        [MenuItem("Tools/SUFanGame/Other/Remove Parentheses")]
        public static void RemoveParentheses()
        {
            //Loop through all selected GameObjects
            foreach (GameObject selectedGameObject in Selection.gameObjects)
            {
                //Cache the GameObject's name
                string name = selectedGameObject.name;

                //Cache the indexes of the last left and right parentheses
                int lastLeftIndex = name.LastIndexOf('(');
                int lastRightIndex = name.LastIndexOf(')');

                //IF there was a last left parentheses
                //AND the character before the last left parentheses was an empty space
                //AND the last right parentheses is the last index
                if (lastLeftIndex != -1 && name[lastLeftIndex - 1] == ' ' && lastRightIndex == name.Length - 1)
                {
                    //Remove the contents of the parentheses and the space before it
                    selectedGameObject.name = name.Substring(0, lastLeftIndex - 1);
                }
            }
        }

        //Optimization
        [MenuItem("Tools/SUFanGame/Other/Optimize Known Texture Locations")]
        public static void OptimizeKnownTextureLocations()
        {
            //Optimize the Template Textures
            SetMaxTextureSize(Application.dataPath + "/Editor/Prefabs/EditorInstances", 32);
            OtherTools.SetPivot(Application.dataPath + "/Editor/Prefabs/EditorInstances", SpriteAlignment.BottomLeft);
            //Optimize the CustomizationItem Textures
            SetMaxTextureSize(Application.dataPath + "/Resources/Textures/CustomizationItems", 128);
            OtherTools.SetPivot(Application.dataPath + "/Resources/Textures/CustomizationItems",
                SpriteAlignment.BottomLeft);
            //Optimize the Ghost Textures
            SetMaxTextureSize(Application.dataPath + "/Resources/Textures/Ghosts", 512);
            OtherTools.SetPivot(Application.dataPath + "/Resources/Textures/Ghosts", SpriteAlignment.BottomLeft);

            EditorSceneManager.SaveOpenScenes();
        }

        public static void SetMaxTextureSize(string textureDirectoryAbsolute, int maxTextureSize)
        {
            //Get a path to all textures in the given directory
            string[] texturePathsAbsolute = Directory.GetFiles(textureDirectoryAbsolute, "*.png",
                SearchOption.AllDirectories);

            //Create a variable to keep track of progress for the progress bar
            float progress = 0f;

            //Update the import settings of each Texture
            foreach (string texturePathAbsolute in texturePathsAbsolute)
            {
                //Display the progress bar
                EditorUtility.DisplayProgressBar("Optimizing Textures in " + textureDirectoryAbsolute,
                    texturePathAbsolute, progress);

                string assetPath = Utilities.ConvertAbsolutePathToAssetPath(texturePathAbsolute);

                TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                TextureImporterSettings settings = new TextureImporterSettings();
                textureImporter.ReadTextureSettings(settings);

                if (settings.maxTextureSize != maxTextureSize || settings.spritePixelsPerUnit != 16)
                {
                    settings.maxTextureSize = maxTextureSize;
                    //TODO make this a parameter
                    settings.spritePixelsPerUnit = 16;
                    textureImporter.SetTextureSettings(settings);
                    AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                }

                //Increment the progress
                progress += 1/(float) texturePathsAbsolute.Length;
            }

            EditorUtility.ClearProgressBar();
        }

        public static void SetPivot(string textureDirectoryAbsolute, SpriteAlignment spriteAlignment)
        {
            //Get a path to all textures in the given directory
            string[] texturePathsAbsolute = Directory.GetFiles(textureDirectoryAbsolute, "*.png",
                SearchOption.AllDirectories);

            //Create a variable to keep track of progress for the progress bar
            float progress = 0f;

            //Update the import settings of each Texture
            foreach (string texturePathAbsolute in texturePathsAbsolute)
            {
                //Display the progress bar
                EditorUtility.DisplayProgressBar("Setting Pivots in " + textureDirectoryAbsolute, texturePathAbsolute,
                    progress);

                string assetPath = Utilities.ConvertAbsolutePathToAssetPath(texturePathAbsolute);

                TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                TextureImporterSettings settings = new TextureImporterSettings();
                textureImporter.ReadTextureSettings(settings);
                settings.spriteAlignment = (int) spriteAlignment;
                textureImporter.SetTextureSettings(settings);
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);

                //Increment the progress
                progress += 1/(float) texturePathsAbsolute.Length;
            }

            EditorUtility.ClearProgressBar();
        }
    }
}