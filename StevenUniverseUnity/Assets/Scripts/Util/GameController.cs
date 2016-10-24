//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.SceneManagement;
//using StevenUniverse.FanGame.Extensions;
//using StevenUniverse.FanGame.Overworld;
//using StevenUniverse.FanGame.Overworld.Instances;
//using StevenUniverse.FanGame.Overworld.Templates;
//using StevenUniverse.FanGame.UI;

//namespace StevenUniverse.FanGame.Util
//{
//    public class GameController : MonoBehaviour
//    {
//        //Static
//        private static GameController instance = null;

//        public static GameController Instance
//        {
//            get { return instance; }
//        }

//        //Instance
//        //UI
//        private GameObject uiParent;
//        //UI Controllers
//        private EventSystem eventSystem;
//        private TitleUIController titleUIController;
//        private DialogUIController dialogUIController;
//        private GameUIController gameUIController;
//        private ProgressBarUIController progressBarUIController;

//        //Scene
//        private string currentScene = "Title";


//        //Characters
//        private GameObject characterParent;

//        public GameObject CharacterParent
//        {
//            get
//            {
//                if (characterParent == null)
//                {
//                    characterParent = new GameObject("Character");
//                }

//                return characterParent;
//            }
//        }

//        //Chunks
//        private GameObject chunkParent;
//        private List<ChunkRenderer> activeChunkRenderers = new List<ChunkRenderer>();


//        //Thread constants
//        private string tempChunkLayerPath = "";

//        public string TempChunkLayerPath
//        {
//            get
//            {
//                if (tempChunkLayerPath == "")
//                {
//                    tempChunkLayerPath = Utilities.ExternalDataPath + "/ChunksTemp";
//                    if (!Directory.Exists(tempChunkLayerPath))
//                    {
//                        Directory.CreateDirectory(tempChunkLayerPath);
//                    }
//                }

//                return tempChunkLayerPath;
//            }
//        }

//        void Awake()
//        {
//            //Set the Singleton Instance
//            if (Instance == null)
//            {
//                instance = this;
//            }
//            else
//            {
//                throw new UnityException("Error: there can only be one GameController");
//            }

//            DontDestroyOnLoad(transform.gameObject);

//            Utilities.ClearDirectory(TempChunkLayerPath);
//        }

//        // Use this for initialization
//        void Start()
//        {
//            //Find the UI Parent GameObject
//            uiParent = gameObject.FindChildWithName("UI");
//            //Find the UI Controllers
//            eventSystem = uiParent.FindChildWithName("EventSystem").GetComponent<EventSystem>();
//            titleUIController = uiParent.FindChildWithName("TitleUI").GetComponent<TitleUIController>();
//            dialogUIController = uiParent.FindChildWithName("DialogUI").GetComponent<DialogUIController>();
//            gameUIController = uiParent.FindChildWithName("GameUI").GetComponent<GameUIController>();
//            progressBarUIController =
//                uiParent.FindChildWithName("ProgressBarUI").GetComponent<ProgressBarUIController>();

//            //Disable all UI Controllers
//            DisableAllUIControllers();

//            //Find the Chunks Parent GameObject
//            chunkParent = gameObject.FindChildWithName("Chunks");

//            //Find the Player
//            GameObject player =
//                (GameObject)
//                Instantiate(Resources.Load<GameObject>("Prefabs/_Entities/Player/Player"), Vector3.zero,
//                    Quaternion.identity);

//            //TO-DO get a real cursor camera
//            //mapCursor = new MapCursor();

//            titleUIController.gameObject.SetActive(true);
//        }

//        // Update is called once per frame
//        void Update()
//        {
//            // Nothing!!
//        }


//        public void RegisterActiveChunkRenderer(ChunkRenderer chunkRenderer)
//        {
//            activeChunkRenderers.Add(chunkRenderer);
//        }

//        public void DeregisterActiveChunkRenderer(ChunkRenderer chunkRenderer)
//        {
//            activeChunkRenderers.Remove(chunkRenderer);
//        }

//        //UI Management
//        //Event System
//        public EventSystem EventSystem
//        {
//            get { return eventSystem; }
//        }

//        public void ToggleEventSystem()
//        {
//            ToggleEventSystem(!eventSystem.gameObject.activeSelf);
//        }

//        public void ToggleEventSystem(bool active)
//        {
//            eventSystem.gameObject.SetActive(active);
//        }

//        //Title UI
//        public TitleUIController TitleUIController
//        {
//            get { return titleUIController; }
//        }

//        public void ToggleTitleUI()
//        {
//            ToggleTitleUI(!titleUIController.gameObject.activeSelf);
//        }

