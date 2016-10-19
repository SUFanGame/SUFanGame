using UnityEngine;
using StevenUniverse.FanGame.Util;

namespace StevenUniverse.FanGame.Characters
{
    [System.Serializable]
    class CharacterMapData : MonoBehaviour
    {

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

        //Events
        public delegate void GenericEventHandler();

        public event GenericEventHandler OnDirectionChange;
        public event GenericEventHandler OnStateChange;

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
