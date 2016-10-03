using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using StevenUniverse.FanGame.Extensions;
using StevenUniverse.FanGame.Overworld;
using StevenUniverse.FanGame.Overworld.Instances;
using StevenUniverse.FanGame.OverworldEditor;
using StevenUniverse.FanGame.Util;
using System.Linq;

namespace StevenUniverse.FanGameEditor.Tools
{
    public static class OverworldTools
    {
        //Overworld
        [MenuItem("Tools/SUFanGame/Overworld/Update Sorting Order %#u")]
        public static void UpdateSortingOrder()
        {
            TileInstanceEditor[] tileInstanceEditors = Utilities.GetObjectsInScene<TileInstanceEditor>();

            foreach (TileInstanceEditor tileInstanceEditor in tileInstanceEditors)
            {
                tileInstanceEditor.UpdateSortingOrder();
            }

            SceneView.RepaintAll();
        }

        [MenuItem("Tools/SUFanGame/Overworld/Regenerate ALL Ghosts")]
        public static void RegenerateAllGhosts()
        {
            //Verify that the user wants to perform this time consuming operation
            if (
                !EditorUtility.DisplayDialog("Regenerate ALL Ghosts",
                    "This tool will Regenerate the Ghosts for ALL Chunks. This could take a very long time. Are you sure you wish to proceed?",
                    "Yes", "Cancel"))
            {
                ToolUtilities.CancelOperation("Operation canceled by user");
            }

            RegenerateGhosts(false);
        }

        [MenuItem("Tools/SUFanGame/Overworld/Regenerate CHANGED Ghosts")]
        public static void RegenerateChangedGhosts()
        {
            //Verify that the user wants to perform this time consuming operation
            if (
                !EditorUtility.DisplayDialog("Regenerate CHANGED Ghosts",
                    "This tool will Regenerate the Ghosts for CHANGED Chunks. This could take a very long time. Are you sure you wish to proceed?",
                    "Yes", "Cancel"))
            {
                ToolUtilities.CancelOperation("Operation canceled by user");
            }

            RegenerateGhosts(true);
        }

