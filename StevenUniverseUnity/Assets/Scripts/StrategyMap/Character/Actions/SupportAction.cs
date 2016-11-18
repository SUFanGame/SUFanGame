using System.Collections.Generic;
using StevenUniverse.FanGame.Interactions;
using System.Collections;
using StevenUniverse.FanGame.Util.Logic.States;
using UnityEngine;
using StevenUniverse.FanGame.Util;

namespace StevenUniverse.FanGame.StrategyMap
{
    // TODO : Subclass Support/Attack actions into a base CharacterTargetableAction class?

    public class SupportAction : CharacterAction
    {
        public TargetProperties targetProperties_;

        //TODO: since a map may only have 1-2 supportable characters, it may be quicker to set talkPredicate
        //to something specified on the map's data. That won't happen until we have a fully implemented map.
       // System.Predicate<MapCharacter> talkPredicate;

        List<MapCharacter> allies_ = new List<MapCharacter>();

        // Set through the inspector
        public SupportRunner supportCanvas_;


        List<MapCharacter> validTargets_ = new List<MapCharacter>();
        /// <summary>
        /// Read-Only access to the list of valid Support Targets given this action's target properties
        /// </summary>
        public IList<MapCharacter> ValidTargetsReadOnly_ { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            targetProperties_.type_ = TargetType.ALLY;
            targetProperties_.source_ = actor_;
            ValidTargetsReadOnly_ = validTargets_.AsReadOnly();
        }

        public IEnumerator Execute( string supportFileName )
        {
            //Debug.LogFormat("Running action {0}", supportFileName);
            supportCanvas_.Dialog = SupportLoader.ImportSupport(supportFileName);
            supportCanvas_.gameObject.SetActive(true);

            yield return supportCanvas_.DoDialog();

            actor_.Paused_ = true;
            //yield return base.Routine();
        }

        
        public override bool IsUsable()
        {
            var grid = Grid.Instance;

            var pos = actor_.GridPosition;

            validTargets_.Clear();

            grid.GetObjectsInArea((IntVector2)pos, targetProperties_.range_, validTargets_, targetProperties_.IsValid );
            // Quicker to check if this list even has anything.
            //if (talkPredicate.GetInvocationList().Length <= 0)
            //{
            //    return false;
            //}

            //var adj = pos + adjacent[i];

            //grid.GetObjects(adj, allies_, allyPredicate_);

            //return CharacterUtility.ScanForAdjacent(actor_, talkPredicate).Count > 0;

            return validTargets_.Count > 0;
        }

        public override State GetUIState()
        {
            // Bind our callback to Execute where the file name is the actor's name and the target support's name separated by a hyphen
            System.Func<MapCharacter, IEnumerator> cb = (c) => Execute(actor_.name + "-" + c.name);
            var state = new ChooseTargetUIState(actor_, this, targetProperties_, cb, ValidTargetsReadOnly_);
            return state;
        }
    }
}
