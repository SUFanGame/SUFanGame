using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.StrategyMap.UI;

namespace StevenUniverse.FanGame.StrategyMap
{
    public class MoveAction : CharacterAction
    {
        GridPaths path_ = new GridPaths();

        //bool waitingForSelection_ = false;
         
        public override IEnumerator Routine()
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

            yield return WaitForInput();

            //yield return base.Routine();
        }

        IEnumerator WaitForInput()
        {
            while(true)
            {
                // Continue processing from click
                if ( Input.GetMouseButtonDown(0) )
                {
                    HighlightGrid.Clear();
                    break;
                }

                // Cancel move function from escape
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    HighlightGrid.Clear();
                    yield break;
                }

                yield return null;
            }

            var grid = Grid.Instance;
            IntVector3 cursorPos = (IntVector3)(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            int height = grid.GetHeight((IntVector2)cursorPos);

            if (height == int.MinValue)
                yield break;

            cursorPos.z = height;

            if ((IntVector3)cursorPos == actor_.GridPosition)
                yield break;

            var node = grid.GetNode(cursorPos);
            if (node != null && path_.Contains(node))
            {
                yield return MoveFollow(cursorPos);
            }

            path_.Clear();
        }

        IEnumerator MoveFollow( IntVector3 cursorPos )
        {
            var cam = GameObject.FindObjectOfType<SmoothCamera>();
            cam.follow_ = true;
            yield return CharacterUtility.MoveTo( actor_, cursorPos, path_ );
            //yield return CharacterUtility.MoveTo( actor_, cursorPos );
            cam.follow_ = false;
            CharacterActionsUI.Show(actor_);
        }
        
    }
}
