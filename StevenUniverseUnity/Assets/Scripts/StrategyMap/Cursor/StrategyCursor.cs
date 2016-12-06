using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SUGame.StrategyMap;
using SUGame.StrategyMap.Players;
using UnityEngine.EventSystems;
using SUGame.Util;
using SUGame.StrategyMap.Characters.Actions;
using SUGame.Util.Common;

namespace SUGame.StrategyMap
{
    // TODO : This should highlight the relevant stuff that's being selected on the strategy map (units, terrain, etc).
    //        This will have to work with controllers as well so we should work out some way to snap to targets rather than having
    //        to rely on hovering the mouse over stuff
    public class StrategyCursor : MonoBehaviour
    {
        static Vector3 lastCursorPosition_ = Vector3.zero;
        static SpriteRenderer renderer_ = null;
        static Physics2DRaycaster raycaster_;

        List<IntVector3> positionBuffer_ = new List<IntVector3>();
        public bool controllerMode = false;

        public CursorMode CursorMode_ { get; private set; }
        // Area to which the cursor is constrained.
        public int range_ = 5;

        public Transform snapTarget_ = null;

        public Vector2 tile = Vector2.zero;

        /// <summary>
        /// The sprite currently rendered by the cursor.
        /// </summary>
        public static Sprite Sprite
        {
            get
            {
                return renderer_.sprite;
            }
            set
            {
                renderer_.sprite = value;
            }
        }

        /// <summary>
        /// Determines which objects are hit in selection events. Remove objects from the layer
        /// to prevent them from being selected.
        /// </summary>
        public static LayerMask RaycastLayers
        {
            get
            {
                return raycaster_.eventMask;
            }
            set
            {
                raycaster_.eventMask = value;
            }
        }

        public bool onGUI_;

        private bool dPadInUse = false;


        /// <summary>
        /// The world position of the cursor, snapped to the grid.
        /// </summary>
        public Vector3 Position_
        {
            get
            {
                if (controllerMode)
                {
                    return lastCursorPosition_;
                }
                else
                {
                    var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    pos.x = Mathf.Floor(pos.x) + 0.5f;
                    pos.y = Mathf.Floor(pos.y) + 0.5f;
                    return pos;
                }
                //for (int i = 0; i < 2; ++i)
                //    pos[i] = Mathf.Floor(pos[i]);

            }
        }

        public static StrategyCursor Instance
        {
            get; private set;
        }

        void Awake()
        {
            renderer_ = GetComponent<SpriteRenderer>();
            raycaster_ = GameObject.FindObjectOfType<Physics2DRaycaster>();
            Instance = this;
            
        }

        /// <summary>
        /// Sets which targets the cursor is able to select. If TargetType does not include POINT
        /// then the cursor will attempt to snap to valid targets ( if any ) in the given area.
        /// </summary>
        public static void SetTargetTypes( TargetProperties ? targetProperties )
        {
            //if( targets == TargetType.ALL )
            //{
            //    Debug.Log("ALL UNITS");
            //}

            //// If our targets don't include point then the cursor will not be free-moving
            //if ((TargetType.POINT & targets) != TargetType.POINT )
            //{
            //    Instance.SnapMode = true;
            //}
            //else
            //{
            //    Instance.SnapMode = false;
            //}

            // If target types is null then there are no restrictions on the cursor.
            if (targetProperties == null)
            {
                Instance.CursorMode_ = CursorMode.FREE;
                //Instance.targetTypes_ = TargetType.ALL;
                Instance.range_ = 0;
                return;
            }

            var types = targetProperties.Value.type_;

            // If the target types don't include point then the cursor will not be free moving -
            // it will have to snap to targets in the area.
            if ((TargetType.POINT & types) != TargetType.POINT )
            {
            }
            // If the target types DO include point then the cursor will only be restricted by the size of the area, but will
            // otherwise be free moving.
            else
            {

            }
        }

        void Update()
        {
            Vector3 cursorPos = default(Vector3);

            if (!controllerMode)
            {
                cursorPos = MouseCursorPos(cursorPos);
            }
            else
            {
                cursorPos = ControllerCursorPos(cursorPos);
            }

            // TEST
            if (snapTarget_ != null)
            {
                cursorPos = SnapToArea((IntVector3)snapTarget_.transform.position, range_);
                cursorPos.z = 1;
            }

            if (cursorPos != lastCursorPosition_)
            {
                switch( CursorMode_ )
                {
                    case CursorMode.FREE:
                        renderer_.transform.position = (Vector3)cursorPos;
                        lastCursorPosition_ = cursorPos;
                        tile.x = lastCursorPosition_.x - 0.5f;
                        tile.y = lastCursorPosition_.y - 0.5f;
                        break;
                }
            }
        }

