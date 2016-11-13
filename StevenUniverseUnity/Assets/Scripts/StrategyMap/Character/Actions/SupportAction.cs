using System.Collections.Generic;
using StevenUniverse.FanGame.Interactions;
using System.Collections;

namespace StevenUniverse.FanGame.StrategyMap
{
    public class SupportAction : CharacterAction
    {
        //TODO: since a map may only have 1-2 supportable characters, it may be quicker to set talkPredicate
        //to something specified on the map's data. That won't happen until we have a fully implemented map.
        System.Predicate<MapCharacter> talkPredicate;

        List<MapCharacter> allies_ = new List<MapCharacter>();

        // Set through the inspector
        public SupportRunner supportCanvas_;

        protected override void Awake()
        {
            base.Awake();
                        
            talkPredicate = (other) =>
            {
                return actor_.Data.Faction_.GetStanding(other.Data.Faction_) == Factions.Standing.FRIENDLY 
                //&& actor_.Data.SupportInfos.Contains( other )
                ;
            };
            
        }

        public IEnumerator Execute( string supportFileName )
        {
            supportCanvas_.Dialog = SupportLoader.ImportSupport(supportFileName);
            supportCanvas_.gameObject.SetActive(true);

            yield return null;
            //yield return base.Routine();
        }

        
        public override bool IsUsable()
        {
            // Quicker to check if this list even has anything.
            if (talkPredicate.GetInvocationList().Length <= 0)
            {
                return false;
            }

            //var adj = pos + adjacent[i];

            //grid.GetObjects(adj, allies_, allyPredicate_);

            return CharacterUtility.ScanForAdjacent(actor_, talkPredicate).Count > 0;
        }
    }
}
