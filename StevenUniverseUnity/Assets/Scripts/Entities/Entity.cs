using UnityEngine;
using StevenUniverse.FanGame.Entities.Customization;
using StevenUniverse.FanGame.Entities.EntityDrivers;
using StevenUniverse.FanGame.Util;

namespace StevenUniverse.FanGame.Entities
{
    [System.Serializable]
    public class Entity : JsonBase<Entity>
    {
        //Name
        [SerializeField] private string entityName;
        //Starting variables
        [SerializeField] private string directionName;
        [SerializeField] private string stateName;
        //Outfit
        [SerializeField] private Outfit outfit;

        private EntityDriver entityDriver;

        //Events
        public delegate void GenericEventHandler();

        public event GenericEventHandler OnDirectionChange;
        public event GenericEventHandler OnStateChange;
        public event GenericEventHandler OnInteract;

        public Entity(string characterName, string directionName, string stateName, Outfit startingOutfit)
        {
            this.entityName = characterName;
            this.directionName = directionName;
            this.stateName = stateName;
            this.outfit = startingOutfit;
        }

        //Name
        public string EntityName
        {
            get { return entityName; }
            set { entityName = value; }
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

        public virtual void Interact(Entity interactor)
        {
            //Face the interactor upon being interacted with
            CurrentDirection = interactor.CurrentDirection.Opposite;

            if (OnInteract != null)
            {
                OnInteract();
            }
        }

        public EntityDriver EntityDriver
        {
            get { return entityDriver; }
            set { entityDriver = value; }
        }

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
}