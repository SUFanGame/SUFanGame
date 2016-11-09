using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Util.Logic.States;
using StevenUniverse.FanGame.StrategyMap;
using StevenUniverse.FanGame.StrategyMap.Players;

public class ChooseTargetState : State
{
    public MapCharacter Actor { get; private set; }
    public TargetType TargetType { get; private set; }

    public ChooseTargetState( MapCharacter actor, TargetType targetType )
    {
        Actor = actor;
        this.TargetType = targetType;
    }

    public override void OnCharacterSelected(StrategyPlayer player, MapCharacter target )
    {
    }

    // override 


}
