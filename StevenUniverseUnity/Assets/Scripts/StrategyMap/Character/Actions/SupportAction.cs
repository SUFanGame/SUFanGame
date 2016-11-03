using System.Collections.Generic;
using StevenUniverse.FanGame.Interactions;

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

        public override void Execute()
        {
            supportCanvas_.Dialog = SupportLoader.ImportSupport("Righty_Lefty_C");
            supportCanvas_.gameObject.SetActive(true);
        }

        
        public override bool IsUsable()
        {
            // Quicker to check if this list even has anything.
            if (talkPredicate.GetInvocationList().Length <= 0)
            {
                return false;
            }

            return CharacterUtility.ScanForAdjacent(actor_, talkPredicate).Count > 0;
        }
    }
}