        public static void RegenerateGhosts(bool onlyRegenerateChangedChunks)
        {
            //Clear all JSON caches because this tool needs to reload Json information
            ToolUtilities.ClearAllJsonCaches();

            //Create a variable to keep track of progress for the progress bar
            float progress = 0f;
            //Display the progress bar
            EditorUtility.DisplayProgressBar("Regenerating Ghosts", "Initializing...", progress);

            //Cache the ghost directory
            string ghostOutputDirectoryAbsolutePath = Application.dataPath + "/Resources/Textures/Ghosts";
            //Clear the ghost directory if ALL chunks should be regenerated
            if (!onlyRegenerateChangedChunks)
            {
                Utilities.ClearDirectory(ghostOutputDirectoryAbsolutePath);
            }

            //Cache the Chunk directory
            string chunkDirectoryAbsolutePath = Utilities.ExternalDataPath + "/Chunks";
            //Load all the chunks into a list
            string[] chunkAbsolutePaths = Directory.GetFiles(chunkDirectoryAbsolutePath, "*.json",
                SearchOption.AllDirectories);
            List<Chunk> chunks = new List<Chunk>();
            foreach (string chunkAbsolutePath in chunkAbsolutePaths)
            {
                //Display the progress bar
                EditorUtility.DisplayProgressBar("Regenerating Ghosts", "Loading Chunks...", progress);

                //Load the Chunk
                string chunkAppDataPath = Utilities.ConvertAbsolutePathToAppDataPath(chunkAbsolutePath);
                Chunk chunk = Chunk.GetChunk(chunkAppDataPath);

                //If only changed Chunks should be regenerated, check if the current Chunk has been changed before adding it to the list of Chunks to regenerate
                if (onlyRegenerateChangedChunks)
                {
                    if (chunk.Changed)
                    {
                        Debug.Log(chunk.Name);
                        chunks.Add(chunk);
                    }
                }
                //Otherwise, add the Chunk without checking
                else
                {
                    chunks.Add(chunk);
                }

                //Increment the progress
                progress += 1/(float) chunkAbsolutePaths.Length;
            }

            //Reset the progress
            progress = 0f;

            //Generate the Ghosts
            foreach (Chunk chunk in chunks)
            {
                //Display the progress bar
                EditorUtility.DisplayProgressBar("Regenerating Ghosts", "Rendering Ghosts...", progress);

                Texture2D ghost = chunk.GenerateFlattenedTexture();
                byte[] ghostBytes = ghost.EncodeToPNG();

                string ghostOutputAbsolutePath = ghostOutputDirectoryAbsolutePath + "/" +
                                                 chunk.AppDataPath.Replace("Chunks/", "") + ".png";
                string ghostOutputParentDirectoryAbsolutePath =
                    Utilities.ConvertPathToParentDirectoryPath(ghostOutputAbsolutePath);

                if (!Directory.Exists(ghostOutputParentDirectoryAbsolutePath))
                {
                    Directory.CreateDirectory(ghostOutputParentDirectoryAbsolutePath);
                }

                Debug.Log(ghostOutputAbsolutePath);
                File.WriteAllBytes(ghostOutputAbsolutePath, ghostBytes);

                //Update the changed status of the Chunk to reflect that it now matches its current ghost
                chunk.Changed = false;
                chunk.Save();

                //Increment the progress
                progress += 1/(float) chunks.Count;
            }

            EditorUtility.ClearProgressBar();

            //Optimize the Ghost Textures
            string ghostTextureAbsoluteDirectoryPath = Application.dataPath + "/Resources/Textures/Ghosts";
            OtherTools.SetMaxTextureSize(ghostTextureAbsoluteDirectoryPath, 512);
        }

        [MenuItem("Tools/SUFanGame/Overworld/Place Ghosts")]
        public static void PlaceGhosts()
        {
            //Clear all JSON caches because this tool needs to reload Json information
            ToolUtilities.ClearAllJsonCaches();

            //Create a variable to keep track of progress for the progress bar
            float progress = 0f;
            //Display the progress bar
            EditorUtility.DisplayProgressBar("Placing Ghosts", "Initializing...", progress);

            //Create a GameObject to hold the generated Ghosts
            GameObject ghostParent = new GameObject(string.Format("Ghosts ({0})", Utilities.GetDateTimeStamp()));

            string currentEditorSceneName = GetCurrentEditorSceneName();
            string currentSceneGhostDirectoryAbsolutePath = Application.dataPath + "/Resources/Textures/Ghosts/" +
                                                            currentEditorSceneName;

            //Debug.Log(currentSceneGhostDirectoryAbsolutePath);
            if (!Directory.Exists(currentSceneGhostDirectoryAbsolutePath))
            {
                ToolUtilities.CancelOperation("The current scene has no ghost folder!");
            }

            //Get an array of the ghosts
            string[] ghostAbsolutePaths = Directory.GetFiles(currentSceneGhostDirectoryAbsolutePath, "*.png",
                SearchOption.AllDirectories);

            foreach (string ghostAbsolutePath in ghostAbsolutePaths)
            {
                //Display the progress bar
                EditorUtility.DisplayProgressBar("Placing Ghosts", ghostAbsolutePath, progress);

                //Trim the absolute path
                string ghostAbsolutePathTrimmed = Utilities.RemoveFileExtension(ghostAbsolutePath);
                //Get the asset path
                string ghostAssetPath = Utilities.ConvertAbsolutePathToAssetPath(ghostAbsolutePathTrimmed);
                //Get the resource path
                string ghostResourcePath = Utilities.ConvertAssetPathToResourcePath(ghostAssetPath);
                //Get the app data path
                string ghostChunkAppDataPath = ghostResourcePath.Replace("Textures/Ghosts", "Chunks");

                string ghostName = Utilities.ConvertFilePathToFileName(ghostResourcePath);
                if (NameIsChunkName(ghostName))
                {
                    //Load the ghost's Sprite
                    Sprite ghostSprite = Resources.Load<Sprite>(ghostResourcePath);

                    //Create a GameObject to store the ghost
                    GameObject ghostGameObject = new GameObject(ghostName);
                    ghostGameObject.transform.SetParent(ghostParent.transform);

                    //Add a SpriteRenderer component to the ghost GameObject with the ghosts's Sprite
                    SpriteRenderer ghostSpriteRenderer = ghostGameObject.AddComponent<SpriteRenderer>();
                    ghostSpriteRenderer.sprite = ghostSprite;

                    Chunk ghostChunk = Chunk.GetChunk(ghostChunkAppDataPath);
                    ChunkRenderer ghostChunkRenderer = ChunkRenderer.AddChunkRenderer(ghostGameObject, true, ghostChunk);
                    ghostChunkRenderer.UpdatePivotPoint();

                    Vector2 chunkCoordinates =
                        OverworldTools.GetCoordinatesFromChunkName(ghostChunkRenderer.SourceChunk.Name);
                    //Set the sorting order to the negative of the y coordinate, putting ghosts in front on top
                    ghostChunkRenderer.gameObject.GetComponent<SpriteRenderer>().sortingOrder =
                        -(int) chunkCoordinates.y;

                    //Remove the ChunkRenderer component now that is has been used
                    GameObject.DestroyImmediate(ghostChunkRenderer);
                }
                else
                {
                    Debug.Log(string.Format("Skipped {0} because it did not have a valid chunk name!", ghostName));
                }

                //Increment the progress
                progress += 1/(float) ghostAbsolutePaths.Length;
            }
            EditorUtility.ClearProgressBar();
        }

