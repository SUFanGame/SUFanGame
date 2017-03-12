
namespace SUGame.Characters
{
    /// <summary>
    /// Any relevant information about a character's relationship.
    /// </summary>
    [System.Serializable]
    public class SupportInfo
    {
        [System.NonSerialized]
        private CharacterData other;
        private int supportLevel; 
        private bool canFuse;

        public SupportInfo(CharacterData other, bool canFuse)
        {
            this.other = other;
            this.canFuse = canFuse;
            supportLevel = 0;
        }

        public CharacterData Other
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
