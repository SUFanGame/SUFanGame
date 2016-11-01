using UnityEngine;
using System.Collections;

namespace StevenUniverse.FanGame.Interactions
{
    public class CutsceneRunner : MonoBehaviour
    {
        public SupportRunner supportNode; //Set the support inside the editor, set canvas inside of this component

        private Scene[] cutscene;

        public Scene[] Cutscene
        {
            set { cutscene = value; }
        }


        public void OnEnable()
        {
            if (cutscene == null)
            {
                throw new UnityException("There's no cutscene set!");
            }

            Debug.Log("CutsceneRunner enabled");
                        
            foreach (Scene curScene in cutscene)
            {
                //Process the scenes first, they're not loaded into coroutines yet
                //Cutscenes are loaded dynamically in this way. Unsure about performance with big cutscenes

                //PROBLEM CODE HERE vvvvv
                if (curScene.CameraChange != null)
                {
                    //start camera routine
                    Debug.Log("Started Camera routine");
                    StartCoroutine(ChangeCamera(curScene.CameraChange));
                }
                if (curScene.CharaAction != null)
                {
                    //start character routine
                    Debug.Log("Started Character routine");
                    foreach (CutsceneCharacterAction act in curScene.CharaAction)
                    {
                        //In this set-up actions are done sequentially and cannot happen simultaneously
                        StartCoroutine(DoAction(act));
                    }
                }
                if (curScene.DialogFileName != null)
                {
                    //start dialog routine
                    Debug.Log("Started dialog: "+curScene.DialogFileName);

                    //supportNode.Dialog = SupportLoader.ImportSupport(curScene.DialogFileName);
                    //supportNode.DontDestroyOnEnd = curScene.DontDestroyDialogOnEnd;
                    //supportNode.enabled = true;
                }

                Debug.Log("This scene finished");
            }

            cutscene = null; //Done with this cutscene, clear
            gameObject.SetActive(false); //disable itself for next scene
        }

        public IEnumerator ChangeCamera(CameraChange newCam)
        {
            switch (newCam.getCamType())
            {
                case cameraType.FIXED:
                    Debug.Log("Camera changed to FIXED " + newCam.NewX + "," + newCam.NewY);
                    break;
                case cameraType.FOLLOW:
                    Debug.Log("Camera changed to FOLLOW "+ newCam.Target);
                    break;
            }
            yield return null;
        }

        public IEnumerator DoAction(CutsceneCharacterAction act)
        {
            switch (act.GetActType())
            {
                case actionType.ATTACK:
                    Debug.Log(act.Name + " attacked " + act.Target);
                    break;
                case actionType.MOVE:
                    Debug.Log(act.Name + " moved to " + act.NewX +"," + act.NewY);
                    break;
                case actionType.EXITMAP:
                    Debug.Log(act.Name + " exited the map");
                    break;
                case actionType.ENTERMAP:
                    Debug.Log(act.Name + " entered the map");
                    break;
            }
            yield return null;
        }
    }
}
