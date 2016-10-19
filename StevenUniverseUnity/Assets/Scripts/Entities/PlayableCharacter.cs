using StevenUniverse.FanGame.Data;
using StevenUniverse.FanGame.Characters.Customization;

// Need to think conceptually about what makes a playable character different from any other character
// Otherwise delete this

namespace StevenUniverse.FanGame.Characters
{
    [System.Serializable]
    public class PlayableCharacter : Character
    {
        public static PlayableCharacter GetPlayer(string playerAppDataPath)
        {
            return Get<PlayableCharacter>(playerAppDataPath);
        }
                
        public PlayableCharacter(
            string characterName,
            string affiliation,
            Outfit startingOutfit,
            SaveData saveData)
            : base(characterName, affiliation, startingOutfit, saveData)
        {
            // Anything go here?
        }
        
    }
}