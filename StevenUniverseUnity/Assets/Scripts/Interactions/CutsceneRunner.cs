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


        public IEnumerator execute()
        {
            if (cutscene == null)
            {
                throw new UnityException("There's no cutscene set!");
            }

            //Debug.Log("CutsceneRunner enabled");
                        
            foreach (Scene curScene in cutscene)
            {
                //Execute each subroutine in order
                if (curScene.CameraChange != null)
                {
                    //add a camera routine
                    yield return ChangeCamera(curScene.CameraChange);
                }
                if (curScene.CharaAction != null)
                {
                    foreach (CutsceneCharacterAction act in curScene.CharaAction)
                    {
                        //start character routine
                        yield return DoAction(act);
                    }
                }
                if (curScene.DialogFileName != null)
                {
                    //start dialog routine
                    yield return StartDialog(curScene);
                }

                Debug.Log("This scene finished");
            }

            cutscene = null; //Done with this cutscene, clear
        }


        public IEnumerator ChangeCamera(CameraChange newCam)
        {
            switch (newCam.CameraType)
            {
                case cameraType.Fixed:
                    Debug.Log("Camera changed to Fixed " + newCam.NewX + "," + newCam.NewY);
                    break;
                case cameraType.Follow:
                    Debug.Log("Camera changed to Follow "+ newCam.Target);
                    break;
            }
            
            yield return new WaitForSeconds(.8f);
            //yield return new WaitWhile(() => !Input.GetKeyDown(KeyCode.Space));
        }


        public IEnumerator DoAction(CutsceneCharacterAction act)
        {
                //In this set-up actions are done sequentially and cannot happen simultaneously
                switch (act.ActType)
                {
                    case actionType.Attack:
                        Debug.Log(act.Name + " attacked " + act.Target);
                        break;
                    case actionType.Move:
                        Debug.Log(act.Name + " moved to " + act.NewX + "," + act.NewY);
                        break;
                    case actionType.ExitMap:
                        Debug.Log(act.Name + " exited the map");
                        break;
                    case actionType.EnterMap:
                        Debug.Log(act.Name + " entered the map");
                        break;
                }

            yield return new WaitForSeconds(.8f);
        }


        public IEnumerator StartDialog(Scene curScene)
        {
            Debug.Log("Started dialog: " + curScene.DialogFileName);

            supportNode.Dialog = SupportLoader.ImportSupport(curScene.DialogFileName);
            supportNode.DestroyOnEnd = curScene.DestroyDialogOnEnd;

            yield return supportNode.DoDialog();
        }
    }
}
