using System.Collections.Generic;
using UnityEngine;
using StevenUniverse.FanGame.Overworld;
using StevenUniverse.FanGame.Overworld.Instances;
using StevenUniverse.FanGame.Util;

namespace StevenUniverse.FanGame.Characters.Customization
{
    [System.Serializable]
    public class Outfit
    {
        public Outfit(string bodyName, string hatName, string eyesName, string shirtName)
        {
            this.bodySpriteSheetName = bodyName;
            this.hatSpriteSheetName = hatName;
            this.eyesSpriteSheetName = eyesName;
            this.shirtSpriteSheetName = shirtName;
        }

        public delegate void GenericEventHandler();

        public event GenericEventHandler OnSpriteChange;

        [SerializeField] private string bodySpriteSheetName;
        [SerializeField] private string hatSpriteSheetName;
        [SerializeField] private string eyesSpriteSheetName;
        [SerializeField] private string shirtSpriteSheetName;

        private CustomizationItem body;
        private CustomizationItem hat;
        private CustomizationItem eyes;
        private CustomizationItem shirt;

        private Dictionary<State, Dictionary<Direction, Chunk>> animations;

        //Instance Properties
        public string BodySpriteSheetName
        {
            get { return bodySpriteSheetName; }
            set
            {
                bodySpriteSheetName = value;
                body = null;
                DoOnSpriteChange();
            }
        }

        public string HatSpriteSheetName
        {
            get { return hatSpriteSheetName; }
            set
            {
                hatSpriteSheetName = value;
                hat = null;
                DoOnSpriteChange();
            }
        }

        public string EyesSpriteSheetName
        {
            get { return eyesSpriteSheetName; }
            set
            {
                eyesSpriteSheetName = value;
                eyes = null;
                DoOnSpriteChange();
            }
        }

        public string ShirtSpriteSheetName
        {
            get { return shirtSpriteSheetName; }
            set
            {
                shirtSpriteSheetName = value;
                shirt = null;
                DoOnSpriteChange();
            }
        }

        public Sprite BodySpriteSheet
        {
            get
            {
                return
                    Resources.Load<Sprite>(string.Format("Textures/CustomizationItems/Bodies/{0}", BodySpriteSheetName));
            }
            set { BodySpriteSheetName = value.name; }
        }

        public Sprite HatSpriteSheet
        {
            get
            {
                return Resources.Load<Sprite>(string.Format("Textures/CustomizationItems/Hats/{0}", HatSpriteSheetName));
            }
            set { HatSpriteSheetName = value.name; }
        }

        public Sprite EyesSpriteSheet
        {
            get
            {
                return Resources.Load<Sprite>(string.Format("Textures/CustomizationItems/Eyes/{0}", EyesSpriteSheetName));
            }
            set { EyesSpriteSheetName = value.name; }
        }

        public Sprite ShirtSpriteSheet
        {
            get
            {
                return
                    Resources.Load<Sprite>(string.Format("Textures/CustomizationItems/Shirts/{0}", ShirtSpriteSheetName));
            }
            set { ShirtSpriteSheetName = value.name; }
        }

        public CustomizationItem Body
        {
            get
            {
                if (body == null)
                {
                    body = CustomizationItem.GetFromSpriteSheet(BodySpriteSheet);
                }
                return body;
            }
        }

        public CustomizationItem Hat
        {
            get
            {
                if (hat == null)
                {
                    hat = CustomizationItem.GetFromSpriteSheet(HatSpriteSheet);
                }
                return hat;
            }
        }

        public CustomizationItem Eyes
        {
            get
            {
                if (eyes == null)
                {
                    eyes = CustomizationItem.GetFromSpriteSheet(EyesSpriteSheet);
                }
                return eyes;
            }
        }

        public CustomizationItem Shirt
        {
            get
            {
                if (shirt == null)
                {
                    shirt = CustomizationItem.GetFromSpriteSheet(ShirtSpriteSheet);
                }
                return shirt;
            }
        }

        private void DoOnSpriteChange()
        {
            Animations.Clear();
            if (OnSpriteChange != null)
            {
                OnSpriteChange();
            }
        }

        private Dictionary<State, Dictionary<Direction, Chunk>> Animations
        {
            get
            {
                if (animations == null)
                {
                    animations = new Dictionary<State, Dictionary<Direction, Chunk>>();
                }

                return animations;
            }
        }

        public Chunk GetChunk(State state, Direction direction)
        {
            //Add an index for the State if there isn't one
            if (!Animations.ContainsKey(state))
            {
                Animations.Add(state, new Dictionary<Direction, Chunk>());
            }

            //Add an index for the Direction if there isn't one
            if (!Animations[state].ContainsKey(direction))
            {
                Chunk generatedChunk = null;
                if (state == State.Standing)
                {
                    //Determine the starting row
                    int startingRow = 0;
                    if (direction == Direction.Down)
                    {
                        startingRow = 0;
                    }
                    else if (direction == Direction.Up)
                    {
                        startingRow = 2;
                    }
                    else if (direction == Direction.Left)
                    {
                        startingRow = 4;
                    }
                    else if (direction == Direction.Right)
                    {
                        startingRow = 6;
                    }

                    generatedChunk = GenerateChunk(new int[] {0}, startingRow);
                }
                else if (state == State.Walking)
                {
                    //Determine the starting row
                    int startingRow = 0;
                    if (direction == Direction.Down)
                    {
                        startingRow = 0;
                    }
                    else if (direction == Direction.Up)
                    {
                        startingRow = 2;
                    }
                    else if (direction == Direction.Left)
                    {
                        startingRow = 4;
                    }
                    else if (direction == Direction.Right)
                    {
                        startingRow = 6;
                    }

                    generatedChunk = GenerateChunk(new int[] {1, 0, 2, 0}, startingRow);
                }
                Animations[state].Add(direction, generatedChunk);
            }

            return Animations[state][direction];
        }

        private Chunk GenerateChunk(int[] frames, int startingRow)
        {
            return new Chunk
            (
                new TileInstance[]
                {
                    //Body
                    new TileInstance(Body.GetTileTemplate(frames, startingRow), 0, 1, 1),
                    new TileInstance(Body.GetTileTemplate(frames, startingRow + 1), 0, 0, 0),
                    //Hat
                    new TileInstance(Hat.GetTileTemplate(frames, startingRow), 0, 1, 1),
                    new TileInstance(Hat.GetTileTemplate(frames, startingRow + 1), 0, 0, 0),
                    //Eyes
                    new TileInstance(Eyes.GetTileTemplate(frames, startingRow), 0, 1, 1),
                    new TileInstance(Eyes.GetTileTemplate(frames, startingRow + 1), 0, 0, 0),
                    //Shirt
                    new TileInstance(Shirt.GetTileTemplate(frames, startingRow), 0, 1, 1),
                    new TileInstance(Shirt.GetTileTemplate(frames, startingRow + 1), 0, 0, 0),
                },
                new GroupInstance[] {}
            );
        }

    }
}