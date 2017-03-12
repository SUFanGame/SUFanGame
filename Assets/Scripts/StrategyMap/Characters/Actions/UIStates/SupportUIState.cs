using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.Util.Logic.States;
using SUGame.StrategyMap.Players;
using SUGame.Interactions;

namespace SUGame.StrategyMap.Characters.Actions.UIStates
{
    class SupportUIState : State
    {
        MapCharacter initiator_;
        MapCharacter receiver_;
        // System.Func<MapCharacter, IEnumerator> actionCallback_;

        public SupportUIState(
            MapCharacter initiator,
            MapCharacter receiver
            //System.Func<MapCharacter, IEnumerator> actionCallback 
            )
        {
            initiator_ = initiator;
            receiver_ = receiver;
            //actionCallback_ = actionCallback;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            //Debug.Log("Entering support state");

            string supportFileName = initiator_.name +"-"+ receiver_.name;

            SupportPanel.Dialog = SupportLoader.ImportSupport(supportFileName);
            //supportCanvas_.gameObject.SetActive(true);

            SupportPanel.Instance.StartCoroutine(SupportPanel.DoDialogWithCanvas());
            initiator_.Paused_ = true;

        }


        public override void OnExit()
        {
            //Debug.Log("Exiting support state");
            base.OnExit();
            

        }
        

        public override IEnumerator Tick()
        {
            if (SupportPanel.Dialog == null)
            {

                OnExit();
                //yield return actionCallback_.Invoke(defender_);
                Machine.Clear();
                initiator_.Paused_ = true;
            }


            yield return base.Tick();
        }

        public override void OnCancelInput(StrategyPlayer player)
        {
            
        }

        /// <summary>
        /// Wait until the state completes.
        /// </summary>
        //public IEnumerator WaitForAnimations()
        //{
        //    var wait = new WaitForSeconds(.01f);
        //    while (true)
        //    {
        //        if (dialogFinished_ == false)
        //        {
        //            yield return wait;
        //            continue;
        //        }
        //        yield break;
        //    }
        //}

    }
}