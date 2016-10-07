using System.IO;
using UnityEditor;
using UnityEngine;
using StevenUniverse.FanGame.Entities;
using StevenUniverse.FanGame.Entities.Customization;
using StevenUniverse.FanGame.Extensions;
using StevenUniverse.FanGame.Interactions;
using StevenUniverse.FanGame.Interactions.Activities;
using StevenUniverse.FanGame.Overworld;
using StevenUniverse.FanGame.Overworld.Templates;
using StevenUniverse.FanGame.Util;
using StevenUniverse.FanGame.Util.Logic;
using System.Collections.Generic;
using StevenUniverse.FanGame.Overworld.Instances;

namespace StevenUniverse.FanGameEditor.Tools
{
    public static class TempTools
    {
        [MenuItem("Tools/SUFanGame/Temp/Test String Split")]
        public static void TestStringSplit()
        {
            string tester = "This<=isa<=test";
            string[] results = tester.Split("asdfg");

            foreach (string result in results)
            {
                Debug.Log(result);
            }
        }

        [MenuItem("Tools/SUFanGame/Temp/Clear Progress Bar")]
        public static void ClearProgressBar()
        {
            EditorUtility.ClearProgressBar();
        }

        [MenuItem("Tools/SUFanGame/Temp/Create Temp Character")]
        public static void CreateTempCharacter()
        {
            Outfit sapphireOutfit = new Outfit("Light Body", "Sapphire Hat", "Calm Eyes", "Sapphire Shirt");

            Character sapphire = new Character
                (
                "Sapphire",
                "Down",
                "Standing",
                sapphireOutfit,

                new Character.CharacterInstance[]
                {
                    new Character.CharacterInstance
                    (
                        new Conditional("true"),
                        new Vector3(-156, 162, 0),
                        0,
                        new Character.CharacterInstance.InteractionStarter[]
                        {
                            new Character.CharacterInstance.InteractionStarter( new Conditional("true"), 5 )
                        }
                    )
                },

                new ActivityBlock[]
                {
                    new ActivityBlock(1, new int[] { 0, 2 }, 2),
                    new ActivityBlock(2, new int[] { 0, 1 }, -1),
                    new ActivityBlock(3, new int[] { 3 }, 4),
                    new ActivityBlock(4, new int[] { 4 }, 6)
                },
                new Branch[]
                {
                    new Branch(5, new Conditional("true"), 3, 4),
                    new Branch(6, new Conditional("true"), 5, 1)
                },

                new DestroyEntity[] { },
                new Dialog[]
                {
                    new Dialog(0, false, "This is a test", "Sapphire"),
                    new Dialog(1, false, "You shouldn't see this!", "Sapphire"),
                    new Dialog(2, false, "And it worked!", "Sapphire"),
                    new Dialog(3, false, "Something something!", "Sapphire"),
                    new Dialog(4, false, "Something else...", "Sapphire")
                },
                new InstantiateGameObject[] { },
                new Movement[] { },
                new SetData[] { },
                new Wait[] { },
                new WaitForBool[] { }
                );

            sapphire.AppDataPath = "Characters/Overworld/Caldera/Sapphire";

            sapphire.Save();
        }

