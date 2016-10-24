using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using StevenUniverse.FanGame.StrategyMap;
using System.Linq;
using System;
using StevenUniverse.FanGame.StrategyMap.UI;

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
        // Imaginary class containing specific character data that might be passed between modules.
        // CharacterData data_;

        bool moving_ = false;

        public float tilesMovedPerSecond_ = 3f;

        public int moveRange_ = 5;

        SpriteRenderer renderer_;

        List<CharacterAction> actions_ = null;

        /// <summary>
        /// Populate the given buffer with valid actions this character can perform.
        /// </summary>
        public void GetActions( List<CharacterAction> buffer )
        {
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
            //path_ = new GridPaths(grid_);
            var actions = GetComponentsInChildren<CharacterAction>();

            if( actions != null && actions.Length != 0 )
            {
                actions_ = actions.ToList();
            }

            UpdateSortingOrder();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            eventData.selectedObject = gameObject;
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (moving_)
                return;

            CharacterActionsUI.Show(this);
        }

        public void UpdateSortingOrder()
        {
            renderer_.sortingOrder = GridPosition.z * 100 + 90;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            HighlightGrid.Clear();
        }
    }

}