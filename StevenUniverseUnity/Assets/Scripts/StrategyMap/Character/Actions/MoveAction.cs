using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StevenUniverse.FanGame.StrategyMap
{
    public class MoveAction : CharacterAction
    {
        GridPaths path_ = new GridPaths();

        //bool waitingForSelection_ = false;
         
        public override void Execute()
        {
            HighlightGrid.Clear();

            var pos = actor_.GridPosition;
            var grid = Grid.Instance;

            // TODO: Come up with a different method, this will not work if a unit stops beneath another tile. Just don't let
            // that happen maybe?
            pos.z = grid.GetHeight((IntVector2)pos);

            transform.position = (Vector3)pos;

            path_.Clear();

            grid.GetNodesInRange(pos, actor_.moveRange_, path_, MovementType.GROUNDED );

            foreach (var node in path_.NodesReadOnly_)
            {
                HighlightGrid.HighlightPos(node.Pos_.x, node.Pos_.y, node.Pos_.z, Color.blue);
            }

            //waitingForSelection_ = true;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HighlightGrid.Clear();

                var grid = Grid.Instance;
                IntVector3 cursorPos = (IntVector3)(Camera.main.ScreenToWorldPoint(Input.mousePosition));

                int height = grid.GetHeight((IntVector2)cursorPos);

                if (height == int.MinValue)
                    return;

                cursorPos.z = height;

                if ((IntVector3)cursorPos == actor_.GridPosition)
                    return;

                var node = grid.GetNode(cursorPos);
                if( node != null && path_.Contains(node) )
                {
                    StartCoroutine(MoveFollow(cursorPos));
                }

                path_.Clear();
            }
        }

        IEnumerator MoveFollow( IntVector3 cursorPos )
        {
            var cam = GameObject.FindObjectOfType<SmoothCamera>();
            cam.follow_ = true;
            //yield return CharacterUtility.MoveTo( actor_, cursorPos, path_ );
            yield return CharacterUtility.MoveTo( actor_, cursorPos );
            cam.follow_ = false;
        }
        
    }
}
