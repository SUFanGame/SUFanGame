using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using StevenUniverse.FanGame.StrategyMap.UI;
using StevenUniverse.FanGame.Util.Logic.States;

namespace StevenUniverse.FanGame.StrategyMap.Players
{
    public class HumanPlayer : StrategyPlayer
    {

        /// <summary>
        /// The unit that is currently selected by this player.
        /// </summary>
        MapCharacter currentlySelected_ = null;

        Coroutine selectionRoutine_ = null;

        StateMachine selectionState_ = new StateMachine();

        void OnEnable()
        {
            MapCharacter.OnSelected_ += OnSelected;
        }

        void OnDisable()
        {
            MapCharacter.OnSelected_ -= OnSelected;
        }

        /// <summary>
        /// Buffer to hold character actions.
        /// </summary>
        List<CharacterAction> actionBuffer_ = new List<CharacterAction>();


        void Start()
        {
            StartCoroutine(selectionState_.TickRoutine());
        }

        /// <summary>
        /// Called whenever a player clicks on a unit.
        /// </summary>
        void OnSelected( MapCharacter character )
        {
            // TODO : Make a "SelectCharacter" state which is the default. Selection actions can get forwarded to that and it
            // can decide what to do from there.
            if( selectionState_.StateCount == 0 
                && CurrentlyActing_
                && units_.Contains(character) 
                && character.Paused_ == false
                && character.HasAction<MoveAction>() )
            {
                selectionState_.Push(new ChooseMoveState(selectionState_, character));
            }

        }

    }
}
