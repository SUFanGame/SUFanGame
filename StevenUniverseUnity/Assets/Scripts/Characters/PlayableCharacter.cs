using UnityEngine;
using StevenUniverse.FanGame.Data;
using StevenUniverse.FanGame.Characters.Customization;

namespace StevenUniverse.FanGame.Characters
{
    [System.Serializable]
    public class PlayableCharacter : Character
    {
        public static PlayableCharacter GetPlayer(string playerAppDataPath)
        {
            return Get<PlayableCharacter>(playerAppDataPath);
        }

        [SerializeField]
        private SupportInfo[] supportList; //All the other units this one can support
        

        public PlayableCharacter(
            string characterName,
            string affiliation,
            Outfit startingOutfit,
            SaveData saveData,
            SupportInfo[] supportList)
            : base(characterName, affiliation, startingOutfit, saveData)
        {
            this.supportList = supportList;
        }

        //Supportable characters
        public SupportInfo[] SupportList
        {
            get { return supportList; }
            set { supportList = value; }
        }

    }

    [System.Serializable]
    public class SupportInfo
    {
        //Any relevant information about a character's relationship
        private Character other;
        private int supportLevel;
        private bool canFuse;

        public SupportInfo(Character other, bool canFuse)
        {
            this.other = other;
            this.canFuse = canFuse;
            supportLevel = 0;
        }

        public Character Other
        {
            get { return other; }
        }

        public int SupportLevel
        {
            get { return supportLevel; }
            set { supportLevel = value; }
        }

        public bool CanFuse
        {
            get { return canFuse; }
        }

    }
}