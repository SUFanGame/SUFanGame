
namespace SUGame.Characters.Customization
{
    [System.Serializable]
    public class UnitStats
    {
        // Could this just be a struct?
        // Just holds all the unit stats. Public for editor access

        public int str;
        public int mag;
        public int spd;
        public int skl;
        public int lck;
        public int def;
        public int res;
        public int crit;

        public int maxHP;
        public int currentHP;
        public int avoidance;
        public int accuracy;

        public UnitStats()
        {

        }
    }
}
