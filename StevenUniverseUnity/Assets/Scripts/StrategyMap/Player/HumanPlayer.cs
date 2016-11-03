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
                CharacterActionsUI.Hide();

                currentlySelected_ = character;

                // If it's not our turn, the character has already acted or if the character isn't one of our units...
                if ( !CurrentlyActing_ || character.Paused_ || !units_.Contains(character) )
                {
                    // Show a stats panel
                    return;
                }

                StartCoroutine(ShowActions(character));

                //Debug.LogFormat("{0} has selected {1}", name, character.name);
            }
            // What to do if a unit is clicked again after it's already been selected
            else if( currentlySelected_ == character )
            {
                CharacterActionsUI.Show(character);
            }

        }

        // Shows available actions to the player for the given character.
        IEnumerator ShowActions(MapCharacter character)
        {
            actionBuffer_.Clear();
            character.GetActions(actionBuffer_);
            
            for (int i = 0; i < actionBuffer_.Count; ++i)
            {
                var action = actionBuffer_[i];
                var move = action as MoveAction;
                // If a character has a move action...
                if (move != null)
                {
                    var start = character.GridPosition;
                    //Immediately run our move action
                    yield return move.Routine();

                    // If the character hasn't moved then the move action was cancelled 
                    if (character.GridPosition == start)
                    {
                        yield break;
                    }
                }
            }

            // If the character doesn't have a move action, just show the Context UI.
            CharacterActionsUI.Show(character);
        }
        

        //void Update()
        //{
        //    if( Input.GetKeyDown(KeyCode.Escape) )
        //    {
        //        HighlightGrid.Clear();
        //        CharacterActionsUI.Hide();
        //    }


        //}


    }
}
