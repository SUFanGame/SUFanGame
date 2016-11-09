using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Util.Logic.States;
using StevenUniverse.FanGame.StrategyMap;
using StevenUniverse.FanGame.StrategyMap.Players;

public class StrategyMapIdleUIState : State
{
    public override void OnCharacterSelected(StrategyPlayer player, MapCharacter character)
    {
        if( !character.Paused_ && character.OwningPlayer == player )
        {
            var moveAction = character.GetAction<MoveAction>();
            if( moveAction != null )
            {
                Machine.Push(new ChooseMoveUIState(character, moveAction ) );
            }
            else
            {
                Machine.Push(new ChooseActionUIState(character));
            }
        }
    }
}
