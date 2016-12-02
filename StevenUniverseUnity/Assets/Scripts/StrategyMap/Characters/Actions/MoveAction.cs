using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.StrategyMap.UI;
using SUGame.Util;
using SUGame.Characters;
using SUGame.Util.Common;

namespace SUGame.StrategyMap.Characters.Actions
{
    /// <summary>
    /// Allows the character to move around the map.
    /// </summary>
    public class MoveAction : CharacterAction
    {
        GridPaths paths_ = new GridPaths();

        //public IntVector3 Destination { get; set; }

        //public IEnumerator Execute()
        //{
        //    yield return MoveFollow(Destination);
        //}

        public IEnumerator Execute( IntVector3 pos )
        {
            yield return MoveFollow(pos);
        }

        /// <summary>
        /// Retrieves the viable paths this character can moved to. 
        /// Players and AIs can use this to examine their potiential moves.
        /// Note this is not a cheap operation and it passes out the underlying paths object.
        /// Results should be cached and should never be modified.
        /// </summary>
        public GridPaths GetPaths()
        {
            var pos = actor_.GridPosition;
            var grid = Grid.Instance;

            // TODO: Come up with a different method, this will not work if a unit stops beneath another tile. Just don't let
            // that happen maybe?
            pos.z = grid.GetHeight((IntVector2)pos);

            // Nap the actor to the grid at the starting position.
            actor_.transform.position = (Vector3)pos;

            paths_.Clear();

            // TODO: Work out movement types and retrieve from character
            grid.GetNodesInRange(pos, actor_.moveRange_, paths_, MovementType.GROUNDED);

            return paths_;
        }

        IEnumerator MoveFollow(IntVector3 cursorPos)
        {
            var cam = GameObject.FindObjectOfType<SmoothCamera>();
            cam.SnapToTarget();
            cam.follow_ = true;
            yield return CharacterUtility.MoveTo(actor_, cursorPos, paths_);
            //yield return CharacterUtility.MoveTo( actor_, cursorPos );
            cam.follow_ = false;
        }
    }
}
