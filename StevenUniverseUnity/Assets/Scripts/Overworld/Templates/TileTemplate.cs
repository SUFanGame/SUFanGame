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
        [SerializeField] private string[] animationSpriteNames;
        [SerializeField] private bool syncAnimation;
        [SerializeField] private float secondsPerFrame;
        [SerializeField] private string tileModeName;
        [SerializeField] private string tileLayerName;
        [SerializeField] private bool isGrounded;

        private Sprite[] animationSprites;

        //Constructor for on-the-fly TileTemplate creation
        public TileTemplate
        (
            Sprite[] animationSprites,
            bool syncAnimation,
            float secondsPerFrame,
            string tileModeName,
            string tileLayerName,
            bool isGrounded
        ) : base()
        {
            AnimationSprites = animationSprites;

            SyncAnimation = syncAnimation;
            SecondsPerFrame = secondsPerFrame;
            TileModeName = tileModeName;
            TileLayerName = tileLayerName;
            IsGrounded = isGrounded;
        }

        public TileTemplate()
        {
            //TODO why is this called whenever the code is saved?
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

        public string TileModeName
        {
            get { return tileModeName; }
            set { tileModeName = value; }
        }

        public string TileLayerName
        {
            get { return tileLayerName; }
            set { tileLayerName = value; }
        }

        public bool IsGrounded
        {
            get { return isGrounded; }
            set { isGrounded = value; }
        }

        public Sprite[] AnimationSprites
        {
            get
            {
                if (animationSprites.Length == 0)
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
            get { return Mode.Get(tileModeName); }
        }

        public Layer TileLayer
        {
            get { return Layer.Get(tileLayerName); }
        }

        public class Mode : EnhancedEnum<Mode>
        {
            //Instance
            private Mode(string name) : base(name)
            {
            }

            //Static Instances
            static Mode()
            {
                Add(new Mode("Normal"));
                Add(new Mode("Surface"));
                Add(new Mode("Transitional"));
                Add(new Mode("Collidable"));
            }
        }

        public class Layer : EnhancedEnum<Layer>
        {
            //Instance
            private int sortingValue;

            //Constructor
            private Layer(string name, int sortingValue) : base(name)
            {
                this.sortingValue = sortingValue;
            }

            //Properties
            public int SortingValue
            {
                get { return sortingValue; }
            }

            //Static Instances
            static Layer()
            {
                Add(new Layer("Water", 0));
                Add(new Layer("WaterOverlay", 1));
                Add(new Layer("Ground", 2));
                Add(new Layer("GroundOverlay", 3));
                Add(new Layer("Main", 4));
                Add(new Layer("CharacterBody", 5));
                Add(new Layer("CharacterFace", 6));
                Add(new Layer("CharacterHair", 7));
                Add(new Layer("CharacterShoes", 8));
                Add(new Layer("CharacterPants", 9));
                Add(new Layer("CharacterShirt", 10));
                Add(new Layer("CharacterAccessory", 11));
                Add(new Layer("CharacterHat", 12));
                Add(new Layer("Foreground", 13));
            }
        }
    }
}