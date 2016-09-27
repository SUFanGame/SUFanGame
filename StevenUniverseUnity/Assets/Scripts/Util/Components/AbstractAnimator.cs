using UnityEngine;

namespace StevenUniverse.FanGame.Util.Components
{
    public abstract class AbstractAnimator : MonoBehaviour
    {
        private Sprite[] currentSprites;
        private AbstractAnimation currentAnimation;

        protected virtual void Update()
        {
            GetComponent<SpriteRenderer>().sprite = CurrentSprites[CurrentFrame];
        }

        protected Sprite[] CurrentSprites
        {
            get { return currentSprites; }
            set { currentSprites = value; }
        }

        protected AbstractAnimation CurrentAbstractAnimation
        {
            get { return currentAnimation; }
            set { currentAnimation = value; }
        }

        protected abstract int CurrentFrame { get; }

        protected float CurrentSecondsPerFrame
        {
            get { return CurrentAbstractAnimation.SecondsPerLoop/CurrentSprites.Length; }
        }

        [System.Serializable]
        public class AbstractAnimation
        {
            [SerializeField] private float secondsPerLoop;

            public float SecondsPerLoop
            {
                get { return secondsPerLoop; }
                set { secondsPerLoop = value; }
            }
        }
    }
}