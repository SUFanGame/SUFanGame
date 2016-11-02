using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace StevenUniverse.FanGame.StrategyMap.Players
{
    public class HumanPlayer : StrategyPlayer
    {

        /// <summary>
        /// The unit that is currently selected by this player.
        /// </summary>
        MapCharacter currentlySelected_ = null;

        void OnEnable()
        {
            MapCharacter.OnSelected_ += OnSelected;
        }

        void OnDisable()
        {
            MapCharacter.OnSelected_ -= OnSelected;
        }

        /// <summary>
        /// Called whenever a player clicks on a unit.
        /// </summary>
        void OnSelected( MapCharacter character )
        {
            // What to do when a unit is first selected
            if( currentlySelected_ != character )
            {
                currentlySelected_ = character;
                Debug.LogFormat("{0} has selected {1}", name, character.name);
            }
            // What to do if a unit is clicked again after it's already been selected
            else if( currentlySelected_ == character )
            {
                Debug.LogFormat("{0} has re-selected {1}", name, character.name);
            }

        }

    }
}
