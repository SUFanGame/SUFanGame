using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using StevenUniverse.FanGame.Overworld.Instances;
using StevenUniverse.FanGame.Overworld.Templates;
using StevenUniverse.FanGame.OverworldEditor;
using StevenUniverse.FanGame.Util;

namespace StevenUniverse.FanGameEditor.Tools
{
    public static class InstanceEditorTools
    {
        //TODO clarify everywhere that ASSET PATHS HAVE EXTENSIONS... or think of a more universal way?
        [MenuItem("Tools/SUFanGame/Instance Editors/Regenerate ALL Tile Instance Editors")]
        public static void RegenerateAllInstanceEditors()
        {
            //Clear all JSON caches because this tool needs to reload Json information
            ToolUtilities.ClearAllJsonCaches();

            //Define the template directory
            string templateDirectory = Utilities.ExternalDataPath + "/Templates";

            //Verify that the user wants to perform this time consuming operation
            if
            (
                !EditorUtility.DisplayDialog
                (
                    "Regenerate All Markers",
                    string.Format(
                        "This tool will DELETE ALL EXISTING MARKERS and regenerate them based on the Templates stored at '{0}'. Are you sure you want to proceed?",
                        templateDirectory),
                    "Yes",
                    "Cancel"
                )
            )
            {
                return;
            }

            //Create a variable to keep track of progress for the progress bar
            float progress = 0f;
            //Display the progress bar
            EditorUtility.DisplayProgressBar("Regenerating ALL Instance Editors", "Deleting existing InstanceEditors...",
                progress);

            //Clear all JSON caches because this tool needs to reload Json information
            ToolUtilities.ClearAllJsonCaches();

            //Delete the existing contents of the directories
            string editorInstancesPrefabDirectoryPathAbsolute = Application.dataPath + "/Editor/Prefabs/EditorInstances";
            Utilities.ClearDirectory(editorInstancesPrefabDirectoryPathAbsolute);
            string editorInstancesTextureDirectoryPathAbsolute = Application.dataPath +
                                                                 "/Editor/Textures/EditorInstances";
            Utilities.ClearDirectory(editorInstancesTextureDirectoryPathAbsolute);

            //Get an array of absolute template paths to all templates
            string[] templateAbsolutePaths = Directory.GetFiles(templateDirectory, "*.json", SearchOption.AllDirectories);

            List<TileTemplate> tileTemplates = new List<TileTemplate>();
            List<GroupTemplate> groupTemplates = new List<GroupTemplate>();

            //Reset the progress
            progress = 0f;
            //Sort the Templates by type
            foreach (string templateAbsolutePath in templateAbsolutePaths)
            {
                string templateAppDataPath = Utilities.ConvertAbsolutePathToAppDataPath(templateAbsolutePath);
                EditorUtility.DisplayProgressBar("Sorting Templates", templateAppDataPath, progress);

                //Check if it's a TileTemplate
                TileTemplate templateAsTileTemplate = TileTemplate.GetTileTemplate(templateAppDataPath);
                if (templateAsTileTemplate != null)
                {
                    tileTemplates.Add(templateAsTileTemplate);
                }
                //Otherwise, check if it's a GroupTemplate
                else
                {
                    GroupTemplate templateAsGroupTemplate = GroupTemplate.GetGroupTemplate(templateAppDataPath);
                    if (templateAsGroupTemplate != null)
                    {
                        groupTemplates.Add(templateAsGroupTemplate);
                    }
                    else
                    {
                        Debug.Log(templateAppDataPath);
                        ToolUtilities.CancelOperation("Couldn't find valid type");
                    }
                }

                progress += 1/(float) templateAbsolutePaths.Length;
            }

            //Regenerate the TileTemplates
            progress = 0f;
            foreach (TileTemplate tileTemplate in tileTemplates)
            {
                EditorUtility.DisplayProgressBar("Regenerating Tile Instance Editors", tileTemplate.Name, progress);
                RegnerateTileInstanceEditor(tileTemplate);
                progress += 1/(float) tileTemplates.Count;
            }

            //Regenerate the GroupTemplates
            progress = 0f;
            foreach (GroupTemplate groupTemplate in groupTemplates)
            {
                EditorUtility.DisplayProgressBar("Regenerating Group Instance Editors", groupTemplate.Name, progress);
                RegenerateGroupInstanceEditor(groupTemplate);
                progress += 1/(float) groupTemplates.Count;
            }

            EditorUtility.ClearProgressBar();
        }