        [MenuItem("Tools/SUFanGame/Overworld/Load Selected Ghost Chunks")]
        public static void LoadSelectedGhostChunks()
        {
            //Clear all JSON caches because this tool needs to reload Json information
            ToolUtilities.ClearAllJsonCaches();

            //Create a variable to keep track of progress for the progress bar
            float progress = 0f;
            //Display the progress bar
            EditorUtility.DisplayProgressBar("Loading Selected Ghost Chunks", "Initializing...", progress);

            GameObject[] selectedGameObjects = Selection.gameObjects;

            if (selectedGameObjects.Length < 1)
            {
                ToolUtilities.CancelOperation("Please select at least one GameObject!");
            }

            foreach (GameObject selectedGameObject in selectedGameObjects)
            {
                string selectedChunkName = selectedGameObject.name;

                //Display the progress bar
                EditorUtility.DisplayProgressBar("Placing Ghosts", selectedChunkName, progress);

                //TODO, also add non-functional "ghost" script to ghosts for extra caution
                if (!NameIsChunkName(selectedChunkName))
                {
                    ToolUtilities.CancelOperation(
                        "The selected GameObject's name was not formatted properly for a Chunk.");
                }

                string chunkAppDataPath = string.Format("{0}/{1}/{1}", GetChunkAppDataDirectoryForCurrentEditorScene(),
                    selectedChunkName);

                //TODO what happens if this is null?
                Chunk selectedChunk = Chunk.GetChunk(chunkAppDataPath);

                LoadChunk(selectedChunk);

                //Increment the progress
                progress += 1/(float) selectedGameObjects.Length;
            }

            EditorUtility.ClearProgressBar();
        }

        private static string GetCurrentEditorSceneName()
        {
            return EditorSceneManager.GetActiveScene().name;
        }

        private static string GetChunkAppDataDirectoryForCurrentEditorScene()
        {
            return "Chunks/" + GetCurrentEditorSceneName();
        }

