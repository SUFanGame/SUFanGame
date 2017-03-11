using UnityEngine;
using System.Collections;
using SUGame.Util.Common;
using SUGame.StrategyMap.Characters.Actions.UIStates;
using SUGame.StrategyMap;
using SUGame.Util.Logic.States;
using SUGame.Util;

namespace SUGame.Interactions
{
    public class CutscenePanel : MonoBehaviour
    {
        /* TODO:
         * Add the camera changes depending on the option
         * Add the character movement action
         * Add the character enter/exit action
         * Add the character attack action
         */

        //public SupportPanel supportNode; 
        //Set the support inside the editor, set canvas inside of this component

        static CutscenePanel instance_;
        public static CutscenePanel Instance { get { return instance_; } }

        public Scene[] Cutscene { get; set; }

        public void Awake()
        {
            //Debug.Log("Woke up");
            Cutscene = null;
            instance_ = this;
        }

        public IEnumerator execute()
        {
            if (Cutscene == null)
            {
                throw new UnityException("There's no cutscene set!");
            }

            //Debug.Log("CutsceneRunner enabled");
                        
            foreach (Scene curScene in Cutscene)
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

                //Debug.Log("This scene finished");
            }

            Cutscene = null; //Done with this cutscene, clear
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
            MapCharacter actor_ = GameObject.Find(act.name).GetComponent<MapCharacter>();

            //In this set-up actions are done sequentially and cannot happen simultaneously
            switch (act.type)
            {
                case actionType.Attack:
                    Debug.Log(act.name + " attacked " + act.target);
                    MapCharacter target_ = GameObject.Find(act.target).GetComponent<MapCharacter>();
                    var combat = new CombatUIState(actor_, target_);
                    yield return combat.QuickCombat();
                    break;
                case actionType.Move:
                    Debug.Log(act.name + " moved to " + act.newX + "," + act.newY);
                    yield return CharacterUtility.MoveTo(actor_, new IntVector3(act.newX, act.newY, 0));
                    break;
                case actionType.ExitMap:
                    Debug.Log(act.name + " exited the map");
                    break;
                case actionType.EnterMap:
                    Debug.Log(act.name + " entered the map");
                    break;
            }

            yield return new WaitForSeconds(.8f);
        }


        public IEnumerator StartDialog(Scene curScene)
        {
            Debug.Log("Started dialog: " + curScene.DialogFileName);

            SupportPanel.Dialog = SupportLoader.ImportSupport(curScene.DialogFileName);
            SupportPanel.DestroyOnEnd = curScene.DestroyDialogOnEnd;

            yield return SupportPanel.DoDialogWithCanvas();
        }
    }
}
