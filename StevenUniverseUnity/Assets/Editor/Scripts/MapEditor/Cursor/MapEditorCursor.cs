using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Util;
using StevenUniverse.FanGame.World;

namespace StevenUniverse.FanGameEditor.SceneEditing
{
    public class MapEditorCursor
    {
        int size_;
        /// <summary>
        /// The size of the cursor
        /// </summary>
        public int Size_
        {
            get
            {
                return size_;
            }
            set
            {
                value = Mathf.Clamp(value, 0, int.MaxValue);

                if (value == size_)
                    return;

                // 0 == one tile
                if( value == 0 )
                {
                    size_ = 0;
                    cursorPoints_.Add(IntVector2.zero);
                    return;
                }

                // If the size is greater than 0 then we'll cache our points list so we can quickly apply
                // the effect to our target area.
                cursorPoints_.Clear();

                size_ = value;

                int iterSize = size_ * 2 + 1;

                IntVector2 half = (IntVector2.one * iterSize) / 2;

                for (int x = -half.x; x < half.x + 1; ++x)
                {
                    for (int y = -half.y; y < half.y + 1; ++y)
                    {
                        cursorPoints_.Add( new IntVector2(x, y) );
                    }

                }
            }
        }

        // List of points affected by our cursor.
        List<IntVector2> cursorPoints_ = new List<IntVector2>();

        public MapEditorCursor()
        {
            cursorPoints_.Add(IntVector2.zero);
        }
        
        /// <summary>
        /// Apply the cursor's effects at the given position.
        /// </summary>
        public void Apply( Map map, IntVector3 pos )
        {
            for( int i = 0; i < cursorPoints_.Count; ++i )
            {
                IntVector3 targetPoint = pos + (IntVector3)cursorPoints_[i];

                Effect(map, targetPoint);

            }
        }

        /// <summary>
        /// Derived classes can implement this to define how the cursor affects the map.
        /// </summary>
        /// <param name="map">The map being affected.</param>
        /// <param name="localPos">The target point in world space..</param>
        public virtual void Effect( Map map, IntVector3 worldPos )
        {
        }
    }
}