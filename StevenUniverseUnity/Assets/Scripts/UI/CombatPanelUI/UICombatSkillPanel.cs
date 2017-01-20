using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.Characters.Skills;

namespace SUGame.StrategyMap.UI.CombatPanelUI
{
    /// <summary>
    /// Layout panel that <seealso cref="UICombatSkillLabel"/>s will be parented to
    /// when skills occur in combat.
    /// </summary>
    public class UICombatSkillPanel : MonoBehaviour
    {
        UICombatSkillLabel label_;


        public void ShowLabel(UICombatSkillLabel prefab, CombatSkill skill, float time)
        {
            if (label_ != null)
            {
                RemoveLabel();
            }

            label_ = Instantiate(prefab);
            label_.SetFromSkill(skill);
            label_.transform.SetParent(transform);
            label_.CanvasGroup_.alpha = 0;
            LeanTween.alphaCanvas(label_.CanvasGroup_, 1f, time);
        }

        public void RemoveLabel()
        {
            DestroyImmediate(label_.gameObject);
            label_ = null;
        }

        public void FadeAndRemoveLabel(float time)
        {
            LeanTween.alphaCanvas(label_.CanvasGroup_, 0, time).setOnComplete(RemoveLabel);
        }

    }
}