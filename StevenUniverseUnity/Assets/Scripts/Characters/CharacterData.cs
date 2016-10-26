using UnityEngine;
using StevenUniverse.FanGame.Data;
using StevenUniverse.FanGame.Characters.Customization;
using StevenUniverse.FanGame.Util;
using StevenUniverse.FanGame.Factions;

namespace StevenUniverse.FanGame.Characters
{
    /// <summary>
    /// Base character data, intended to entirely encapsulate all information needed to load characters in different contexts.
    /// </summary>
    [System.Serializable]
    public class CharacterData : JsonBase<CharacterData>
    {
        
        [SerializeField]
        private string characterName; //Name
        [SerializeField]
        private Faction faction;
        [SerializeField]
        private HeldItems items; //WILDCATS
        [SerializeField]
        private Skill[] skills; //All available skills
        [SerializeField]
        private UnitStats stats; //All the unit battle modifiers
        [SerializeField]
        private SupportInfo[] supportInfos;
        
        //Unchanged from original Entity, consider changing
        //[SerializeField]
        //private Outfit outfit; // TO-DO change to all graphical stuff
        [SerializeField]
        private SaveData savedData;

        public CharacterData(  
            string characterName,
            string affiliation, 
            //Outfit startingOutfit,
            SaveData saveData
            )
        {
            this.characterName = characterName;
            this.faction = (Faction)System.Enum.Parse(typeof(Faction), affiliation, true );
            //this.outfit = startingOutfit;
            this.savedData = saveData;

            // All other data parameters may want to be loaded from SaveData at instantiation.
            // This particular class as-is is meant for in-map character entities but could be further abstracted
        }


        //Name
        public string EntityName
        {
            get { return characterName; }
            set { characterName = value; }
        }

        //Team name
        public Faction Faction_
        {
            get { return faction; }
            set { faction = value; }
        }

        //Held items
        public HeldItems Items
        {
            get { return items; }
            set { items = value; }
        }
        
        //Skills available
        public Skill[] Skills
        {
            get { return skills; }
            set { skills = value; }
        }
        
        //Unit stats
        public UnitStats Stats
        {
            get { return stats; }
            set { stats = value; }
        }


        public SaveData SavedData
        {
            get { return savedData; }
            set { savedData = value; }
        }

        
        //Outfit
        //public Outfit Outfit
        //{
        //    get { return outfit; }
        //    set { outfit = value; }
        //}

        public SupportInfo[] SupportInfos
        {
            get { return supportInfos; }
            set { supportInfos = value; }
        }
        
    } 
}