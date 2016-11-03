using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using StevenUniverse.FanGame.StrategyMap.UI;

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
        /// Buffer to hold character actions.
        /// </summary>
        List<CharacterAction> actionBuffer_ = new List<CharacterAction>();

        /// <summary>
        /// Called whenever a player clicks on a unit.
        /// </summary>
        void OnSelected( MapCharacter character )
        {
            // What to do when a unit is first selected
            if( currentlySelected_ != character )
            {
                currentlySelected_ = character;

                if (character.Paused_ || !units_.Contains(character) )
                    return;

                actionBuffer_.Clear();
                character.GetActions(actionBuffer_);

                // When a cahracter is first selected and they are able to move, immediately enter move prompt.
                for (int i = 0; i < actionBuffer_.Count; ++i)
                {
                    var action = actionBuffer_[i];
                    var move = action as MoveAction;
                    if (move != null)
                    {
                        move.Execute();
                        return;
                    }
                }

                // If the character doesn't have a move action, just show the Context UI.
                CharacterActionsUI.Show(character);

                //Debug.LogFormat("{0} has selected {1}", name, character.name);
            }
            // What to do if a unit is clicked again after it's already been selected
            else if( currentlySelected_ == character )
            {
                //Debug.LogFormat("{0} has re-selected {1}", name, character.name);
            }

        }

        

        void Update()
        {
            if( Input.GetKeyDown(KeyCode.Escape) )
            {
                HighlightGrid.Clear();
                CharacterActionsUI.Hide();
            }


        }

        /// <summary>
        /// Player tick function. The players turn will be over when this returns.
        /// </summary>
        public override IEnumerator Tick()
        {
            while( true )
            {

                bool allActed = true;

                for( int i = 0; i < units_.Count; ++i )
                {
                    if (units_[i].Paused_ == false)
                        allActed = false;
                }

                if (allActed)
                    break;

                yield return null;
            }
            
        }

    }
}
