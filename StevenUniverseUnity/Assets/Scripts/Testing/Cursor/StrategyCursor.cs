using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Battle;

namespace StevenUniverse.FanGame.Battle
{
    public class StrategyCursor : MonoBehaviour
    {
        Vector3 lastCursorPosition_ = Vector3.zero;
        SpriteRenderer cursorSprite_ = null;

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

        void Awake()
        {
            cursorSprite_ = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            var cursorPos = Position_;
            cursorPos.z = 1;

            if (cursorPos != lastCursorPosition_)
            {
                cursorSprite_.transform.position = (Vector3)cursorPos;
                lastCursorPosition_ = cursorPos;
            }
        }
    }
}
