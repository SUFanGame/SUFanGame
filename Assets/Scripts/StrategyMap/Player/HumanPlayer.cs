using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using SUGame.StrategyMap.UI;
using SUGame.Util.Logic.States;

namespace SUGame.StrategyMap.Players
{
    public class HumanPlayer : StrategyPlayer
    {

        /// <summary>
        /// The unit that is currently selected by this player.
        /// </summary>
        //MapCharacter currentlySelected_ = null;
        
        /// <summary>
        /// State machine that handles player inputs and manages the state of the ui.
        /// Input will be forwarded from the player to the machine to whichever state is currently
        /// active.
        /// </summary>
        StateMachine stateMachine_;

        void OnEnable()
        {
            stateMachine_ = new StateMachine(this);

            var grid = GameObject.FindObjectOfType<Grid>();

            if (grid == null)
                return;

            // Register for click events.
            grid.OnNodeClicked_ += OnNodeClicked;
            MapCharacter.OnClicked_ += OnUnitClicked;
        }

        void OnDisable()
        {
            var grid = GameObject.FindObjectOfType<Grid>();

            if (grid == null)
                return;

            // Unregister click events.
            grid.OnNodeClicked_ -= OnNodeClicked;
            MapCharacter.OnClicked_ -= OnUnitClicked;
        }


        /// <summary>
        /// Buffer to hold character actions.
        /// </summary>
        //List<CharacterAction> actionBuffer_ = new List<CharacterAction>();

        public static HumanPlayer Instance { get; private set; }


        void Start()
        {
            Instance = this;
            StartCoroutine(stateMachine_.TickRoutine());
        }

        void OnNodeClicked( Node node )
        {
            //Debug.LogFormat("Node {0} clicked", node);
            stateMachine_.OnPointSelected(node);
        }
        
        void OnUnitClicked( MapCharacter character )
        {
            //Debug.LogFormat("Character {0} clicked", character.name );
            stateMachine_.OnUnitSelected(character);
        }



        void Update()
        {
            // Handle keyboard events.
            if( Input.GetButtonDown("Submit") )
            {
                stateMachine_.OnAccept();
            }

            if( Input.GetButtonDown("Cancel") )
            {
                stateMachine_.OnCancel();
            }
        }

        public StateMachine StateMachine_ { get { return stateMachine_; } }

        ///// <summary>
        ///// Called whenever a player clicks on a unit.
        ///// </summary>
        //void OnSelected( MapCharacter character )
        //{
        //    // TODO : Make a "SelectCharacter" state which is the default? Selection actions could get forwarded to that and it
        //    // can decide what to do from there.
        //    if( selectionState_.StateCount == 0 
        //        && CurrentlyActing_
        //        && units_.Contains(character) 
        //        && character.Paused_ == false )
        //    {
        //        if (character.HasAction<MoveAction>())
        //            selectionState_.Push(new CharacterMoveUIState(character));
        //        else
        //            selectionState_.Push(new ChooseCharacterActionState(character));
        //    }

        //}

    }
}
