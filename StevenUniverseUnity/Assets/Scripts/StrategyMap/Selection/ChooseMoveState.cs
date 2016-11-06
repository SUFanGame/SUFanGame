using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.StrategyMap;
using StevenUniverse.FanGame.StrategyMap.UI;

namespace StevenUniverse.FanGame.Util.Logic.States
{
    public class ChooseMoveState : State
    {
        MapCharacter actor_ = null;
        static GridPaths paths_ = new GridPaths();

        // The original position of the character, used when following actions are cancelled
        // to return the character to where they were before.
        IntVector3 originalPosition_;

        public ChooseMoveState(StateMachine machine, MapCharacter actor) : base(machine)
        {
            actor_ = actor;
        }

        public override IEnumerator Tick()
        {
            while (true)
            {
                // Continue processing from click
                if (Input.GetMouseButtonDown(0))
                {
                    HighlightGrid.Clear();
                    break;
                }

                // Cancel move function from escape
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    HighlightGrid.Clear();
                    machine_.Pop();
                    yield break;
                }

                yield return null;
            }

            var grid = Grid.Instance;
            IntVector3 cursorPos = (IntVector3)(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            int height = grid.GetHeight((IntVector2)cursorPos);

            // If the player clicks outside the grid
            if (height == int.MinValue)
            {
                machine_.Pop();
                yield break;
            }

            cursorPos.z = height;

            // If the player clicks on the character again
            if ((IntVector3)cursorPos == actor_.GridPosition)
            {
                machine_.Push(new ChooseCharacterActionState(machine_, actor_));
                yield break;
            }

            var node = grid.GetNode(cursorPos);
            if (node != null && paths_.Contains(node))
            {
                yield return MoveFollow(cursorPos);

                machine_.Push(new ChooseCharacterActionState(machine_, actor_));
            }

            paths_.Clear();
        }



        public override void OnEnter()
        {
            base.OnEnter();

            HighlightGrid.Clear();

            var pos = actor_.GridPosition;
            var grid = Grid.Instance;
            originalPosition_ = pos;

            // TODO: Come up with a different method, this will not work if a unit stops beneath another tile. Just don't let
            // that happen maybe?
            pos.z = grid.GetHeight((IntVector2)pos);

            // Nap the actor to the grid at the starting position.
            actor_.transform.position = (Vector3)pos;

            paths_.Clear();

            grid.GetNodesInRange(pos, actor_.moveRange_, paths_, MovementType.GROUNDED);

            foreach (var node in paths_.NodesReadOnly_)
            {
                HighlightGrid.HighlightPos(node.Pos_.x, node.Pos_.y, node.Pos_.z, Color.blue);
            }
        }


        IEnumerator MoveFollow(IntVector3 cursorPos)
        {
            var cam = GameObject.FindObjectOfType<SmoothCamera>();
            cam.follow_ = true;
            yield return CharacterUtility.MoveTo(actor_, cursorPos, paths_);
            //yield return CharacterUtility.MoveTo( actor_, cursorPos );
            cam.follow_ = false;
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
                if ( value != Paused_ && value == false )
                {
                    base.Paused_ = value;
                    // If so it means a previous state was cancelled. We want to reset this state,
                    // but note that there is no way to reset the state of a coroutine, so
                    // at this point it's already past the input polling portion. 
                    // We'll use our cached character position to return them
                    // to where they should be then push a new move state to start fresh.

                    // TODO: Maybe we could adjust so we're keeping track of whether or not
                    // input was already processed - if so we could just reset that state
                    // and the coroutine would return to polling for input.
                    actor_.transform.position = (Vector3)originalPosition_;
                    machine_.ReplaceTop(new ChooseMoveState(machine_, actor_));
                }
                else
                    base.Paused_ = value;
            }
        }


    }

}
