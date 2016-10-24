using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using StevenUniverse.FanGame.MapSkirmish;
using System.Linq;
using System;

// Just a note about unity's built in Selection Handlers - they require that the camera have a "Physics Raycaster"
// and that an "EventSystem" is in the scene (GameObject->UI->EventSystem). Any objects to be selected
// must also have an appropriate collider. The handlers WILL receieve notifications from chid objects.

namespace StevenUniverse.FanGame.MapSkirmish
{
    /// <summary>
    /// A character in the battle map.
    /// </summary>
    [SelectionBase]
    public class MapCharacter : MonoBehaviour, IPointerClickHandler, ISelectHandler, IDeselectHandler
    {
        // Imaginary class containing specific character data that might be passed between modules.
        // CharacterData data_;
        GridPaths path_ = null;

        Grid grid_ = null;

        bool moving_ = false;

        public float tilesMovedPerSecond_ = 3f;

        public int moveRange_ = 5;

        SpriteRenderer renderer_;

        IntVector3 GridPosition
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
            grid_ = GameObject.FindObjectOfType<Grid>();
            renderer_ = GetComponentInChildren<SpriteRenderer>();
            path_ = new GridPaths(grid_);

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

            HighlightGrid.Clear();

            var pos = GridPosition;
            pos.z = grid_.GetHeight((IntVector2)pos);

            transform.position = (Vector3)pos;

            path_.Clear();

            grid_.GetNodesInRange(pos, moveRange_, path_);
            
            foreach( var node in path_.NodesReadOnly_ )
            {
                HighlightGrid.HighlightPos(node.Pos_.x, node.Pos_.y, node.Pos_.z, Color.blue );
            }
        }

        void Update()
        {
            if (moving_)
            {
                HighlightGrid.Clear();
                return;
            }

            if( Input.GetMouseButtonDown(0))
            {
                IntVector3 cursorPos = (IntVector3)(Camera.main.ScreenToWorldPoint(Input.mousePosition));

                int height = grid_.GetHeight((IntVector2)cursorPos);

                if (height == int.MinValue)
                    return;

                cursorPos.z = height;
                
                if ((IntVector3)cursorPos == GridPosition)
                    return;

                if( path_.Contains(cursorPos) )
                {
                    moving_ = true;
                    var cam = GameObject.FindObjectOfType<SmoothCamera>();
                    cam.follow_ = true;
                    StartCoroutine( MoveTo(cursorPos) );
                }
                path_.Clear();
            }
        }

        IEnumerator MoveTo( IntVector3 targetPos )
        {
            var current = GridPosition;

            //Debug.LogFormat("Moving {0} from {1} to {2}", name, current, pos);

            List<Node> path = new List<Node>();
            path_.PathPosition(GridPosition, targetPos, path);

            //string pathString = string.Join("->", path.Select(p => p.Pos_.ToString()).ToArray());
            //Debug.LogFormat("Path to {0}: {1}", targetPos, pathString );

            for (int i = 0; i < path.Count; ++i)
            {
                var next = path[i];

                var pos = GridPosition;
                pos.z = next.Pos_.z;
                GridPosition = pos;

                UpdateSortingOrder();
                
                var dest = (Vector3)next.Pos_;

                float t = 0;
                while (t <= 1f)
                {
                    transform.position = Vector3.Lerp(transform.position, dest, t );
                    t += Time.deltaTime * tilesMovedPerSecond_;
                    yield return null;
                }
            }

            moving_ = false;
            Debug.LogFormat("Done moving");
            yield return null;
        }

        void UpdateSortingOrder()
        {
            renderer_.sortingOrder = GridPosition.z * 100 + 90;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            HighlightGrid.Clear();
        }
    }

}