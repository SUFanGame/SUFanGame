using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.StrategyMap;
using StevenUniverse.FanGame.StrategyMap.UI;
using StevenUniverse.FanGame.StrategyMap.Players;

namespace StevenUniverse.FanGame.Util.Logic.States
{
    public class ChooseMoveUIState : State
    {
        MapCharacter actor_ = null;
        MoveAction moveAction_;

        // The original position of the character, used when following actions are cancelled
        // to return the character to where they were before.
        IntVector3 originalPosition_;
        IntVector3? target_ = null;

        GridPaths paths_ = null;

        public ChooseMoveUIState(MapCharacter actor, MoveAction moveAction )
        {
            actor_ = actor;
            moveAction_ = moveAction;
        }
        

        //public override IEnumerator Tick()
        //{
        //    // Process input first.
        //    while (!stopTicking_)
        //    {
        //        // Continue processing from click
        //        if (Input.GetMouseButtonDown(0))
        //        {
        //            HighlightGrid.Clear();
        //            break;
        //        }
        //        // Only yield if we haven't receieved input so we avoid the one frame delay
        //        // between input and action.
        //        else
        //        {
        //            yield return null;
        //        }

        //        // Cancel move function from escape
        //        if (Input.GetKeyDown(KeyCode.Escape))
        //        {
        //            HighlightGrid.Clear();
        //            Machine.Pop();
        //            yield break;
        //        }

        //    }


        //    var grid = Grid.Instance;

        //    // Retrieve our click point
        //    IntVector3 cursorPos = (IntVector3)(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        //    int height = grid.GetHeight((IntVector2)cursorPos);

        //    // If the player clicks outside the grid
        //    if (height == int.MinValue)
        //    {
        //        Machine.Pop();
        //        yield break;
        //    }

        //    cursorPos.z = height;

        //    // If the player clicks on the character again
        //    if ((IntVector3)cursorPos == actor_.GridPosition)
        //    {
        //        Machine.Push(new ChooseActionUIState(actor_));
        //        yield break;
        //    }

        //    var node = grid.GetNode(cursorPos);

        //    if (node != null && paths_.Contains(node))
        //    {
        //        // If we get here then the player clicked on a valid node to move to.
        //        //var move = actor_.GetAction<MoveAction>();

        //        // Assign our destination to the move action.
        //        //move.Destination = node.Pos_;

        //        // ...and execute the move action.
        //        //yield return move.Routine();

        //        Machine.Push(new ChooseActionUIState(actor_));
        //    }
        //}

        /// <summary>
        /// Displays the choose movement grid when we enter this state.
        /// </summary>
        public override void OnEnter()
        {
            base.OnEnter();

            HighlightGrid.Clear();

            originalPosition_ = actor_.GridPosition;

            var move = actor_.GetAction<MoveAction>();

            paths_ = move.GetPaths();

            foreach (var node in paths_.NodesReadOnly_)
            {
                HighlightGrid.HighlightPos(node.Pos_.x, node.Pos_.y, node.Pos_.z, Color.blue);
            }


            TargetProperties targetProperties = new TargetProperties(TargetType.POINT, actor_.moveRange_, actor_); 

            StrategyCursor.SetTargetTypes( targetProperties );
        }




        public override bool Paused_
        {
            get
            {
                return base.Paused_;
            }

            set
            {
                // Check if we're unpausing
                if ( value != Paused_ )
                {
                    // Check if we're unpausing
                    if ( value == false )
                    {
                        // Show the grid if we're unpausing
                        actor_.transform.position = (Vector3)originalPosition_;
                    }
                    else
                    {
                        // Hide the grid if we're pausing.
                        HighlightGrid.Clear();
                    }
                }

                base.Paused_ = value;
            }
        }

        public override IEnumerator Tick()
        {
            if( target_ != null )
            {
                yield return moveAction_.Execute(target_.Value);
                target_ = null;
                Machine.Push(new ChooseActionUIState(actor_));

            }
            yield return null;
        }

        public override void OnPointSelected(StrategyPlayer player, Node point)
        {
            //Debug.LogFormat("PointSelected");
            target_ = point.Pos_;
        }

        public override void OnExit()
        {
            HighlightGrid.Clear();
            StrategyCursor.SetTargetTypes(null);
            base.OnExit();
        }
    }

}
