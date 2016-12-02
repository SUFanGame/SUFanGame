using UnityEngine;
using System.Collections;
using SUGame.StrategyMap.Players;
using SUGame.Util.Logic.States;
using SUGame.Util;
using SUGame.StrategyMap.UI;
using SUGame.Util.Common;

namespace SUGame.StrategyMap.Characters.Actions.UIStates
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

        public override void OnCharacterSelected(StrategyPlayer player, MapCharacter character)
        {
            if( character == actor_ )
            {
                Machine.Pop();
                Machine.Push(new ChooseActionUIState(actor_));
            }
        }

        public override void OnExit()
        {
            HighlightGrid.Clear();
            //StrategyCursor.SetTargetTypes(null);
            base.OnExit();
        }
    }

}
