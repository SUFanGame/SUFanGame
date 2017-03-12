using UnityEngine;
using System.IO;
using SUGame.Util;

namespace SUGame.Interactions
{
    
    public class CutsceneLoader
    {

        public static Scene[] ImportCutscene(string cutsceneName)
        {
            // Note you must omit the file extension
            var asset = Resources.Load<TextAsset>("Cutscenes/" + cutsceneName);
            if ( asset == null )
            {
                throw new UnityException("Cutscene " + cutsceneName + " was not found.");
            }
            
            Scene[] parsedLines = JsonHelper.Deserialize<Scene>(asset.text);

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
        public CameraChange CameraChange { get; set; }
        [SerializeField]
        public CutsceneCharacterAction[] CharaAction { get; set; }
        [SerializeField]
        public string DialogFileName { get; set; }
        [SerializeField]
        public bool DestroyDialogOnEnd { get; set; }
        

        public Scene(
            CameraChange camChange = null,
            CutsceneCharacterAction[] charaAction = null,
            string dialogFile = null,
            bool destroyDialog = true)
        {
            CameraChange = camChange;
            CharaAction = charaAction;
            DialogFileName = dialogFile;
            DestroyDialogOnEnd = destroyDialog;
        }

        //For debugging purposes
        public override string ToString()
        {
            string s = "cam: ";

            if (CameraChange != null) {
                s += CameraChange;
            }

            s += "\ndialog: ";

            if (DialogFileName != null)
            {
                s += DialogFileName;
            }

            if (CharaAction != null)
            {
                foreach (CutsceneCharacterAction act in CharaAction)
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
        public cameraType CameraType { get; set; } //Need to get json to cast to enum
        [SerializeField]
        public string Target { get; set; } //Only FOLLOW has a target
        [SerializeField]
        public int NewX { get; set; } //Only FIXED has an x/y, will need to accomodate Sark's ToMove()
        [SerializeField]
        public int NewY { get; set; }
        
        public CameraChange(cameraType type)
        {
            this.CameraType = type;
        }
        public CameraChange(cameraType type, string target) : this (type)
        {
            Target = target;
        }

        public CameraChange(cameraType type, int newX, int newY) : this(type)
        {
            NewX = newX;
            NewY = newY;
        }


        public override string ToString()
        {
            return CameraType.ToString();
        }

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
        public actionType type { get; set; } //needs to be validated as actionType
        [SerializeField]
        public string name { get; set; } //Character that is doing the action
        [SerializeField]
        public string target { get; set; } //Only ATTACK has a target. Must be an identifiable name and attackable
        [SerializeField]
        public int newX { get; set; }
        [SerializeField]
        public int newY { get; set; }
        [SerializeField]
        public int edgeX { get; set; } //Only EXIT/ENTER has an edge point for despawn/spawn
        [SerializeField]
        public int edgeY { get; set; }

        public CutsceneCharacterAction(actionType type, string name)
        {
            this.type = type;
            this.name = name;
        }

        public CutsceneCharacterAction(actionType type, string name, string Target) : this(type, name)
        {
            this.target = Target;
        }

        public CutsceneCharacterAction(actionType type, string name, int newX, int newY) : this(type, name)
        {
            this.newX = newX;
            this.newY = newY;
        }

        public override string ToString()
        {
            return name + type;
        }

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
