using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.Util.Logic.States;
using SUGame.StrategyMap;
using SUGame.StrategyMap.Players;

namespace SUGame.StrategyMap.Characters.Actions.UIStates
{


    public class StrategyMapIdleUIState : State
    {
        public override void OnCharacterSelected(StrategyPlayer player, MapCharacter character)
        {
            if (!character.Paused_ && character.OwningPlayer == player && character.OwningPlayer.CurrentlyActing_)
            {
                var moveAction = character.GetAction<MoveAction>();
                if (moveAction != null)
                {
                    Machine.Push(new ChooseMoveUIState(character, moveAction));
                }
                else
                {
                    Machine.Push(new ChooseActionUIState(character));
                }
            }
        }
    }
}
