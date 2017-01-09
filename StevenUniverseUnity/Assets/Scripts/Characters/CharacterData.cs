using UnityEngine;
using System.Collections.Generic;
using SUGame.Characters.Customization;
using SUGame.Factions;
using SUGame.Items;
using SUGame.Characters.Skills;

namespace SUGame.Characters
{
    /// <summary>
    /// Base character data, intended to entirely encapsulate all information needed to load characters in different contexts.
    /// </summary>
    [System.Serializable]
    public class CharacterData
    {
        
        [SerializeField]
        private string characterName; //Name
        [SerializeField]
        private Faction faction; //What team?

        [SerializeField]
        private Stats stats; //All the unit battle modifiers

        [SerializeField]
        private List<Item> heldItems; //WILDCATS
        [SerializeField]
        private Skill[] skills; //All available skills
        [SerializeField]
        private SupportInfo[] supportInfos;

        public CharacterData(  
            string characterName,
            string affiliation
            )
        {
            this.characterName = characterName;
            this.faction = (Faction)System.Enum.Parse(typeof(Faction), affiliation, true );

            // All other data parameters may want to be loaded from SaveData at instantiation.
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
        public List<Item> HeldItems
        {
            get { return heldItems; }
            set { heldItems = value; }
        }
        
        //Skills available
        public Skill[] Skills
        {
            get { return skills; }
            set { skills = value; }
        }
        
        //Unit stats
        public Stats Stats
        {
            get { return stats; }
            set { stats = value; }
        }

        public SupportInfo[] SupportInfos
        {
            get { return supportInfos; }
            set { supportInfos = value; }
        }
        
    } 
}