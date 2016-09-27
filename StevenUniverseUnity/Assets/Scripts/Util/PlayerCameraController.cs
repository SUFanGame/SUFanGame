using UnityEngine;

namespace StevenUniverse.FanGame.Util
{
    public class PlayerCameraController : MonoBehaviour
    {
        private const float MIN_VOLUME = 0.0f;
        private const float MAX_VOLUME = 1.0f;
        private const float FADE_OUT_AMOUNT = 0.025f;

        private static float textureSize = 16f;
        private static float unitsPerPixel = 1f/textureSize;
        private static float imageScale = 2f;

        private bool fadeAudio = false;

        private AudioClip targetClip = null;
        private AudioClip overrideClip = null;

        //private SpriteRenderer filterSpriteRenderer;

        protected void Start()
        {
            MatchPixelGrid();
            //filterSpriteRenderer = gameObject.FindChildWithName("Filter").GetComponent<SpriteRenderer>();
        }

        private void MatchPixelGrid()
        {
            GetComponent<Camera>().orthographicSize = ((Screen.height/2f)*unitsPerPixel)/imageScale;
            //HACK this is the current standard way of avoiding sampling the edges of pixels until Unity offers a better solution
            transform.position += new Vector3(-0.1f, -0.1f, 0f);
        }

        protected void Update()
        {
        }

        protected void FixedUpdate()
        {
            AudioSource source = this.GetComponent<AudioSource>();

            if (OverrideClip == null)
            {
                if (fadeAudio)
                {
                    if (source.volume > MIN_VOLUME)
                    {
                        source.volume -= FADE_OUT_AMOUNT;
                    }
                    else
                    {
                        source.volume = MAX_VOLUME;
                        source.clip = targetClip;
                        source.Play();
                        fadeAudio = false;
                    }
                }
            }
            else
            {
                if (fadeAudio)
                {
                    source.volume = MAX_VOLUME;
                    source.clip = OverrideClip;
                    source.Play();
                    fadeAudio = false;
                }
            }
        }

        public void SetDay()
        {
            //filterSpriteRenderer.color = new Color(1f, 1f, 1f, 0f);
        }

        public void SetNight()
        {
            //filterSpriteRenderer.color = new Color(0.015f, 0.011f, 0.105f, 0.725f);
        }

        public AudioClip TargetClip
        {
            get { return targetClip; }
            set
            {
                targetClip = value;
                fadeAudio = true;
            }
        }

        public AudioClip OverrideClip
        {
            get { return overrideClip; }
            set
            {
                overrideClip = value;
                fadeAudio = true;
            }
        }

        public void PlayAudioClip(AudioClip clipToPlay)
        {
            //If the zone has background music and it is not the music the main camera is already playing, play it
            if (clipToPlay != null && clipToPlay != TargetClip)
            {
                TargetClip = clipToPlay;
            }
        }
    }
}