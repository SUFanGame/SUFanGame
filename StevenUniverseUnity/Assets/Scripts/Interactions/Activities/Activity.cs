using UnityEngine;

namespace StevenUniverse.FanGame.Interactions.Activities
{
    [System.Serializable]
    public class Activity
    {
        [SerializeField] private int activityID;
        [SerializeField] private bool allowsControl = false;

        private bool started = false;
        private bool isComplete = false;

        public Activity(int activityID, bool allowsControl)
        {
            this.activityID = activityID;
            this.allowsControl = allowsControl;
        }

        public virtual void Reset()
        {
            isComplete = false;
            started = false;
        }

        public virtual void StartActivity()
        {
            started = true;
        }

        public virtual void UpdateActivity()
        {
        }

        public int ActivityID
        {
            get { return activityID; }
        }

        public bool AllowsControl
        {
            get { return allowsControl; }
            private set { allowsControl = value; }
        }

        public bool Started
        {
            get { return started; }
        }

        public bool IsComplete
        {
            get { return isComplete; }
            protected set { isComplete = value; }
        }
    }
}