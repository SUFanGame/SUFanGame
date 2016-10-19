
using UnityEngine;
using StevenUniverse.FanGame.Data;
using StevenUniverse.FanGame.Characters.Customization;
using StevenUniverse.FanGame.Util;


namespace StevenUniverse.FanGame.Characters
{
    [System.Serializable]
    public class PlayableCharacter : Character
    {
        public static PlayableCharacter GetPlayer(string playerAppDataPath)
        {
            return Get<PlayableCharacter>(playerAppDataPath);
        }

        // Is this needed anymore?
        //public WarpPoint SavedWarpPoint
        //{
        //    get { return new WarpPoint("", SavedSceneName, new Vector3(SavedX, SavedY, 0f), SavedElevation); }
        //}
        
        public PlayableCharacter(
            string characterName,
            string affiliation,
            Outfit startingOutfit,
            SaveData saveData)
            : base(characterName, affiliation, startingOutfit, saveData)
        {
            // Anything go here?
        }

        // Need new default values
        //public static PlayableCharacter CreateDefaultPlayer()
        //{
        //    Outfit playerOutfit = new Outfit("Light Body", "Red Hat", "Calm Eyes", "Red Shirt");
        //    SaveData playerSaveData = new SaveData
        //    (
        //        new DataGroup[]
        //        {
        //            new DataGroup
        //            (
        //                "Intro",
        //                new DataBool[]
        //                {
        //                    new DataBool("CharacterCustomizationComplete", false),
        //                    new DataBool("RubyIntro", false),
        //                    new DataBool("SapphireIntro", false),
        //                    new DataBool("EmeraldIntro", false),
        //                    new DataBool("RubyDefeated", false),
        //                    new DataBool("SapphireDefeated", false),
        //                    new DataBool("EmeraldDefeated", false),
        //                },
        //                new DataInt[]
        //                {
        //                    new DataInt("ten", 10),
        //                    new DataInt("five", 5)
        //                }
        //            ),
        //        }
        //    );
        //    PlayableCharacter defaultPlayer = new PlayableCharacter("Brendan", "Down", "Standing", playerOutfit, "Overworld", 54, 8, 0, playerSaveData);
        //    defaultPlayer.AppDataPath = "Saves/Player";
        //
        //    return defaultPlayer;
        //}

        
        public override void Save()
        {
            // Need to reconsider what is unique to PlayableCharacter that needs to be saved
            base.Save();
        }

        
    }
}