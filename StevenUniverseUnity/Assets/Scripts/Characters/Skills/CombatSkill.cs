using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.StrategyMap;

namespace SUGame.Characters.Skills
{
    /// <summary>
    /// Skills which trigger in combat.
    /// </summary>
    [CreateAssetMenu( fileName ="CombatSkill", menuName ="Skills/Combat/Base", order = 9000 )]
    public class CombatSkill : Skill
    {
        [SerializeField]
        Combat.Phase phase_;
        public Combat.Phase Phase_ { get { return phase_; } }

        public List<TestA> a_ = new List<TestA>();

        public virtual IEnumerator Execute( Combat combat )
        {

            yield return null;
        }

        void AddA()
        {
            a_.Add(new TestA());
        }

        void AddB()
        {
            a_.Add(new TestDerived());
        }

        

    }
}
