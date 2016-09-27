using UnityEngine;

namespace StevenUniverse.FanGame.Util.Components.Animators
{
    public class CharacterAnimator : MonoBehaviour
    {
        /*
		[SerializeField] private CharacterAnimation standingAnimation = new CharacterAnimation();
		[SerializeField] private CharacterAnimation walkingAnimation = new CharacterAnimation();
		[SerializeField] private CharacterAnimation runningAnimation = new CharacterAnimation();
		[SerializeField] private CharacterAnimation bikingAnimation = new CharacterAnimation();

		private CharacterAnimation currentAnimation;
		private CharacterX target;

		Dictionary<CharacterX.States, CharacterAnimation> animationStates = new Dictionary<CharacterX.States, CharacterAnimation>();

		private float timeOfCurrentAnimationStart;

		protected void Awake()
		{
			animationStates.Add (CharacterX.States.Standing, standingAnimation);
			animationStates.Add (CharacterX.States.Walking, walkingAnimation);
			animationStates.Add (CharacterX.States.Running, runningAnimation);
			animationStates.Add (CharacterX.States.Biking, bikingAnimation);
		}

		protected void Start()
		{
			UpdateAnimation ();
			timeOfCurrentAnimationStart = Time.time;
		}

		protected void Update()
		{
			if ( Target != null )
			{
				GetComponent<SpriteRenderer>().sprite = CurrentSprites[CurrentFrame];
			}

			UpdateAnimation ();
		}

		private void UpdateAnimation ()
		{
			CharacterAnimation lastAnimation = currentAnimation;
			currentAnimation = animationStates [Target.State];

			//Reset the time of current animation start if the animation has changed
			//TODO why does this break character movement? not player movement, just characters
			if ( currentAnimation != lastAnimation ) { timeOfCurrentAnimationStart = Time.time; }
		}

		public Sprite[] CurrentSprites
		{
			get
			{
				return currentAnimation.GetSpritesForDirection(Target.Direction);
			}
		}

		private float CurrentSecondsPerFrame { get { return currentAnimation.SecondsPerLoop / CurrentSprites.Length; } }

		private float TimeSinceCurrentAnimationStart { get { return Time.time - timeOfCurrentAnimationStart; } }

		private int CurrentFrame { get { return Mathf.FloorToInt( TimeSinceCurrentAnimationStart / CurrentSecondsPerFrame ) % CurrentSprites.Length; } }

		private CharacterX Target
		{
			get
			{
				if ( target == null ) { target = gameObject.GetComponentInParent<CharacterX> (); }
				return target;
			}
		}

		public CharacterAnimation IdleAnimation { get { return standingAnimation; } set { standingAnimation = value; } }
		public CharacterAnimation WalkingAnimation { get { return walkingAnimation; } set { walkingAnimation = value; } }
		public CharacterAnimation RunningAnimation { get { return runningAnimation; } set { runningAnimation = value; } }

		[System.Serializable]
		public class CharacterAnimation
		{
			[SerializeField] private float secondsPerLoop;

			[SerializeField] private Sprite[] downAnimation = new Sprite[] {};
			[SerializeField] private Sprite[] upAnimation = new Sprite[] {};
			[SerializeField] private Sprite[] leftAnimation = new Sprite[] {};
			[SerializeField] private Sprite[] rightAnimation = new Sprite[] {};

			public CharacterAnimation()
			{

			}

			public Sprite[] GetSpritesForDirection ( Direction direction )
			{
				switch ( direction )
				{
				case Directions.Down: return downAnimation;
				case Directions.Up: return upAnimation;
				case Directions.Left: return leftAnimation;
				case Directions.Right: return rightAnimation;
				default: throw new UnityException("Unsupported direction");
				}
			}

			public float SecondsPerLoop { get { return secondsPerLoop; } set { secondsPerLoop = value; } }

			public Sprite[] DownAnimation { get { return downAnimation; } set { downAnimation = value; } }
			public Sprite[] UpAnimation { get { return upAnimation; } set { upAnimation = value; } }
			public Sprite[] LeftAnimation { get { return leftAnimation; } set { leftAnimation = value; } }
			public Sprite[] RightAnimation { get { return rightAnimation; } set { rightAnimation = value; } }
		}
        */
    }
}