using UnityEngine;

namespace StevenUniverse.FanGame.Util.Components.Animators
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SimpleAnimator : AbstractAnimator
    {
        [SerializeField] private SimpleAnimation simpleAnimation;

        private float timeOfCreation;

        protected void Awake()
        {
            timeOfCreation = Time.time;
            CurrentSimpleAnimation = simpleAnimation;
            CurrentSprites = CurrentSimpleAnimation.Animation;
        }

        protected override void Update()
        {
            //If the animation doesn't loop and a loop has completed, destroy the GameObject
            if (!CurrentSimpleAnimation.Loops && (TimeSinceCreation >= CurrentSimpleAnimation.SecondsPerLoop))
            {
                if (CurrentSimpleAnimation.DestroyAfterLoop)
                {
                    Destroy(gameObject);
                }
            }
            //Otherwise, update the current frame
            else
            {
                base.Update();
            }
        }

        protected override int CurrentFrame
        {
            get
            {
                if (CurrentSimpleAnimation.Synced)
                {
                    return Mathf.FloorToInt(Time.time/CurrentSecondsPerFrame)%CurrentSprites.Length;
                }
                else
                {
                    return Mathf.FloorToInt(TimeSinceCreation/CurrentSecondsPerFrame)%CurrentSprites.Length;
                }
            }
        }

        private float TimeSinceCreation
        {
            get { return Time.time - timeOfCreation; }
        }

        public SimpleAnimation CurrentSimpleAnimation
        {
            get { return CurrentAbstractAnimation as SimpleAnimation; }
            private set { CurrentAbstractAnimation = value; }
        }

        [System.Serializable]
        public class SimpleAnimation : AbstractAnimation
        {
            [SerializeField] private bool loops;

            [SerializeField] private bool destroyAfterLoop;

            [SerializeField] private bool synced;

            [SerializeField] private Sprite[] animation;

            public bool Loops
            {
                get { return loops; }
            }

            public bool DestroyAfterLoop
            {
                get { return destroyAfterLoop; }
                set { destroyAfterLoop = value; }
            }

            public bool Synced
            {
                get { return synced; }
            }

            public Sprite[] Animation
            {
                get { return animation; }
            }
        }
    }
}