        private Vector3 ControllerCursorPos(Vector3 cursorPos)
        {
            cursorPos = lastCursorPosition_;
            if (Input.GetAxisRaw("XboxDpadX") != 0 && !dPadInUse)
            {
                cursorPos.x += Input.GetAxis("XboxDpadX");
                dPadInUse = true;
            }
            else if (Input.GetAxisRaw("XboxDpadY") != 0 && !dPadInUse)
            {
                cursorPos.y += Input.GetAxis("XboxDpadY");
                dPadInUse = true;
            }
            if (Input.GetAxisRaw("XboxDpadX") == 0 && Input.GetAxisRaw("XboxDpadY") == 0)
            {
                dPadInUse = false;
            }
            

            return cursorPos;
        }

        private Vector3 MouseCursorPos(Vector3 cursorPos)
        {
            switch (CursorMode_)
            {
                case CursorMode.FREE:
                    cursorPos = Position_;
                    cursorPos.z = 1;
                    break;

                case CursorMode.AREA:
                    cursorPos = Position_;

                    break;
            }
            return cursorPos;
        }

        void OnGUI()
        {
            if ((Input.GetAxis("XboxDpadX") != 0) || (Input.GetAxis("XboxDpadY") != 0))
            {
                controllerMode = true;
            }
            else if (Input.GetMouseButton(1))
            {
                controllerMode = false;
            }



            if (!onGUI_)
                return;

            //GUILayout.Label("Cursor Pos: " + Position_);
        }

        /// <summary>
        /// Given a center point and a range this will force the cursor to remain in that area.
        /// </summary>
        Vector3 SnapToArea( IntVector3 center, int range )
        {
            // To Snap: If we're inside the X dimension of the area just snap to y (cursor Y = area size Y - cursor Y)
            var pos = (IntVector3)Position_;
            
            for( int i = 0; i < 2; ++i )
            {
                pos[i] = Mathf.Clamp(pos[i], center[i] - range, center[i] + range);
            }

            var diff = center - pos;
            diff = IntVector3.Clamp(diff, -range, range);

            for( int i = 0; i < 2; ++i )
            {
                diff[i] = Mathf.Abs(diff[i]);
            }

            // Get the nearest point by snapping along the x axis first.
            var xFirst = GetSnappedPoint(pos, diff, center, range, 0);
            // Get the nearest point by snapping along the y axis first.
            var yFirst = GetSnappedPoint(pos, diff, center, range, 1);

            positionBuffer_.Clear();
            positionBuffer_.Add(xFirst);
            positionBuffer_.Add(yFirst);
            // Measure the distance from our potential snapped positions to the cursor. Whichever is shortest will be chosen.
            pos = NearestPoint(pos, positionBuffer_);

            return (Vector3)pos;

           // cursorX = Mathf.Clamp(cursorX, center.x - range, center.y + range);
           // cursorY = Mathf.Clamp(cursorY, center.y - range, center.y + range);


            //return Vector3.zero;
        }

        /// <summary>
        /// Returns whichever point from the list is closest (via manhattan distance) to the point origin
        /// </summary>
        IntVector3 NearestPoint( IntVector3 origin, List<IntVector3> points )
        {
            if (points == null || points.Count == 0)
                throw new System.ArgumentException("Nearest point called with a null or empty list");

            IntVector3 p = points[0];
            var minDist = Vector3.Distance((Vector3)origin, (Vector3)p);
                //IntVector2.ManhattanDistance((IntVector2)origin, (IntVector2)p);

            for( int i = 1; i < points.Count; ++i )
            {
                var dist = Vector3.Distance((Vector3)origin, (Vector3)points[i]);
                    //IntVector2.ManhattanDistance((IntVector2)origin, (IntVector2)points[i] );

                if( dist < minDist )
                {
                    p = points[i];
                    minDist = dist;
                }
            }

            return p;
        }

        IntVector3 GetSnappedPoint( IntVector3 pos, IntVector3 diff, IntVector3 center, int range, int axis )
        {
            pos[axis] = Mathf.Clamp(pos[axis], center[axis] - range + diff[1 - axis], center[axis] + range - diff[1 - axis]);

            diff[axis] = center[axis] - pos[axis];

            axis = 1 - axis;

            pos[axis] = Mathf.Clamp(pos[axis], center[axis] - range + diff[1 - axis], center[axis] + range - diff[1 - axis]);

            return pos;
        }

        public enum CursorMode
        {
            // The cursor has no restrictions
            FREE,
            // The cursor will be snapped to certain units based on target properties.
            UNITS,
            // The cursor will be restricted to a certain area based on target properties.
            AREA
        }
    }
}
