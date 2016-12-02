using SUGame.Factions;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SUGame.StrategyMap.Players
{
    /// <summary>
    /// An example AI Player
    /// </summary>
    public class AIPlayer : StrategyPlayer
    {

        public override IEnumerator Tick()
        {
            // AI would perform it's actions here.
            yield return null;
        }
    }
}