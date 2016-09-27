using UnityEngine;

namespace StevenUniverse.FanGame.Interactions.Activities
{
    [System.Serializable]
    public class Wait : Activity
    {
        [SerializeField] private float seconds;

        private float startTime;

        public Wait(int id, bool allowsControl, float seconds) : base(id, allowsControl)
        {
            this.seconds = seconds;
        }

        public override void StartActivity()
        {
            base.StartActivity();

            startTime = Time.time;
        }

        public override void UpdateActivity()
        {
            base.UpdateActivity();

            if (!IsComplete && SecondsSinceStartTime >= seconds)
            {
                IsComplete = true;
            }
        }

        public float SecondsSinceStartTime
        {
            get { return Time.time - startTime; }
        }
    }
}