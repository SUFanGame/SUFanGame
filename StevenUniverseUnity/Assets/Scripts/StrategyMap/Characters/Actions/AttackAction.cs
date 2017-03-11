using System.Collections.Generic;
using SUGame.Factions;
using SUGame.Util.Logic.States;
using System.Collections;
using SUGame.Util;
using UnityEngine;
using SUGame.StrategyMap.Characters.Actions.UIStates;
using SUGame.Util.Common;

namespace SUGame.StrategyMap.Characters.Actions
{
    /// <summary>
    /// Character attack action
    /// </summary>
    public class AttackAction : CharacterAction
    {
        // TODO: Attack properties should be retrieved from whatever weapon the character is currently using.
        public TargetProperties targetProperties_;

        public override string UIName
        {
            get
            {
                return "Attack";
            }
        }

        /// <summary>
        /// Target to be attacked
        /// </summary>
        //public MapCharacter Target { get; set; }

        protected override void Awake()
        {
            base.Awake();

            //targetProperties_ = new TargetProperties(targetTy);
            targetProperties_.type_ = TargetType.ENEMY;
            targetProperties_.source_ = actor_;

            ValidTargetsReadOnly_ = validTargets_.AsReadOnly();
        }


        List<MapCharacter> validTargets_ = new List<MapCharacter>();
        /// <summary>
        /// Read-Only access to the list of valid attack targets given this attack's current target properties.
        /// </summary>
        public IList<MapCharacter> ValidTargetsReadOnly_ { get; private set; }

        public override bool IsUsable()
        {
            var grid = Grid.Instance;
            
            var pos = actor_.GridPosition;

            validTargets_.Clear();

            //Debug.Log("Checking attack targets...");
            grid.GetObjectsInArea((IntVector2)pos, targetProperties_.range_, validTargets_, targetProperties_.IsValid );

            //var adjacent = Directions2D.Quadrilateral;

            //opponents_.Clear();
            //for ( int i = 0; i < adjacent.Length; ++i )
            //{
            //    var adj = pos + adjacent[i];

            //    grid.GetObjects(adj, opponents_, opponentPredicate_ );
            //}

            return validTargets_.Count > 0;
        }

        public IEnumerator Execute( MapCharacter target )
        {
            // Play cool attack scene animation
            //target.Data.Stats.currentHP -= actor_.Data.Stats.str;
            actor_.Paused_ = true;
            yield return null;
        }

        public override State GetUIState()
        {
            //Debug.Log("Pushing choose target state from " + actor_.name);
            var state = new ChooseTargetUIState(actor_, targetProperties_, typeof(CombatUIState), ValidTargetsReadOnly_);
            return state;
        }
    }

}