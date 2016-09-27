using UnityEngine.Events;

namespace StevenUniverse.FanGame.Interactions.Activities
{
    [System.Serializable]
    public class CodeActivity : Activity
    {
        private UnityAction action;

        public CodeActivity(int id, bool allowsControl, UnityAction action) : base(id, allowsControl)
        {
            Action = action;
        }

        public override void StartActivity()
        {
            base.StartActivity();

            action();
            IsComplete = true;
        }

        public UnityAction Action
        {
            get { return action; }
            private set { action = value; }
        }
    }
}