        public static string RegnerateTileInstanceEditor(TileTemplate tileTemplate)
        {
            //Cache the AppData path
            string tileTemplateAppDataPath = tileTemplate.AppDataPath;

            //Get the instance editor asset path
            string instanceEditorAssetPath =
                Utilities.ConvertTemplateAppDataPathToMirroringEditorInstanceAssetPath(tileTemplateAppDataPath);

            //Create the directory containing the marker if it doesn't exist
            string editorInstanceParentDirectoryAssetPath =
                Utilities.ConvertPathToParentDirectoryPath(instanceEditorAssetPath);
            string editorInstanceParentDirectoryAbsolutePath =
                Utilities.ConvertPathToParentDirectoryPath(Application.dataPath) + "/" +
                editorInstanceParentDirectoryAssetPath;
            if (!Directory.Exists(editorInstanceParentDirectoryAbsolutePath))
            {
                Directory.CreateDirectory(editorInstanceParentDirectoryAbsolutePath);
            }

            //Copy the first frame
            //Get the path to the original first frame
            string tileTemplateAbsolutePath = Utilities.ExternalDataPath + "/" + tileTemplateAppDataPath;
            string tileTemplateParentDirectoryAbsolutePath =
                Utilities.ConvertPathToParentDirectoryPath(tileTemplateAbsolutePath);
            string firstFrameName = tileTemplate.AnimationSpriteNames[0];
            string firstFrameAbsolutePath = tileTemplateParentDirectoryAbsolutePath + "/" + firstFrameName + ".png";
            //Create the directory to hold the copy
            string copyOutputParentDirectoryAbsolutePath = editorInstanceParentDirectoryAbsolutePath;
            if (!Directory.Exists(copyOutputParentDirectoryAbsolutePath))
            {
                Directory.CreateDirectory(copyOutputParentDirectoryAbsolutePath);
            }
            //Get the path the location to which the first frame should be copied
            string copyOutputAbsolutePath = copyOutputParentDirectoryAbsolutePath + "/" + firstFrameName + ".png";
            File.Copy(firstFrameAbsolutePath, copyOutputAbsolutePath, true);
            AssetDatabase.Refresh();
            string copyOutputAssetPath = editorInstanceParentDirectoryAssetPath + "/" + firstFrameName + ".png";
            Sprite firstFrame = AssetDatabase.LoadAssetAtPath<Sprite>(copyOutputAssetPath);

            //Create the marker
            GameObject generatedTileInstanceEditorGameObject = new GameObject(tileTemplate.Name);
            TileInstanceEditor generatedTileInstanceEditor =
                generatedTileInstanceEditorGameObject.AddComponent<TileInstanceEditor>();
            generatedTileInstanceEditor.TileInstance = new TileInstance(tileTemplateAppDataPath, 0, 0, 0);
            SpriteRenderer generatedTileInstanceEditorSpriteRenderer =
                generatedTileInstanceEditorGameObject.AddComponent<SpriteRenderer>();
            generatedTileInstanceEditorSpriteRenderer.sprite = firstFrame;
            generatedTileInstanceEditorSpriteRenderer.sortingLayerName = "Overworld";

            PrefabUtility.CreatePrefab(instanceEditorAssetPath, generatedTileInstanceEditorGameObject);
            GameObject.DestroyImmediate(generatedTileInstanceEditorGameObject);

            return instanceEditorAssetPath;
        }

