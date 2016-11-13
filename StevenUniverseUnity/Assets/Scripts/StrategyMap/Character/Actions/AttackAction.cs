﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using StevenUniverse.FanGame.Factions;
using StevenUniverse.FanGame.Util.Logic.States;

namespace StevenUniverse.FanGame.StrategyMap
{
    /// <summary>
    /// Character attack action
    /// </summary>
    public class AttackAction : CharacterAction
    {
        static Predicate<MapCharacter> opponentPredicate_ = null;

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

            // Set our predicate to search only for enemies
            opponentPredicate_ = (c) =>
            {
                return targetProperties_.IsValid(c);
            };

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

            grid.GetObjectsInArea((IntVector2)pos, targetProperties_.range_, validTargets_, opponentPredicate_);

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
            actor_.Paused_ = true;
            yield return null;
        }

        public override State GetUIState()
        {
            var state = new ChooseTargetUIState(actor_, this, targetProperties_, Execute, ValidTargetsReadOnly_);
            return state;
        }
    }

}