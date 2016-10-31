using UnityEngine;
using System.IO;
using System;
using StevenUniverse.FanGame.Util;

namespace StevenUniverse.FanGame.Interactions
{

    /*
     * Cutscenes are made up of multiple Scenes. A Scene can have only one of each:
       - A set of characters that do one action each
       - A dialog
       - A Camera movement
       
     * In a scene, Characters can:
       - move
       - enter map
       - run off map
       - attack
       
     */

    public class CutsceneLoader
    {

        public static Scene[] ImportSupport(string cutsceneName)
        {

            //Not sure how assets are handled in build, so check if this is the correct way to search for a file
            string absolutePath = Utilities.ConvertAssetPathToAbsolutePath("Assets/Resources/Cutscene/" + cutsceneName + ".json");
            if (!File.Exists(absolutePath))
            {
                throw new UnityException("Cutscene " + cutsceneName + " was not found.");
            }
            
            Scene[] parsedLines = JsonHelper.FromJson<Scene>(File.ReadAllText(absolutePath));

            return parsedLines;
        }

    }

    /// <summary>
    /// Individual Scene made up of 3 components: camera changes, character actions, and dialog.
    /// </summary>
    [System.Serializable]
    public class Scene
    {
        [SerializeField]
        private CameraChange cameraChange;
        [SerializeField]
        private CutsceneCharacterAction[] charaAction = { }; //Default values make it easier to test
        [SerializeField]
        private ScriptLine[] dialogParsed = { };
        [SerializeField]
        private string dialogFileName = "n/a";
        [SerializeField]
        private bool dontDestroyDialogOnEnd = false;

        //For debugging purposes
        public override string ToString()
        {
            
            string s = "cam: ";

            if (cameraChange != null) {
                s += cameraChange;
            }

            s += "\ndialog: " + dialogFileName;

            foreach (CutsceneCharacterAction act in charaAction)
            {
                s += "\naction: " + act;
            }

            return s;
        }

    }

    /// <summary>
    /// Changes the Camera's location or target during a cutscene
    /// </summary>
    [System.Serializable]
    public class CameraChange
    {
        [SerializeField]
        public enum changeType
        {
            FOLLOW,
            FIXED
        }

        [SerializeField]
        private string type; //Need to get json to cast to enum
        [SerializeField]
        private string target; //Only FOLLOW has a target
        [SerializeField]
        private int newX; //Only FIXED has an x/y, will need to accomodate Sark's ToMove()
        [SerializeField]
        private int newY;
        
        public CameraChange(string type)
        {
            this.type = type;
        }

        public override string ToString()
        {
            return type;//.ToString();
        }

        public changeType getCamType() {
            return (changeType)Enum.Parse(typeof(changeType), type, true);
        }

        public string Type { get; set; }
        public string Target { get; set; }
        public int NewX { get; set; }
        public int NewY { get; set; }

    }

    /// <summary>
    /// Actions that a character can take during a cutscene.
    /// </summary>
    [System.Serializable]
    public class CutsceneCharacterAction
    {
        [SerializeField]
        public enum actionType
        {
            MOVE,
            ATTACK,
            EXITMAP,
            ENTERMAP,
        }

        [SerializeField]
        private string type; //Need to get json to cast to enum
        [SerializeField]
        private string name; //Character that is doing the action
        [SerializeField]
        private string target; //Only ATTACK has a target. Must be an identifiable name and attackable
        [SerializeField]
        private int newX;
        [SerializeField]
        private int newY;
        [SerializeField]
        private int edgeX; //Only EXIT/ENTER has an edge point for despawn/spawn
        [SerializeField]
        private int edgeY;

        public CutsceneCharacterAction(string type, string name)
        {
            this.type = type;
            this.name = name;
        }

        public override string ToString()
        {
            return name + type;//.ToString();
        }

        public actionType getCamType()
        {
            return (actionType)Enum.Parse(typeof(actionType), type, true);
        }

        public string Type { get; set; }
        public string Name { get; set; }
        public string Target { get; set; }
        public int NewX { get; set; }
        public int NewY { get; set; }

    }
}
