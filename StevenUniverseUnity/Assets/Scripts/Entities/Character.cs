using UnityEngine;
using StevenUniverse.FanGame.Data;
using StevenUniverse.FanGame.Characters.Customization;
using StevenUniverse.FanGame.Util;

// This class used to be called "Entity"

namespace StevenUniverse.FanGame.Characters
{
    [System.Serializable]
    public class Character : JsonBase<Character>
    {
        
        [SerializeField]
        private string name; //Name
        [SerializeField]
        private string affiliation; //What Team?
        [SerializeField]
        private HeldItems items; //WILDCATS
        [SerializeField]
        private Skill[] skills; //All available skills
        [SerializeField]
        private UnitStats stats; //All the unit battle modifiers
        [SerializeField]
        private Character[] supportList; //All the other units this one can support

        //Unchanged from original Entity, consider changing
        [SerializeField]
        private Outfit outfit; // TO-DO change to all graphical stuff
        [SerializeField]
        private SaveData savedData;

        public Character(
            string characterName,
            string affiliation,
            Outfit startingOutfit,
            SaveData saveData)
        {
            this.name = characterName;
            this.affiliation = affiliation;
            this.outfit = startingOutfit;
            this.savedData = saveData;

            // All other data parameters may want to be loaded from SaveData at instantiation.
            // This particular class as-is is meant for in-map character entities but could be further abstracted
        }


        //Name
        public string EntityName
        {
            get { return name; }
            set { name = value; }
        }

        //Team name
        public string Affiliation
        {
            get { return affiliation; }
            set { affiliation = value; }
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

        //Supportable characters
        public Character[] SupportList
        {
            get { return supportList; }
            set { supportList = value; }
        }


        public SaveData SavedData
        {
            get { return savedData; }
            set { savedData = value; }
        }

        
        //Outfit
        public Outfit Outfit
        {
            get { return outfit; }
            set { outfit = value; }
        }

        
    } 
}