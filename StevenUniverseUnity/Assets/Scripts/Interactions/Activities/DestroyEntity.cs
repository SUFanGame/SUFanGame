using UnityEngine;
using StevenUniverse.FanGame.Entities.EntityDrivers;
using StevenUniverse.FanGame.Util;

namespace StevenUniverse.FanGame.Interactions.Activities
{
    [System.Serializable]
    public class DestroyEntity : Activity
    {
        [SerializeField] string targetName;

        private EntityDriver target;

        public DestroyEntity(int id, bool allowsControl, string targetName) : base(id, allowsControl)
        {
            this.targetName = targetName;
        }

        public override void StartActivity()
        {
            base.StartActivity();
            GameObject.Destroy(Target.gameObject);
            IsComplete = true;
        }

        public EntityDriver Target
        {
            get
            {
                //Find the target if it hasn't been found yet
                target = GameController.Instance.FindEntity(targetName);
                return target;
            }
            private set { target = value; }
        }
    }
}