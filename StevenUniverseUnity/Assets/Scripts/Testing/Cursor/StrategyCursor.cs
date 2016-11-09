using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.StrategyMap;
using StevenUniverse.FanGame.StrategyMap.Players;
using UnityEngine.EventSystems;

namespace StevenUniverse.FanGame.StrategyMap
{
    public class StrategyCursor : MonoBehaviour
    {
        Vector3 lastCursorPosition_ = Vector3.zero;
        SpriteRenderer renderer_ = null;
        Physics2DRaycaster raycaster_;
        /// <summary>
        /// The sprite currently rendered by the cursor.
        /// </summary>
        public static Sprite Sprite
        {
            get
            {
                return Instance.renderer_.sprite;
            }
            set
            {
                Instance.renderer_.sprite = value;
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
                return Instance.raycaster_.eventMask;
            }
            set
            {
                Instance.raycaster_.eventMask = value;
            }
        }

        public bool onGUI_;

        /// <summary>
        /// The world position of the cursor, snapped to the grid.
        /// </summary>
        public static Vector3 Position_
        {
            get
            {
                var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                for (int i = 0; i < 2; ++i)
                    pos[i] = Mathf.Floor(pos[i]);
                return pos;
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
        /// Sets which targets the cursor will focus on, typically points, units (enemy,ally), or self.
        /// Optional parameters may be required depending on the scope of the targets.
        /// </summary>
        /// <param name="targets">The target types to focus on.</param>
        /// <param name="player">The player against which ENEMY/ALLY standing will be tested (ignored if ENEMY/ALLY is excluded)</param>
        /// <param name="character">The map character against which SELF will be tested. Ignored if SELF is excluded.</param>
        public static void SetTargetTypes( TargetType targets, StrategyPlayer player = null, MapCharacter character = null )
        {
            if( targets == TargetType.ALL )
            {
                Debug.Log("ALL UNITS");
            }
        }

        void Update()
        {
            var cursorPos = Position_;
            cursorPos.z = 1;

            if (cursorPos != lastCursorPosition_)
            {
                renderer_.transform.position = (Vector3)cursorPos;
                lastCursorPosition_ = cursorPos;
            }
        }

        void OnGUI()
        {
            if (!onGUI_)
                return;

            GUILayout.Label("Cursor Pos: " + Position_);
        }
        
    }
}
