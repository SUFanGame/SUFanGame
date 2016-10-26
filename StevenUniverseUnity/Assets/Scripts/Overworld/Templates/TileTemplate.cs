using System.Collections.Generic;
using System.IO;
using UnityEngine;
using StevenUniverse.FanGame.Util;

namespace StevenUniverse.FanGame.Overworld.Templates
{
    [System.Serializable]
    public class TileTemplate : Template
    {
        //Class
        public static TileTemplate GetTileTemplate(string tileTemplateAppDataPath)
        {
            return Get<TileTemplate>(tileTemplateAppDataPath);
        }

        //Instance
        [SerializeField]
        private string[] animationSpriteNames;
        [SerializeField]
        private bool syncAnimation;
        [SerializeField]
        private float secondsPerFrame;
        [SerializeField]
        private bool isGrounded;
        [SerializeField]
        private bool usableIndividually;
        [SerializeField]
        private Mode tileMode;
        [SerializeField]
        private Layer tileLayer;

        private Sprite[] animationSprites = new Sprite[0];

        //Constructor for on-the-fly TileTemplate creation
        public TileTemplate
        (
            Sprite[] animationSprites,
            bool syncAnimation,
            float secondsPerFrame,
            Mode tileMode,
            Layer tileLayer,
            bool isGrounded,
            bool usableIndividually
        ) : base()
        {
            AnimationSprites = animationSprites;

            SyncAnimation = syncAnimation;
            SecondsPerFrame = secondsPerFrame;
            TileMode = tileMode;
            TileLayer = tileLayer;
            IsGrounded = isGrounded;
            UsableIndividually = usableIndividually;
        }

        public TileTemplate()
        {

        }

        public Sprite GetCachedSprite(int frame)
        {
            try
            {
                //TODO in the Bitmap implementation, this cloned the return value. Is this still neccesary with Sprites?
                return AnimationSprites[frame];
            }
            catch (System.Exception e)
            {
                Debug.Log(frame);
                Debug.Log(Name);
                throw e;
            }
        }

        //Properties
        public string[] AnimationSpriteNames
        {
            get { return animationSpriteNames; }
            set { animationSpriteNames = value; }
        }

        public float SecondsPerFrame
        {
            get { return secondsPerFrame; }
            set { secondsPerFrame = value; }
        }

        public bool SyncAnimation
        {
            get { return syncAnimation; }
            set { syncAnimation = value; }
        }

        public bool UsableIndividually
        {
            get { return usableIndividually; }
            set { usableIndividually = value; }
        }

        public Sprite[] AnimationSprites
        {
            get
            {
                if (animationSprites == null || animationSprites.Length == 0)
                {
                    List<Sprite> loadedSprites = new List<Sprite>();
                    foreach (string animationSpriteName in AnimationSpriteNames)
                    {
                        string animationSpriteAppDataPath = Utilities.ConvertPathToParentDirectoryPath(AppDataPath) +
                                                            "/" + animationSpriteName;
                        string animationSpriteAbsolutePath = Utilities.ExternalDataPath + "/" +
                                                             animationSpriteAppDataPath + ".png";
                        //sprites[x, y] = Sprite.Create(tileTexture, new Rect(0, 0, PIXELS_PER_TILE, PIXELS_PER_TILE), Vector2.zero, 16f);

                        //Load the Texture
                        Texture2D loadedTexture = new Texture2D(16, 16);
                        loadedTexture.filterMode = FilterMode.Point;
                        loadedTexture.LoadImage(File.ReadAllBytes(animationSpriteAbsolutePath));

                        //Create the Sprite
                        Sprite loadedSprite = Sprite.Create(loadedTexture,
                            new Rect(0, 0, loadedTexture.width, loadedTexture.height), Vector2.zero, 16f);
                        loadedSprites.Add(loadedSprite);
                    }
                    animationSprites = loadedSprites.ToArray();
                }

                return animationSprites;
            }
            private set { animationSprites = value; }
        }

        public Mode TileMode
        {
            get { return tileMode; }
            set { tileMode = value; }
        }

        public Layer TileLayer
        {
            get { return tileLayer; }
            set { tileLayer = value; }
        }

        public bool IsGrounded
        {
            get { return isGrounded; }
            set { isGrounded = value; }
        }

        public enum Mode
        {
            Normal,
            Surface,
            Transitional,
            Collidable
        }

        public enum Layer
        {
            Water,
            WaterOverlay,
            Ground,
            GroundOverlay,
            Main,
            CharacterBody,
            CharacterFace,
            CharacterHair,
            CharacterShoes,
            CharacterPants,
            CharacterShirt,
            CharacterAccessory,
            CharacterHat,
            Foreground
        }
    }

    public static class LayerExtensions
    {
        public static int GetSortingValue(this TileTemplate.Layer tileLayer)
        {
            return (int)tileLayer;
        }
    }
}