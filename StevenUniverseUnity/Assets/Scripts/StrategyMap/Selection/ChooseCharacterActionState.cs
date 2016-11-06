using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.StrategyMap;
using StevenUniverse.FanGame.StrategyMap.UI;

namespace StevenUniverse.FanGame.Util.Logic.States
{
    public class ChooseCharacterActionState : State
    {
        MapCharacter actor_;

        public ChooseCharacterActionState( StateMachine machine, MapCharacter actor ) : base( machine )
        {
            actor_ = actor;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            CharacterActionsUI.Show(actor_);
        }

        public override void OnExit()
        {
            base.OnExit();

            CharacterActionsUI.Hide();
        }

        public override IEnumerator Tick()
        {
            if( Input.GetButtonDown("Cancel") )
            {
                machine_.Pop();
            }

            yield return null;
        }
    }
}