        private static bool NameIsChunkName(string name)
        {
            bool nameIsChunkName = true;

            //Run the GetCoordinatesFromChunkName function. If an exception is thrown, the name isn't a chunk name.
            try
            {
                GetCoordinatesFromChunkName(name);
            }
            catch (UnityException)
            {
                nameIsChunkName = false;
            }

            return nameIsChunkName;
        }

        [MenuItem("Tools/SUFanGame/Overworld/Save ALL Open Chunks")]
        public static bool SaveAllOpenChunks()
        {
            //Verify that the user wants to perform this time consuming operation
            if (
                !EditorUtility.DisplayDialog("Save ALL Open Chunks",
                    "This tool will save ALL open Chunks. This could take a very long time. Are you sure you wish to proceed?",
                    "Yes", "Cancel"))
            {
                return false;
            }

            //Sort the InstanceEditors into Chunks
            GameObject sortParent = SortAllInstanceEditorsIntoChunks();

            foreach (Transform child in sortParent.transform)
            {
                SaveChunk(child.gameObject);
                //SaveChunk(child.name);
            }

            return true;
        }

        [MenuItem("Tools/SUFanGame/Overworld/Sort All InstanceEditors Into Chunks")]
        public static GameObject SortAllInstanceEditorsIntoChunks()
        {
            //Creat a parent GameObject to hold the sorted Markers
            GameObject sortParent = new GameObject(string.Format("Sorted ({0})", Utilities.GetDateTimeStamp()));

            //Cache the root InstanceEditors for sorting
            InstanceEditor[] rootInstanceEditors = GetRootInstanceEditors(Utilities.GetObjectsInScene<InstanceEditor>());

            //Create a CoordinatedList for holding the sorted Markers
            Dictionary<int, Dictionary<int, List<InstanceEditor>>> sortedInstanceEditors =
                new Dictionary<int, Dictionary<int, List<InstanceEditor>>>();
            //Sort the Markers
            foreach (InstanceEditor instanceEditor in rootInstanceEditors)
            {
                Vector3 instancEditorePos = instanceEditor.transform.position;
                int instanceEditorPivotX = Utilities.FloorToNearestMultiple(instancEditorePos.x, Utilities.CHUNK_WIDTH);
                int instanceEditorPivotY = Utilities.FloorToNearestMultiple(instancEditorePos.y, Utilities.CHUNK_HEIGHT);

                //Add a key for the pivot x if there isn't one
                if (!sortedInstanceEditors.ContainsKey(instanceEditorPivotX))
                {
                    sortedInstanceEditors.Add(instanceEditorPivotX, new Dictionary<int, List<InstanceEditor>>());
                }
                //Add a key for the pivot y if there isn't one
                if (!sortedInstanceEditors[instanceEditorPivotX].ContainsKey(instanceEditorPivotY))
                {
                    sortedInstanceEditors[instanceEditorPivotX].Add(instanceEditorPivotY, new List<InstanceEditor>());
                }
                //Add the marker
                sortedInstanceEditors[instanceEditorPivotX][instanceEditorPivotY].Add(instanceEditor);
            }

            foreach (int x in sortedInstanceEditors.Keys)
            {
                foreach (int y in sortedInstanceEditors[x].Keys)
                {
                    GameObject groupParent = new GameObject(string.Format("{0},{1}", x, y));
                    groupParent.transform.SetParent(sortParent.transform);
                    foreach (InstanceEditor instanceEditor in sortedInstanceEditors[x][y])
                    {
                        instanceEditor.transform.SetParent(groupParent.transform);
                    }
                    SortChildInstanceEditors(groupParent);
                }
            }

            return sortParent;
        }

