using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using StevenUniverse.FanGame.StrategyMap;
using System.Linq;
using System;
using StevenUniverse.FanGame.StrategyMap.UI;
using StevenUniverse.FanGame.Characters;

// Just a note about unity's built in Selection Handlers - they require that the camera have a "Physics Raycaster"
// and that an "EventSystem" is in the scene (GameObject->UI->EventSystem). Any objects to be selected
// must also have an appropriate collider. The handlers WILL receieve notifications from chid objects.

namespace StevenUniverse.FanGame.StrategyMap
{
    /// <summary>
    /// A character in the battle map.
    /// </summary>
    [SelectionBase]
    public class MapCharacter : MonoBehaviour, IPointerClickHandler, ISelectHandler, IDeselectHandler
    {
         
        /// <summary>
        /// Character data, to be loaded in once this is instantiated.
        /// </summary>
        [SerializeField]
        CharacterData characterData_;
        public CharacterData Data { get { return characterData_; } }


        #region possiblecharacterdata
        public float tilesMovedPerSecond_ = 3f;

        public int moveRange_ = 5;
        #endregion

        SpriteRenderer renderer_;

        /// <summary>
        /// List of actions this character is capable of. Actions are components added to the character
        /// through the editor. Actions will need to be mirrored in JSON in some way, but we also want to keep them as components
        /// on the unity side. Maybe character data just references actions by name? "Move", "Attack", etc
        /// </summary>
        List<CharacterAction> actions_ = null;

        ActingState state_ = ActingState.IDLE;

        public ActingState CurrentActingState { get { return state_; } }


        /// <summary>
        /// Populate the given buffer with valid actions this character can perform.
        /// </summary>
        public void GetActions( List<CharacterAction> buffer )
        {
            if (actions_ == null)
                return;

            for( int i = 0; i < actions_.Count; ++i )
            {
                var action = actions_[i];
                if (action.IsUsable())
                    buffer.Add(action);
            }
        }

        public IntVector3 GridPosition
        {
            get
            {
                return (IntVector3)transform.position;
            }
            set
            {
                transform.position = (Vector3)value;
            }
        }

        void Awake()
        {
            renderer_ = GetComponentInChildren<SpriteRenderer>();

            // Populate our list of actions
            var actions = GetComponentsInChildren<CharacterAction>();

            if( actions != null && actions.Length != 0 )
            {
                actions_ = actions.ToList();
            }
            
            UpdateSortingOrder();
        }

        void Start()
        {
            Grid.Instance.OnGridBuilt_ += AddToGrid;
        }

        void OnDestroy()
        {
            if( Grid.Instance != null )
                Grid.Instance.OnGridBuilt_ -= AddToGrid;
        }

        void AddToGrid( Grid grid )
        {
            // Snap our character's z position to the highest point on the grid.
            // Not ideal but for now while we're having to manually place characters in the map this works.
            var pos = transform.position;
            pos.z = grid.GetHeight(new IntVector2(pos.x, pos.y));
            transform.position = pos;

            // Add our character to the grid at it's current position.
            grid.AddObject(GridPosition, this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (CurrentActingState != ActingState.IDLE)
                return;

            eventData.selectedObject = gameObject;
        }

        public void OnSelect(BaseEventData eventData)
        {
            CharacterActionsUI.Show(this);
        }

        /// <summary>
        /// Update the character's sorting order based on their current world position.
        /// </summary>
        public void UpdateSortingOrder()
        {
            renderer_.sortingOrder = GridPosition.z * 100 + 90;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            HighlightGrid.Clear();
        }

        /// <summary>
        /// Current state of the character
        /// </summary>
        public enum ActingState
        {
            MOVING,
            IDLE,
        };

        public IEnumerator MoveTo( IntVector3 end )
        {
            float startTime = Time.time;
            float dist = Vector3.Distance(transform.position, (Vector3)end );
            var start = transform.position;

            while ( transform.position != (Vector3)end )
            {
                float distCovered = (Time.time - startTime) * tilesMovedPerSecond_;
                float t = distCovered / dist; 
                transform.position = Vector3.Lerp(start, (Vector3)end, t);
                yield return null;
            }
        }
    }

}