//        public void ToggleTitleUI(bool active)
//        {
//            titleUIController.gameObject.SetActive(active);
//        }

//        //Dialog UI
//        public DialogUIController DialogUIController
//        {
//            get { return dialogUIController; }
//        }

//        public void ToggleDialogUI()
//        {
//            ToggleDialogUI(!dialogUIController.gameObject.activeSelf);
//        }

//        public void ToggleDialogUI(bool active)
//        {
//            dialogUIController.gameObject.SetActive(active);
//        }

//        //Game UI
//        public GameUIController GameUIController
//        {
//            get { return gameUIController; }
//        }

//        public void ToggleGameUI()
//        {
//            ToggleGameUI(!gameUIController.gameObject.activeSelf);
//        }

//        public void ToggleGameUI(bool active)
//        {
//            gameUIController.gameObject.SetActive(active);
//        }

//        //Progress Bar UI
//        public ProgressBarUIController ProgressBarUIController
//        {
//            get { return progressBarUIController; }
//        }

//        public void ToggleProgressBarUI()
//        {
//            ToggleProgressBarUI(!progressBarUIController.gameObject.activeSelf);
//        }

//        public void ToggleProgressBarUI(bool active)
//        {
//            progressBarUIController.gameObject.SetActive(active);
//        }


//        private bool controlDisabledOverride = false;

        
//        public void DisableAllUIControllers()
//        {
//            ToggleTitleUI(false);
//            ToggleDialogUI(false);
//            ToggleGameUI(false);
//            ToggleProgressBarUI(false);
//        }

//        public string CurrentScene
//        {
//            get { return currentScene; }
//            set
//            {
//                //Cache the last scene
//                string lastScene = CurrentScene;

//                currentScene = value;
//                titleUIController.gameObject.SetActive(false);

//                //Start loading the area around the player
//                bool sceneChanged = lastScene != CurrentScene;
//                StartLoadAreaAroundPlayer(sceneChanged, true);
//            }
//        }

//        public TileInstance[] GetTileInstancesAtPosition(Vector3 pos)
//        {
//            return GetTileInstancesAtPosition(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
//        }

//        public TileInstance[] GetTileInstancesAtPosition(int x, int y)
//        {
//            List<TileInstance> tileInstancesAtPosition = new List<TileInstance>();

//            foreach (ChunkRenderer activeChunkRenderer in activeChunkRenderers)
//            {
//                tileInstancesAtPosition.AddRange(activeChunkRenderer.GetTileInstancesAtPosition(x, y));
//            }

//            return tileInstancesAtPosition.ToArray();
//        }

//        public TileInstance[] GetTileInstancesAtPositionAndElevation(Vector3 tilePosition, int tileElevation)
//        {
//            //Cache the Tiles at the requested position
//            TileInstance[] tileInstancesAtPosition = GetTileInstancesAtPosition(tilePosition);

//            List<TileInstance> tileInstancesAtPositionAndElevation = new List<TileInstance>();

//            foreach (TileInstance ti in tileInstancesAtPosition)
//            {
//                TileTemplate tt = ti.TileTemplate;
//                if (tt.TileLayer != TileTemplate.Layer.Get("CharacterBody") && ti.Elevation == tileElevation)
//                {
//                    tileInstancesAtPositionAndElevation.Add(ti);
//                }
//            }

//            return tileInstancesAtPositionAndElevation.ToArray();
//        }

//        public void StartNewGame()
//        {
//            DisableAllUIControllers();
//            //TO-DO, make proper camera attached to MapCursor
//            //MapCursor.ChunkRenderer.SetVisibility(false);
//            SceneManager.LoadScene("Intro");
//        }

//        public void ContinueFile()
//        {
//            DisableAllUIControllers();
//            //CurrentPlayer.MoveTo(CurrentPlayer.SourcePlayer.SavedWarpPoint);
//            //CurrentPlayer.PlayerCameraController.SetNight();
//        }

//        public void StartLoadAreaAroundPlayer(bool changeScene, bool showProgressBars)
//        {
//            StartCoroutine(LoadAreaAroundPlayer(changeScene, showProgressBars));
//        }

//        private IEnumerator LoadAreaAroundPlayer(bool changeScene, bool showProgressBars)
//        {
//            if (changeScene)
//            {
//                //Destroy the existing ChunkRenderers
//                foreach (ChunkRenderer activeChunkRenderer in activeChunkRenderers)
//                {
//                    // Need to change focus to a cursor camera

//                    ////If the ChunkRenderer is not a Player
//                    //if (activeChunkRenderer.GetComponent<PlayerDriver>() == null)
//                    //{
//                    //    //Debug.Log("destroying chunk!");
//                    //    Destroy(activeChunkRenderer.gameObject);
//                    //}
//                }