        private static InstanceEditor[] GetRootInstanceEditors(InstanceEditor[] instanceEditors)
        {
            List<InstanceEditor> rootInstanceEditors = new List<InstanceEditor>();

            foreach (InstanceEditor instanceEditor in instanceEditors)
            {
                //Check if an InstanceEditor is a root InstanceEditor by determining if it either has no parent or if its parent does not have another InstanceEditor component
                Transform parent = instanceEditor.transform.parent;
                if (parent == null || parent.GetComponent<InstanceEditor>() == null)
                {
                    rootInstanceEditors.Add(instanceEditor);
                }
            }

            return rootInstanceEditors.ToArray();
        }

        private static Vector2 GetCoordinatesFromChunkName(string chunkName)
        {
            //Keep track of wether or not the operation is still valid, assume it's invalid until success
            bool validOperation = false;
            //Store the gotten coordinates
            Vector2 coordinates = Vector2.zero;

            //Get the coordinates from the chunk name
            string[] parts = chunkName.Split(',');
            if (parts.Length == 2)
            {
                int pivotX;
                int pivotY;
                if (int.TryParse(parts[0], out pivotX) && int.TryParse(parts[1], out pivotY))
                {
                    bool pivotXIsValid = Utilities.FloorToNearestMultiple(pivotX, Utilities.CHUNK_WIDTH) == pivotX;
                    bool pivotYIsValid = Utilities.FloorToNearestMultiple(pivotY, Utilities.CHUNK_HEIGHT) == pivotY;
                    if (pivotXIsValid && pivotYIsValid)
                    {
                        coordinates = new Vector2(pivotX, pivotY);
                        validOperation = true;
                    }
                }
            }

            if (!validOperation)
            {
                throw new UnityException("Selected GameObject did not match the naming requirements!");
            }
            else
            {
                return coordinates;
            }
        }

        private static List<string> sstuff = new List<string>();

        private static void LoadChunk(Chunk chunkToLoad)
        {
            //Create an empty parent object to store all the loaded Instances
            GameObject chunkParent = new GameObject(chunkToLoad.Name);

            //Create a variable to keep track of progress for the progress bar
            float progress = 0f;

            //Load all the instances
            foreach (Instance instance in chunkToLoad.AllInstances)
            {
                string templateAppDataPath = instance.TemplateAppDataPath;
                string editorInstanceAssetPath =
                    Utilities.ConvertTemplateAppDataPathToMirroringEditorInstanceAssetPath(templateAppDataPath);
                GameObject editorInstancePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(editorInstanceAssetPath);

                //Display the progress bar
                EditorUtility.DisplayProgressBar("Loading Instances", editorInstancePrefab.name, progress);

                //Instantiate an instance of the Instance's prefab
                GameObject editorInstance = (GameObject) PrefabUtility.InstantiatePrefab(editorInstancePrefab);
                editorInstance.transform.SetParent(chunkParent.transform);
                editorInstance.transform.position = instance.Position;

                //Get the InstanceEditor
                InstanceEditor instanceEditor = editorInstance.GetComponent<InstanceEditor>();

                //Verify the InstanceEditor component
                if (instanceEditor == null)
                {
                    ToolUtilities.CancelOperation(
                        "Instantiated EditorInstance did not have an InstanceEditor component!");
                }

                string DEBUGtemplateAppDataPath = instanceEditor.Instance.TemplateAppDataPath;
                if (!sstuff.Contains(instanceEditor.Instance.TemplateAppDataPath))
                {
                    sstuff.Add(DEBUGtemplateAppDataPath);

                    if (sstuff.Count == 76)
                    {
                        sstuff = sstuff.OrderBy(q => q).ToList();

                        foreach (string s in sstuff)
                        {
                            Debug.Log(s);
                        }

                        sstuff.Clear();
                    }
                }

                if (instanceEditor is TileInstanceEditor)
                {
                    TileInstanceEditor tileInstanceEditor = instanceEditor as TileInstanceEditor;
                    TileInstance tileInstance = instance as TileInstance;

                    tileInstanceEditor.TileInstance = tileInstance;
                }
                else if (instanceEditor is GroupInstanceEditor)
                {
                    GroupInstanceEditor groupInstanceEditor = instanceEditor as GroupInstanceEditor;
                    GroupInstance groupInstance = instance as GroupInstance;

                    groupInstanceEditor.GroupInstance = groupInstance;
                }
                else
                {
                    ToolUtilities.CancelOperation("Invalid InstanceEditor type!");
                }

                //Increment the progress
                progress += 1/(float) chunkToLoad.TileInstances.Length;
            }

            //Sort by name
            SortChildInstanceEditors(chunkParent);

            UpdateSortingOrder();

            EditorUtility.ClearProgressBar();
        }

