using UnityEngine;
using System.Collections.Generic;
using SUGame.Characters.Customization;
using SUGame.Factions;
using SUGame.Items;
using SUGame.Characters.Skills;
using System.Linq;
using SUGame.PropertyAttributes;

namespace SUGame.Characters
{
    /// <summary>
    /// Base character data, intended to entirely encapsulate all information needed to load characters in different contexts.
    /// </summary>
    [System.Serializable]
    public class CharacterData
    {
        
        [SerializeField]
        private string characterName_; //Name
        [SerializeField]
        private Faction faction_; //What team?

        [SerializeField]
        private Stats stats_ = new Stats();

        [SerializeField]
        private List<Item> heldItems_; //WILDCATS

        [SerializeField]
        private Weapon equippedWeapon_ = null;
        /// <summary>
        /// The weapon this character currently has equipped.
        /// </summary>
        public Weapon EquippedWeapon_ { get { return equippedWeapon_; } }

        [SerializeField]
        private List<Skill> skills_; //All available skills
        
        private List<CombatSkill> combatSkills_;
        /// <summary>
        /// List of the character's combat skills.
        /// </summary>
        public IList<CombatSkill> CombatSkills_ { get; private set; }

        [SerializeField]
        private SupportInfo[] supportInfos_;


        
        public CharacterData()
        {

        }

        public CharacterData(  
            string characterName,
            string affiliation
            ) : this()
        {
            this.characterName_ = characterName;
            this.faction_ = (Faction)System.Enum.Parse(typeof(Faction), affiliation, true );

            // All other data parameters may want to be loaded from SaveData at instantiation.
        }

        /// <summary>
        /// Called from monobehaviour during awake.
        /// </summary>
        public void Awake()
        {
            BuildSkillLists();
            
            stats_.Awake();
        }

        void BuildSkillLists()
        {
            if( skills_ != null )
            {
                //Debug.Log("Adding combat skills to " + characterName_);
                if (combatSkills_ == null)
                    combatSkills_ = new List<CombatSkill>();
                if (CombatSkills_ == null)
                    CombatSkills_ = combatSkills_.AsReadOnly();

                combatSkills_.Clear();
                GetSkills(combatSkills_);
                //Debug.LogFormat("Combat skills after refresh for {0} are null: {1}. Read only null: {2}", characterName_, combatSkills_ == null, CombatSkills_ == null);
            }
        }


        //Name
        public string CharacterName_
        {
            get { return characterName_; }
            set { characterName_ = value; }
        }

        //Team name
        public Faction Faction_
        {
            get { return faction_; }
            set { faction_ = value; }
        }

        //Held items
        public List<Item> HeldItems_
        {
            get { return heldItems_; }
            set { heldItems_ = value; }
        }
        
        //Skills available
        public List<Skill> Skills_
        {
            get { return skills_; }
            set { skills_ = value; }
        }
        
        //Unit stats
        public Stats Stats_
        {
            get { return stats_; }
            set { stats_ = value; }
        }

        public SupportInfo[] SupportInfos_
        {
            get { return supportInfos_; }
            set { supportInfos_ = value; }
        }
        
        /// <summary>
        /// Populate the given buffer with all skills of type T
        /// </summary>
        public void GetSkills<T>( List<T> buffer ) where T : Skill
        {
            foreach (var s in skills_)
            {
                var t = s as T;
                if (t != null)
                    buffer.Add(t);
            }
        }
    } 
}