//                //Load the scene
//                SceneManager.LoadScene(CurrentScene);
//            }

//            if (showProgressBars)
//            {
//                //Lock player control until the end
//                controlDisabledOverride = true;

//                //Activate the progress bar
//                ToggleProgressBarUI(true);
//                //Set the progress bar's title
//                progressBarUIController.Title = "Loading chunks...";
//                progressBarUIController.Info = "Initializing...";
//                progressBarUIController.Progress = 0f;
//            }

//            //Floor the Player's position
//            Vector3 flooredPosition = Utilities.FloorToNearestMultiple(mapCursor.TruePosition, Utilities.CHUNK_WIDTH, Utilities.CHUNK_HEIGHT);

//            List<string> chunkNamesToLoad = new List<string>();
//            for (int i = -1; i <= 1; i++)
//            {
//                for (int j = -1; j <= 1; j++)
//                {
//                    //Find the target chunk's name
//                    int x = i*Utilities.CHUNK_WIDTH;
//                    int y = j*Utilities.CHUNK_HEIGHT;
//                    chunkNamesToLoad.Add(string.Format("{0},{1}", flooredPosition.x + x, flooredPosition.y + y));
//                }
//            }

//            //Determine which chunk names and chunk renderers to keep
//            foreach (Transform child in chunkParent.transform)
//            {
//                //Start out by assuming the child is no longer neccesary
//                bool destroyThisChild = true;

//                //Loop backwards through all of the chunk names to load
//                for (int i = chunkNamesToLoad.Count - 1; i >= 0; i--)
//                {
//                    //If the child is one of the chunks that should be loaded...
//                    if (child.name == chunkNamesToLoad[i])
//                    {
//                        //Don't load the chunk because it is already loaded
//                        chunkNamesToLoad.RemoveAt(i);
//                        //Don't destroy this child because it is still needed
//                        destroyThisChild = false;
//                    }
//                }

//                //If the child was not determined to be needed, destroy it
//                if (destroyThisChild)
//                {
//                    Destroy(child.gameObject);
//                }
//            }

//            //Find the chunks that are within one chunk of the player
//            List<Chunk> chunksToLoad = new List<Chunk>();
//            foreach (string chunkNameToLoad in chunkNamesToLoad)
//            {
//                //TODO make this work for nested scenes
//                string chunkAppDataPath = string.Format("Chunks/{0}/{1}/{1}", CurrentScene, chunkNameToLoad);
//                Chunk chunkAtPosition = Chunk.GetChunk(chunkAppDataPath);

//                if (chunkAtPosition != null)
//                {
//                    chunksToLoad.Add(chunkAtPosition);
//                }
//            }

//            if (showProgressBars)
//            {
//                //Reset the progress
//                progressBarUIController.Progress = 0f;
//            }

//            //Load the Chunk Renderers
//            foreach (Chunk chunkToLoad in chunksToLoad)
//            {
//                //Update the progess bar's info
//                progressBarUIController.Info = string.Format("Setting up Chunk {0}...", chunkToLoad.Name);

//                //Create a Chunk Renderer of the Chunk
//                GameObject chunkObject = new GameObject(chunkToLoad.Name);
//                chunkObject.transform.SetParent(chunkParent.transform);
//                ChunkRenderer.AddChunkRenderer(chunkObject, true, chunkToLoad);

//                if (showProgressBars)
//                {
//                    //Increment the Progress
//                    progressBarUIController.Progress += 1/(float) chunksToLoad.Count;
//                }
//                yield return null;
//            }

//            //Load the CharacterInstances for the scene if changing to a new scene
//            if (changeScene)
//            {
//                string characterDirectoryAbsolutePath = Utilities.ExternalDataPath + "/Characters";
//                string[] characterAbsolutePaths = Directory.GetFiles(characterDirectoryAbsolutePath, "*.json", SearchOption.AllDirectories);

//                List<string> characterAppDataPaths = new List<string>();
//                foreach (string characterAbsolutePath in characterAbsolutePaths)
//                {
//                    characterAppDataPaths.Add(Utilities.ConvertAbsolutePathToAppDataPath(characterAbsolutePath));
//                }

//                foreach (string characterAppDataPath in characterAppDataPaths)
//                {
//                    //TO-DO remake the loading utility
//                    //Character loadedCharacter = Character.GetCharacter(characterAppDataPath);
//                    //loadedCharacter.AttemptLoad();
//                }
//            }

//            if (showProgressBars)
//            {
//                //Deactivate the progress bar
//                progressBarUIController.Progress = 0f;
//                ToggleProgressBarUI(false);

//                //Unlock player control
//                controlDisabledOverride = false;
//            }
//        }
//    }
//}