        [MenuItem("Tools/SUFanGame/Overworld/Load All Chunks For Current Scene")]
        public static void LoadAllChunksForCurrentScene()
        {
            //Verify that the user wants to perform this time consuming operation
            if (
                !EditorUtility.DisplayDialog("Load All Chunks For Current Scene",
                    "This tool will load ALL Chunk for the current scene. This could take a very long time. Are you sure you wish to proceed?",
                    "Yes", "Cancel"))
            {
                return;
            }

            foreach (Chunk chunk in GetChunksForCurrentEditorScene())
            {
                LoadChunk(chunk);
            }
        }

        private static void SaveChunk(GameObject root)
        {
            string currentChunkAbsolutePath = string.Format("{0}/{1}/{2}.json",
                GetChunkDirectoryAbsolutePathForCurrentScene(), root.name, root.name);

            //If there is already a chunk here, make a backup copy
            if (File.Exists(currentChunkAbsolutePath))
            {
                //Get the backup output absolute path
                string backupOutputAbsolutePath =
                    currentChunkAbsolutePath.Replace("/Chunks/", "/Chunks Backup/")
                        .Replace(".json", " Backup " + Utilities.GetDateTimeStamp() + ".json");

                //If the backup output directoy doesn't exist, make it
                string backupOutputParentDirectoryAbsolutePath =
                    Utilities.ConvertPathToParentDirectoryPath(backupOutputAbsolutePath);
                if (!Directory.Exists(backupOutputParentDirectoryAbsolutePath))
                {
                    Directory.CreateDirectory(backupOutputParentDirectoryAbsolutePath);
                }

                File.Copy(currentChunkAbsolutePath, backupOutputAbsolutePath);
            }

            InstanceEditor[] rootInstanceEditors = GetRootInstanceEditors(root.GetComponentsInChildren<InstanceEditor>());
            List<TileInstance> tileInstances = new List<TileInstance>();
            List<GroupInstance> groupInstances = new List<GroupInstance>();

            foreach (InstanceEditor instanceEditor in rootInstanceEditors)
            {
                if (instanceEditor is TileInstanceEditor)
                {
                    TileInstanceEditor tileInstanceEditor = (TileInstanceEditor) instanceEditor;
                    tileInstances.Add(tileInstanceEditor.TileInstance);
                }
                else if (instanceEditor is GroupInstanceEditor)
                {
                    GroupInstanceEditor groupInstanceEditor = (GroupInstanceEditor) instanceEditor;
                    groupInstances.Add(groupInstanceEditor.GroupInstance);
                }
                else
                {
                    ToolUtilities.CancelOperation("Invalid instance editor type!");
                }
            }

            Chunk outputChunk = new Chunk(tileInstances.ToArray(), groupInstances.ToArray());
            outputChunk.Changed = true;

            outputChunk.AppDataPath = Utilities.ConvertAbsolutePathToAppDataPath(currentChunkAbsolutePath);
            outputChunk.Save();
        }

        private static string GetChunkDirectoryAbsolutePathForCurrentScene()
        {
            return Utilities.ConvertAppDataPathToAbsolutePath(GetChunkAppDataDirectoryForCurrentEditorScene());
        }

