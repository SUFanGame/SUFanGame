using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;
using SUGame.Characters;
using SUGame.StrategyMap.Players;
using SUGame.Util;
using SUGame.StrategyMap.Characters.Actions;
using SUGame.Util.Common;

// Just a note about unity's built in Selection Handlers - they require that the camera have a "Physics Raycaster"
// and that an "EventSystem" is in the scene (GameObject->UI->EventSystem). Any objects to be selected
// must also have an appropriate collider. The handlers WILL receieve notifications from chid objects.

namespace SUGame.StrategyMap
{
    /// <summary>
    /// A character in the battle map.
    /// </summary>
    [SelectionBase]
    public class MapCharacter : MonoBehaviour, IPointerClickHandler
    {
        public StrategyPlayer OwningPlayer { get; private set; }
         
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

        public SpriteRenderer renderer_;

        /// <summary>
        /// List of actions this character is capable of. Actions are components added to the character
        /// through the editor. Actions will need to be mirrored in JSON in some way, but we also want to keep them as components
        /// on the unity side. Maybe character data just references actions by name? "Move", "Attack", etc
        /// </summary>
        List<CharacterAction> actions_ = null;
        

        //public ActingState CurrentActingState { get { return state_; } }

        Animator animator_;

        public Animator Animator_ { get { return animator_; } }

        /// <summary>
        /// Event handler for when a character is clicked on.
        /// </summary>
        static public System.Action<MapCharacter> OnClicked_;


        void Start()
        {
            Grid.Instance.OnGridBuilt_ += AddToGrid;
        }

        void OnDestroy()
        {
            if (Grid.Instance != null)
                Grid.Instance.OnGridBuilt_ -= AddToGrid;
        }

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

        public T GetAction<T>() where T : CharacterAction
        {
            for( int i = 0; i < actions_.Count; ++i )
            {
                if (actions_[i] is T)
                    return actions_[i] as T;
            }

            return null;
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
                   // Debug.Log("Pausing " + name, gameObject);
                    animator_.speed = 0f;

                    renderer_.color = new Color(.32f, .32f, .32f);
                    //renderer_.enabled = false;
                }
                else
                {
                    //Debug.LogFormat("UnPausing {0}", name);
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


        void AddToGrid( Grid grid )
        {
            //Debug.LogFormat("Adding {0} to grid", name);
            // Snap our character's z position to the highest point on the grid.
            // Not ideal but for now while we're having to manually place characters in the map this works.
            var pos = transform.position;
            pos = (Vector3)(IntVector3)pos;
            pos.z = grid.GetHeight(new IntVector2(pos.x, pos.y));
            
            transform.position = pos;

            // Add our character to the grid at it's current position.
            //Debug.LogFormat("Adding {0} to {1}", name, GridPosition);
            grid.AddObject(GridPosition, this);

            var players = FindObjectsOfType<StrategyPlayer>();
            foreach( var p in players )
            {
                if( p.Units.Contains(this) )
                {
                    //Debug.LogFormat("Adding {0} to map and assigning to faction {1}", name, p.name);
                    OwningPlayer = p;
                    break;
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // Selection events get forwarded to HumanPlayer
            if (OnClicked_ != null)
                OnClicked_(this);
        }

        public void Update()
        {
            if (Input.GetButtonDown("XboxA") &&
                (StrategyCursor.Instance.tile.x == transform.position.x &&
                 StrategyCursor.Instance.tile.y == transform.position.y))
            {
                // Selection events get forwarded to HumanPlayer
                if (OnClicked_ != null)
                    OnClicked_(this);
            }

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