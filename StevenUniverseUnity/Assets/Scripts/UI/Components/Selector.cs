using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StevenUniverse.FanGame.Extensions;
using StevenUniverse.FanGame.Characters;

namespace StevenUniverse.FanGame.UI.Components
{
    public class Selector : MonoBehaviour
    {
        private Character target;
        private string itemGroup;
        private string[] itemNames;

        public void Init(Character target, string itemGroup, string[] itemNames)
        {
            this.target = target;
            this.itemGroup = itemGroup;
            this.itemNames = itemNames;
        }

        // Use this for initialization
        void Start()
        {
            //Get the item Sprites
            string resourceDirectoryPath = "Textures/CustomizationItems/" + itemGroup;
            List<Sprite> itemSprites = new List<Sprite>();
            foreach (string itemName in itemNames)
            {
                string itemResourcePath = resourceDirectoryPath + "/" + itemName;
                itemSprites.Add(Resources.Load<Sprite>(itemResourcePath));
            }

            //Get the target's current CustomizationItem
            Sprite currentSprite = SpriteSheet;
            if (!itemSprites.Contains(currentSprite))
            {
                throw new UnityException("Current sprite was not one of the options!");
            }
            int indexOfCurrent = itemSprites.IndexOf(currentSprite);

            //Get the different components of the Selector
            Text label = gameObject.FindChildWithName("Label").GetComponent<Text>();
            Text current = gameObject.FindChildWithName("Current").GetComponent<Text>();
            Button leftButton = gameObject.FindChildWithName("Left").GetComponent<Button>();
            Button rightButton = gameObject.FindChildWithName("Right").GetComponent<Button>();

            label.text = GetLabel(itemGroup);
            current.text = currentSprite.name;

            //Set up the left button
            leftButton.onClick.AddListener
            (
                delegate
                {
                    //Get the index one to the left of the current index. If the current index is zero, loop to the end
                    int leftIndex = indexOfCurrent == 0 ? itemSprites.Count - 1 : indexOfCurrent - 1;
                    Sprite leftSprite = itemSprites[leftIndex];
                    SpriteSheet = leftSprite;
                    current.text = leftSprite.name;
                    indexOfCurrent = leftIndex;
                }
            );

            //Set up the right button
            rightButton.onClick.AddListener
            (
                delegate
                {
                    //Get the index one to the right of the current index. If the current index is the last, loop to the start
                    int rightIndex = indexOfCurrent == itemSprites.Count - 1 ? 0 : indexOfCurrent + 1;
                    Sprite rightSprite = itemSprites[rightIndex];
                    SpriteSheet = rightSprite;
                    current.text = rightSprite.name;
                    indexOfCurrent = rightIndex;
                }
            );
        }

        private Sprite SpriteSheet
        {
            get
            {
                switch (itemGroup)
                {
                    case "Bodies":
                        return target.Outfit.BodySpriteSheet;
                    case "Eyes":
                        return target.Outfit.EyesSpriteSheet;
                    case "Hats":
                        return target.Outfit.HatSpriteSheet;
                    case "Shirts":
                        return target.Outfit.ShirtSpriteSheet;
                    default:
                        throw new UnityException("Invalid item group");
                }
            }
            set
            {
                switch (itemGroup)
                {
                    case "Bodies":
                        target.Outfit.BodySpriteSheet = value;
                        break;
                    case "Eyes":
                        target.Outfit.EyesSpriteSheet = value;
                        break;
                    case "Hats":
                        target.Outfit.HatSpriteSheet = value;
                        break;
                    case "Shirts":
                        target.Outfit.ShirtSpriteSheet = value;
                        break;
                    default:
                        throw new UnityException("Invalid item group");
                }
            }
        }

        private string GetLabel(string groupName)
        {
            switch (itemGroup)
            {
                case "Bodies":
                    return "Body";
                case "Eyes":
                    return "Eyes";
                case "Hats":
                    return "Hat";
                case "Shirts":
                    return "Shirt";
                default:
                    throw new UnityException("Invalid item group");
            }
        }
    }
}