        private static Chunk[] GetChunksForCurrentEditorScene()
        {
            //Clear all JSON caches because this tool needs to reload Json information
            ToolUtilities.ClearAllJsonCaches();

            //Get the absolute path to the Chunk directory for the current editor scene
            string chunkDirectoryAbsolutePathForCurrentEditorScene = GetChunkDirectoryAbsolutePathForCurrentScene();

            //Create a List to hold the found Chunks for the current editor scene
            List<Chunk> chunksForCurrentEditorScene = new List<Chunk>();

            //Get the absolute paths to all chunks
            string[] sceneChunkAbsolutePaths = Directory.GetFiles(chunkDirectoryAbsolutePathForCurrentEditorScene,
                "*.json", SearchOption.AllDirectories);
            //Get all the scene Chunks
            foreach (string sceneChunkPathAbsolute in sceneChunkAbsolutePaths)
            {
                //Create an AppData version of the scene chunk absolute path
                string sceneChunkAppDataPath = Utilities.ConvertAbsolutePathToAppDataPath(sceneChunkPathAbsolute);
                //Load the chunk and add it to the list of chunks
                chunksForCurrentEditorScene.Add(Chunk.GetChunk(sceneChunkAppDataPath));
            }

            return chunksForCurrentEditorScene.ToArray();
        }

        /*
        private static Chunk GetChunkForCurrentEditorScene(string targetChunkName)
        {
            foreach (Chunk chunk in GetChunksForCurrentEditorScene())
            {
                if (targetChunkName == chunk.Name )
                {
                    return chunk;
                }
            }
            return null;
        }*/

        private static void SortChildInstanceEditors(GameObject root)
        {
            InstanceEditor[] instanceEditors = root.GetComponentsInJustChildren<InstanceEditor>();

            //Sort the InstanceEditors by Name
            SortedDictionary<string, SortedDictionary<int, List<GameObject>>> sortedInstanceEditors =
                new SortedDictionary<string, SortedDictionary<int, List<GameObject>>>();
            foreach (InstanceEditor instanceEditor in instanceEditors)
            {
                //Cache the InstanceEditor's Instance
                Instance instance = instanceEditor.Instance;

                //Get the Template's directory name
                string templateDirectoryName =
                    Utilities.ConvertDirectoryPathToDirectoryName(
                        Utilities.ConvertPathToParentDirectoryPath(instance.TemplateAppDataPath));
                //Create a place for objects with this directory name if there isn't one
                if (!sortedInstanceEditors.ContainsKey(templateDirectoryName))
                {
                    sortedInstanceEditors.Add(templateDirectoryName, new SortedDictionary<int, List<GameObject>>());
                }
                //Create a place for objects with this name at this elevation if there isn't one
                if (!sortedInstanceEditors[templateDirectoryName].ContainsKey(instance.Elevation))
                {
                    sortedInstanceEditors[templateDirectoryName].Add(instance.Elevation, new List<GameObject>());
                }
                //Add the GameObject at its place
                sortedInstanceEditors[templateDirectoryName][instance.Elevation].Add(instanceEditor.gameObject);
            }

            //Organize all created GameObjects
            foreach (string s in sortedInstanceEditors.Keys)
            {
                //Create an empty GameObject to store all loaded GameObjects of a common name
                GameObject nameGroup = new GameObject(s);
                nameGroup.transform.SetParent(root.transform);

                foreach (int h in sortedInstanceEditors[s].Keys)
                {
                    //Create an empty GameObject to store all loaded GameObjects of a common name and elevation
                    GameObject elevationGroup = new GameObject("Elevation " + h);
                    elevationGroup.transform.SetParent(nameGroup.transform);

                    foreach (GameObject go in sortedInstanceEditors[s][h])
                    {
                        go.transform.SetParent(elevationGroup.transform);
                    }
                }
            }
        }
    }
}