        public static string RegenerateGroupInstanceEditor(GroupTemplate groupTemplate)
        {
            //Cache the AppData path
            string groupTemplateAppDataPath = groupTemplate.AppDataPath;

            //Get the editor instance asset path
            string instanceEditorAssetPath =
                Utilities.ConvertTemplateAppDataPathToMirroringEditorInstanceAssetPath(groupTemplateAppDataPath);

            //Create the directory containing the marker if it doesn't exist
            string editorInstanceParentDirectoryAssetPath =
                Utilities.ConvertPathToParentDirectoryPath(instanceEditorAssetPath);
            string editorInstanceParentDirectoryAbsolutePath =
                Utilities.ConvertPathToParentDirectoryPath(Application.dataPath) + "/" +
                editorInstanceParentDirectoryAssetPath;
            if (!Directory.Exists(editorInstanceParentDirectoryAbsolutePath))
            {
                Directory.CreateDirectory(editorInstanceParentDirectoryAbsolutePath);
            }

            //Create the base object or the prefab to be built on
            GameObject generatedGroupInstanceEditorGameObject = new GameObject(groupTemplate.Name);

            GroupInstanceEditor generatedGroupInstanceEditor =
                generatedGroupInstanceEditorGameObject.AddComponent<GroupInstanceEditor>();
            generatedGroupInstanceEditor.GroupInstance = new GroupInstance(groupTemplateAppDataPath, 0, 0, 0);

            //Build markers for the group's children
            foreach (TileInstance tileInstance in groupTemplate.TileInstances)
            {
                string tileTemplateAppDataPath = tileInstance.TemplateAppDataPath;
                string tileInstanceEditorAssetPath =
                    Utilities.ConvertTemplateAppDataPathToMirroringEditorInstanceAssetPath(tileTemplateAppDataPath);

                GameObject tileInstanceEditorPrefab =
                    AssetDatabase.LoadAssetAtPath<GameObject>(tileInstanceEditorAssetPath);
                GameObject tileInstanceEditorObject =
                    PrefabUtility.InstantiatePrefab(tileInstanceEditorPrefab) as GameObject;
                tileInstanceEditorObject.transform.SetParent(generatedGroupInstanceEditorGameObject.transform);
                tileInstanceEditorObject.transform.localPosition = tileInstance.Position;

                TileInstanceEditor tileInstanceEditor = tileInstanceEditorObject.GetComponent<TileInstanceEditor>();
                tileInstanceEditor.TileInstance = tileInstance;
            }

            PrefabUtility.CreatePrefab(instanceEditorAssetPath, generatedGroupInstanceEditorGameObject);
            GameObject.DestroyImmediate(generatedGroupInstanceEditorGameObject);

            return instanceEditorAssetPath;
        }

        [MenuItem("Tools/SUFanGame/Instance Editors/Attach Tile Instance Editor Sprites")]
        public static void AttachTileInstanceEditorSprites()
        {
            //Clear all JSON caches because this tool needs to reload Json information
            ToolUtilities.ClearAllJsonCaches();

            //Define the template directory
            string templateDirectory = Utilities.ExternalDataPath + "/Templates";

            //Verify that the user wants to perform this time consuming operation
            if
            (
                !EditorUtility.DisplayDialog
                (
                    "Attach TileInstance Editor Sprites",
                    string.Format(
                        "This tool will attach sprites to all of the Tile Instance Editors. This could take a long time. Are you sure you want to proceed?",
                        templateDirectory),
                    "Yes",
                    "Cancel"
                )
            )
            {
                return;
            }

            //Get an array of absolute template paths to all templates
            string[] templateAbsolutePaths = Directory.GetFiles(templateDirectory, "*.json", SearchOption.AllDirectories);

            float progress = 0f;
            foreach (string templateAbsolutePath in templateAbsolutePaths)
            {
                string templateAppDataPath = Utilities.ConvertAbsolutePathToAppDataPath(templateAbsolutePath);

                EditorUtility.DisplayProgressBar("Attaching Sprites", templateAppDataPath, progress);

                //Load the TileTemplate
                TileTemplate tileTemplate = TileTemplate.GetTileTemplate(templateAppDataPath);
                //Skip non-TileTemplates
                if (tileTemplate != null)
                {
                    string editorInstanceAssetPath =
                        Utilities.ConvertTemplateAppDataPathToMirroringEditorInstanceAssetPath(templateAppDataPath);
                    string editorInstanceParentDirectoryAssetPath =
                        Utilities.ConvertPathToParentDirectoryPath(editorInstanceAssetPath);
                    string firstFrameName = tileTemplate.AnimationSpriteNames[0];
                    string firstFrameAssetPath =
                        editorInstanceParentDirectoryAssetPath.Replace("/Prefabs/", "/Textures/") + "/" + firstFrameName +
                        ".png";
                    GameObject loadedGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(editorInstanceAssetPath);
                    SpriteRenderer generatedTileInstanceEditorSpriteRenderer =
                        loadedGameObject.GetComponent<SpriteRenderer>();
                    Sprite firstFrame = AssetDatabase.LoadAssetAtPath<Sprite>(firstFrameAssetPath);
                    generatedTileInstanceEditorSpriteRenderer.sprite = firstFrame;
                }

                progress += 1/(float) templateAbsolutePaths.Length;
            }
            EditorUtility.ClearProgressBar();
        }
    }
}