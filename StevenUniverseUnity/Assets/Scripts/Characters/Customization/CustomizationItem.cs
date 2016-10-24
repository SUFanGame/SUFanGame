using System.Collections.Generic;
using UnityEngine;
using StevenUniverse.FanGame.Overworld.Templates;

namespace StevenUniverse.FanGame.Characters.Customization
{
    public class CustomizationItem
    {
        //Static
        private const int TILES_PER_WIDTH = 3;
        private const int TILES_PER_HEIGHT = 8;

        private int tileWidth;
        private int tileHeight;

        private static Dictionary<Sprite, CustomizationItem> customizationItems =
            new Dictionary<Sprite, CustomizationItem>();

        public static CustomizationItem GetFromSpriteSheet(Sprite spriteSheet)
        {
            if (!customizationItems.ContainsKey(spriteSheet))
            {
                customizationItems.Add(spriteSheet, new CustomizationItem(spriteSheet));
            }
            return customizationItems[spriteSheet];
        }

        //Instance
        private Sprite spriteSheet;
        private Sprite[,] sprites;

        public Sprite SpriteSheet
        {
            get { return spriteSheet; }
        }

        private CustomizationItem(Sprite spriteSheet)
        {
            this.spriteSheet = spriteSheet;
            this.tileWidth = spriteSheet.texture.width/TILES_PER_WIDTH;
            this.tileHeight = spriteSheet.texture.height/TILES_PER_HEIGHT;

            sprites = new Sprite[TILES_PER_WIDTH, TILES_PER_HEIGHT];

            for (int x = 0; x < TILES_PER_WIDTH; x++)
            {
                for (int y = 0; y < TILES_PER_HEIGHT; y++)
                {
                    Texture2D tileTexture = new Texture2D(tileWidth, tileHeight);
                    tileTexture.filterMode = FilterMode.Point;
                    Color[] tileColors = SpriteSheet.texture.GetPixels(x*tileWidth, y*tileHeight, tileWidth, tileHeight);
                    tileTexture.SetPixels(tileColors);
                    tileTexture.Apply();

                    sprites[x, y] = Sprite.Create(tileTexture, new Rect(0, 0, tileWidth, tileHeight), Vector2.zero, 16f);
                }
            }
        }

        public TileTemplate GetTileTemplate(int[] cols, int row)
        {
            List<Sprite> animation = new List<Sprite>();
            foreach (int col in cols)
            {
                animation.Add(sprites[col, TILES_PER_HEIGHT - 1 - row]);
            }

            return new TileTemplate(animation.ToArray(), false, 0.2f, "Normal", "CharacterBody", false);
        }
    }
}