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

        //public ActingState CurrentActingState { get { return state_; } }

        Animator animator_;

        public Animator Animator_ { get { return animator_; } }

        /// <summary>
        /// Event handler for when a character is selected with the mouse.
        /// </summary>
        static public System.Action<MapCharacter> OnSelected_;

        /// <summary>
        /// Event handler for when this character a "deselected". Note that in unity terms
        /// an object is deselected if the mouse is clicked outside of the object OR if ANY UI
        /// element is clicked.
        /// </summary>
        static public System.Action<MapCharacter> OnDeselected_;

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

        /// <summary>
        /// Returns true if the character is capable of the given action.
        /// </summary>
        public bool HasAction<T>() where T : CharacterAction
        {
            for( int i = 0; i < actions_.Count; ++i )
            {
                if (actions_[i] is T)
                    return true;
            }
            return false;
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

        static Color pausedColor_ = new Color(.32f, .32f, .32f);

        bool paused_ = false;
        public bool Paused_
        {
            get
            {
                return paused_;
            }
            set
            {
                paused_ = value;

                if( paused_ )
                {
                    animator_.speed = 0f;
                    renderer_.color = pausedColor_;
                }
                else
                {
                    animator_.speed = 1f;
                    renderer_.color = Color.white;
                }
            }
        }

        void Awake()
        {
            animator_ = GetComponentInChildren<Animator>();
            
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
            // Selection events get forwarded to HumanPlayer
            if (OnSelected_ != null)
                OnSelected_(this);
            //eventData.selectedObject = gameObject;
            //Debug.LogFormat("CharacterClicked");
            //eventData.selectedObject = gameObject;
            //if (CurrentActingState != ActingState.IDLE)
            //    return;

            //// Perform selection action when a character is FIRST selected.
            //if (eventData.selectedObject == null || eventData.selectedObject != gameObject )
            //{
            //    eventData.selectedObject = gameObject;
            //}
            //// If a character is selected again, but it's already BEEN selected previously, just pop up the context UI.
            //else
            //{
            //    CharacterActionsUI.Show(this);
            //}
        }

        public void OnSelect(BaseEventData eventData)
        {
            // Forward character selection event to listeners
            //if (OnSelected_ != null)
            //    OnSelected_(this);
            //Debug.LogFormat("{0} selected!", name);
            //CharacterActionsUI.Show(this);
            //if (actions_ == null)
            //    return;

            //// When a cahracter is first selected and they are able to move, immediately enter move prompt.
            //for( int i = 0; i < actions_.Count; ++i )
            //{
            //    var action = actions_[i];
            //    var move = action as MoveAction;
            //    if (move != null)
            //    {
            //        move.Execute();
            //        return;
            //    }
            //}

            //// If the character doesn't have a move action, just show the Context UI.
            //CharacterActionsUI.Show(this);
        }

        /// <summary>
        /// Update the character's sorting order based on their current world position.
        /// </summary>
        public void UpdateSortingOrder()
        {
            renderer_.sortingOrder = GridPosition.z * 100 + 90;
        }

        public void UpdateSortingOrder(int newZ )
        {
            var pos = transform.position;
            pos.z = newZ;
            transform.position = pos;
            UpdateSortingOrder();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            // Forward selection event to listeners
            if (OnDeselected_ != null)
                OnDeselected_(this);
            //if( eventData.selectedObject.)
            //Debug.LogFormat("{0} deselected", name);
            //HighlightGrid.Clear();
        }

        /// <summary>
        /// Current state of the character
        /// </summary>
        public enum ActingState
        {
            MOVING,
            IDLE,

        };

        /// <summary>
        /// Coroutine that moves the character smoothly from it's current position to the 
        /// given position. Speed determined by <see cref="tilesMovedPerSecond_"/>.
        /// Note this doesn't use any kind of pathfinding - pathfinding calls upon this to
        /// move characters between nodes - see <see cref="CharacterUtility.MoveTo(MapCharacter, IntVector3)"/>.
        /// </summary>
        /// <param name="end">Where the character will end up.</param>
        public IEnumerator MoveTo( IntVector3 end )
        {
            float startTime = Time.time;
            float dist = Vector3.Distance(transform.position, (Vector3)end );
            var start = transform.position;

            if (end.z > start.z)
            {
                UpdateSortingOrder(end.z);
            }

            while ( transform.position != (Vector3)end )
            {
                float distCovered = (Time.time - startTime) * tilesMovedPerSecond_;
                float t = distCovered / dist; 
                transform.position = Vector3.Lerp(start, (Vector3)end, t);
                yield return null;
            }

            if( end.z < start.z )
            {
                UpdateSortingOrder(end.z);
            }
        }


    }

}