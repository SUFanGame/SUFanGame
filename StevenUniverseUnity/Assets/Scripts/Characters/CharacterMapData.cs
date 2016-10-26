using UnityEngine;
using StevenUniverse.FanGame.Util;

namespace StevenUniverse.FanGame.Characters
{
    [System.Serializable]
    class CharacterMapData : MonoBehaviour
    {
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
        private Direction direction;

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

        public State CurrentState
        {
            get { return currentState; } set { currentState = value; }
        }

        //Direction
        public string DirectionName
        {
            get { return directionName; }
            set { directionName = value; }
        }

        public Direction CurrentDirection
        {
            get { return direction; }
            set
            {
                Direction lastDirection = direction;
                direction = value;
                if (lastDirection != CurrentDirection && OnDirectionChange != null)
                {
                    OnDirectionChange();
                }
            }
        } 
    }

    //TO-DO rework this
    public enum State
    {
        Standing,
        Walking,
        Running
    }
}
