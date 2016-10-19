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

        //Unchanged from original Entity, consider changing
        [SerializeField]
        private Outfit outfit; // TO-DO change to all graphical stuff
        [SerializeField]
        private string stateName; //State, needs to be reworked
        [SerializeField]
        private State currentState; //Current State

        // Location Data
        [SerializeField]
        private string sceneName;
        [SerializeField]
        private int xPosition;
        [SerializeField]
        private int yPosition;
        [SerializeField]
        private int elevation;
        [SerializeField]
        private string directionName;

        [SerializeField]
        private SaveData savedData;

        //Events
        public delegate void GenericEventHandler();

        public event GenericEventHandler OnDirectionChange;
        public event GenericEventHandler OnStateChange;
        public event GenericEventHandler OnInteract;

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

        //Location info
        public string SceneName
        {
            get { return sceneName; }
            set { sceneName = value; }
        }

        public int XPosition
        {
            get { return xPosition; }
            set { xPosition = value; }
        }

        public int YPosition
        {
            get { return yPosition; }
            set { yPosition = value; }
        }

        public int Elevation
        {
            get { return elevation; }
            set { elevation = value; }
        }

        public SaveData SavedData
        {
            get { return savedData; }
            set { savedData = value; }
        }

        //State
        public string StateName
        {
            get { return stateName; }
            set { stateName = value; }
        }

        public State CurrentState
        {
            get { return State.Get(StateName); }
            set
            {
                string lastStateName = StateName;
                StateName = value.Name;
                if (lastStateName != StateName && OnStateChange != null)
                {
                    OnStateChange();
                }
            }
        }

        //Outfit
        public Outfit Outfit
        {
            get { return outfit; }
            set { outfit = value; }
        }

        //Direction
        public string DirectionName
        {
            get { return directionName; }
            set { directionName = value; }
        }

        public Direction CurrentDirection
        {
            get { return Direction.Get(DirectionName); }
            set
            {
                string lastDirectionName = DirectionName;
                DirectionName = value.Name;
                if (lastDirectionName != DirectionName && OnDirectionChange != null)
                {
                    OnDirectionChange();
                }
            }
        }
    }


    //TO-DO rework this
    public class State : EnhancedEnum<State>
    {
        //Instance
        private State(string name) : base(name)
        {
        }

        //Static Instances
        static State()
        {
            Add(new State("Standing"));
            Add(new State("Walking"));
            Add(new State("Running"));
        }

        public static State Standing
        {
            get { return Get("Standing"); }
        }

        public static State Walking
        {
            get { return Get("Walking"); }
        }

        public static State Running
        {
            get { return Get("Running"); }
        }
    }
}