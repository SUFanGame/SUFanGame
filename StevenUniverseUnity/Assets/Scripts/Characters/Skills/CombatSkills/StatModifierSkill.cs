using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.StrategyMap;
using SUGame.StrategyMap.UI.CombatPanelUI;

namespace SUGame.Characters.Skills
{
    /// <summary>
    /// Skill that applies a stat modifier to a character during combat.
    /// </summary>
    [CreateAssetMenu(fileName = "ModifyStatSkill", menuName = "Skills/Combat/Modify Stat", order = 9000)]
    public class StatModifierSkill : CombatSkill
    {
        public Target target_ = Target.SELF;
        [SerializeField]
        List<ModifyStatEffect> effects_ = new List<ModifyStatEffect>();

        public GameObject coolUIEffects_;

        public enum Target
        {
            SELF,
            TARGET
        }

        public override void Execute(Combat combat)
        {
            var tar = target_ == Target.SELF ? combat.Attacker_ : combat.Defender_;

            foreach( var e in effects_ )
            {
                e.Execute(tar.Data.Stats_);
            }
        }

        public override IEnumerator UIRoutine(CombatPanelUnit uiPanel)
        {
            var effects = Instantiate(coolUIEffects_);
            effects.transform.position = Vector3.zero;
            effects.transform.SetParent(uiPanel.transform, true);
            effects.transform.position = Vector3.zero;
            uiPanel.StartCoroutine(ShowCoolEffects(effects));
            uiPanel.RefreshStatsTween(1f);
            yield return null;
        }

        IEnumerator ShowCoolEffects(GameObject effects)
        {
            yield return new WaitForSeconds(1f);
            Destroy(effects);
        }

    }
}
