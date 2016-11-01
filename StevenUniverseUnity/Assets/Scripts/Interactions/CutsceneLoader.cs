using UnityEngine;
using System.IO;
using System;
using FullSerializer;
using StevenUniverse.FanGame.Util;

namespace StevenUniverse.FanGame.Interactions
{
    
    public class CutsceneLoader
    {

        public static Scene[] ImportCutscene(string cutsceneName)
        {

            //Not sure how assets are handled in build, so check if this is the correct way to search for a file
            string absolutePath = Utilities.ConvertAssetPathToAbsolutePath("Assets/Resources/Cutscenes/" + cutsceneName + ".json");
            if (!File.Exists(absolutePath))
            {
                throw new UnityException("Cutscene " + cutsceneName + " was not found.");
            }
            
            Scene[] parsedLines = JsonHelper.Deserialize<Scene>(File.ReadAllText(absolutePath));

            return parsedLines;
        }

    }

    /// <summary>
    /// Individual Scene made up of 3 components: camera changes, character actions, and dialog.
    /// A scene can only have one of each.
    /// </summary>
    [System.Serializable]
    public class Scene
    {
        [SerializeField]
        private CameraChange cameraChange = null;
        [SerializeField]
        private CutsceneCharacterAction[] charaAction = null;
        [SerializeField]
        private string dialogFileName;
        [SerializeField]
        private bool destroyDialogOnEnd;

        public CameraChange CameraChange
        {
            get { return cameraChange; }
            set { cameraChange = value; }
        }
        public CutsceneCharacterAction[] CharaAction
        {
            get { return charaAction; }
            set { charaAction = value; }
        }
        public string DialogFileName
        {
            get { return dialogFileName; }
            set { dialogFileName = value; }
        }
        public bool DestroyDialogOnEnd
        {
            get { return destroyDialogOnEnd; }
            set { destroyDialogOnEnd = value; }
        }

        public Scene(
            CameraChange camChange = null,
            CutsceneCharacterAction charaAction = null,
            string dialogFile = null,
            bool destroyDialog = false)
        {

        }

        //For debugging purposes
        public override string ToString()
        {
            string s = "cam: ";

            if (cameraChange != null) {
                s += cameraChange;
            }

            s += "\ndialog: ";

            if (dialogFileName != null)
            {
                s += dialogFileName;
            }

            if (charaAction != null)
            {
                foreach (CutsceneCharacterAction act in charaAction)
                {
                    s += "\naction: " + act;
                }
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
        private cameraType type; //Need to get json to cast to enum
        [SerializeField]
        private string target; //Only FOLLOW has a target
        [SerializeField]
        private int newX; //Only FIXED has an x/y, will need to accomodate Sark's ToMove()
        [SerializeField]
        private int newY;
        
        public CameraChange(cameraType type, string target = null)
        {
            this.type = type;
            this.target = target;
            //newX newY needs to be changed
        }

        public override string ToString()
        {
            return type.ToString();
        }

        public cameraType CameraType
        {
            get { return type; }
            set { type = value; }
        }

        public string Target
        {
            get { return target; }
            set { target = value; }
        }

        // need to replace?
        public int NewX { get; set; }
        public int NewY { get; set; }

    }

    /// <summary>
    /// Actions that a character can take during a cutscene such as:
    ///  - move
    ///  - enter map
    ///  - exit map
    ///  - attack
    /// </summary>
    [System.Serializable]
    public class CutsceneCharacterAction
    {
        [SerializeField]
        private actionType type; //needs to be validated as actionType
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

        public CutsceneCharacterAction(actionType type, string name)
        {
            this.type = type;
            this.name = name;
        }

        public override string ToString()
        {
            return name + type;
        }

        public actionType ActType
        {
            get { return type; }
            set { type = value; }
        }

        public string Target
        {
            get { return target; }
            set { target = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        //need to replace?
        public int NewX { get; set; }
        public int NewY { get; set; }

    }

    public enum actionType
    {
        Move,
        Attack,
        ExitMap,
        EnterMap,
    }


    public enum cameraType
    {
        Follow,
        Fixed
    }
}
