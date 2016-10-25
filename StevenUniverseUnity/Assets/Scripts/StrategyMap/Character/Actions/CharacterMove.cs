using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StevenUniverse.FanGame.StrategyMap
{
    public class CharacterMove : CharacterAction
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

                    var cam = GameObject.FindObjectOfType<SmoothCamera>();
                    cam.follow_ = true;
                    StartCoroutine(MoveTo(cursorPos));
                }

                path_.Clear();
            }
        }

        IEnumerator MoveTo(IntVector3 targetPos)
        {
            var gridPos = actor_.GridPosition;
            
            var grid = Grid.Instance;
            //Debug.LogFormat("Moving {0} from {1} to {2}", name, current, pos);

            List<Node> path = new List<Node>();
            path_.PathPosition(actor_.GridPosition, targetPos, path, grid);

            //string pathString = string.Join("->", path.Select(p => p.Pos_.ToString()).ToArray());
            //Debug.LogFormat("Path to {0}: {1}", targetPos, pathString );

            for (int i = 0; i < path.Count; ++i)
            {
                var next = path[i];

                gridPos = actor_.GridPosition;
                gridPos.z = next.Pos_.z;
                actor_.GridPosition = gridPos;

                actor_.UpdateSortingOrder();

                var dest = (Vector3)next.Pos_;

                float t = 0;
                while (t <= 1f)
                {
                    transform.position = Vector3.Lerp(transform.position, dest, t);
                    t += Time.deltaTime * actor_.tilesMovedPerSecond_;
                    yield return null;
                }
            }
            
            //Debug.LogFormat("Done moving");
            yield return null;
        }

    }
}