        [MenuItem("Tools/SUFanGame/Temp/Reformat ALL json files")]
        public static void ReformatAllJsonFiles()
        {
            ToolUtilities.ClearAllJsonCaches();

            //Define the template directory
            string jsonDirectory = Utilities.ExternalDataPath;

            //Verify that the user wants to perform this time consuming operation
            if
            (
                !EditorUtility.DisplayDialog
                (
                    "Reformat ALL json files",
                    string.Format(
                        "This tool will reformat ALL json files. This could take a long time. Are you sure you want to proceed?",
                        jsonDirectory),
                    "Yes",
                    "Cancel"
                )
            )
            {
                return;
            }

            //Get an array of absolute paths to all json files
            string[] jsonAbsolutePaths = Directory.GetFiles(jsonDirectory, "*.json", SearchOption.AllDirectories);

            List<string> tileTemplateAppDataPaths = new List<string>();
            List<string> groupTemplateAppDataPaths = new List<string>();
            List<string> chunkAppDataPaths = new List<string>();
            List<string> characterAppDataPaths = new List<string>();

            float progress = 0f;
            foreach (string jsonAbsolutePath in jsonAbsolutePaths)
            {
                string jsonAppDataPath = Utilities.ConvertAbsolutePathToAppDataPath(jsonAbsolutePath);

                EditorUtility.DisplayProgressBar("Sorting json files", jsonAppDataPath, progress);

                if (jsonAppDataPath.Contains("Tiles/"))
                {
                    tileTemplateAppDataPaths.Add(jsonAppDataPath);
                }
                else if (jsonAppDataPath.Contains("Tile Groups/"))
                {
                    groupTemplateAppDataPaths.Add(jsonAppDataPath);
                }
                else if (jsonAppDataPath.Contains("Chunks/"))
                {
                    chunkAppDataPaths.Add(jsonAppDataPath);
                }
                else if (jsonAppDataPath.Contains("Characters/"))
                {
                    characterAppDataPaths.Add(jsonAppDataPath);
                }
                else
                {
                    Debug.Log("Unrecognized directory: " + jsonAppDataPath);
                }

                progress += 1/(float) jsonAbsolutePaths.Length;
            }

            //This is used to find out which tile templates are exclusively used by tile groups
            List<string> tileTemplateAppDataPathsNotUsableIndividually = new List<string>();

            //Reset the progress
            progress = 0f;
            //Reformat group templates
            foreach (string groupTemplateAppDataPath in groupTemplateAppDataPaths)
            {
                EditorUtility.DisplayProgressBar("Refactoring GroupTemplate json files", groupTemplateAppDataPath, progress);

                GroupTemplate loadedGroupTemplate = GroupTemplate.GetGroupTemplate(groupTemplateAppDataPath);
                //loadedGroupTemplate.SerializedType = typeof(GroupTemplate).Name;
                //loadedGroupTemplate.Save();

                foreach (TileInstance childTileInstance in loadedGroupTemplate.TileInstances)
                {
                    string childTileTemplateAppDataPath = childTileInstance.TemplateAppDataPath;

                    if (!tileTemplateAppDataPathsNotUsableIndividually.Contains(childTileTemplateAppDataPath))
                    {
                        tileTemplateAppDataPathsNotUsableIndividually.Add(childTileTemplateAppDataPath);
                    }
                }

                progress += 1 / (float)groupTemplateAppDataPaths.Count;
            }

            //Reset the progress
            progress = 0f;
            //Reformat chunks
            foreach (string chunkAppDataPath in chunkAppDataPaths)
            {
                EditorUtility.DisplayProgressBar("Refactoring Chunk json files", chunkAppDataPath, progress);

                Chunk loadedChunk = Chunk.GetChunk(chunkAppDataPath);
                //loadedChunk.SerializedType = typeof(Chunk).Name;
                //loadedChunk.Save();

                progress += 1 / (float)chunkAppDataPaths.Count;
            }

            //Reset the progress
            progress = 0f;
            //Reformat tile templates
            foreach (string tileTemplateAppDataPath in tileTemplateAppDataPaths)
            {
                EditorUtility.DisplayProgressBar("Refactoring TileTemplate json files", tileTemplateAppDataPath, progress);

                TileTemplate loadedTileTemplate = TileTemplate.GetTileTemplate(tileTemplateAppDataPath);
                //loadedTileTemplate.SerializedType = typeof(TileTemplate).Name;
                loadedTileTemplate.UsableIndividually = !tileTemplateAppDataPathsNotUsableIndividually.Contains(tileTemplateAppDataPath);

                loadedTileTemplate.Save();

                progress += 1 / (float)tileTemplateAppDataPaths.Count;
            }

            //Reset the progress
            progress = 0f;
            //Reformat characters
            foreach (string characterAppDataPath in characterAppDataPaths)
            {
                EditorUtility.DisplayProgressBar("Refactoring Character json files", characterAppDataPath, progress);

                //TODO this

                progress += 1 / (float)tileTemplateAppDataPaths.Count;
            }

            EditorUtility.ClearProgressBar();
        }

        [MenuItem("Tools/SUFanGame/Temp/Count Objects In Scene")]
        public static void CountObjectsInScene()
        {
            GameObject[] gameObjects = GameObject.FindObjectsOfType<GameObject>();
            Debug.Log(gameObjects.Length);
        }

        [MenuItem("Tools/SUFanGame/Temp/Get Selected Object Type")]
        public static void GetSelectedObjectType()
        {
            Debug.Log(Selection.activeObject.GetType().ToString());
        }
    }
}