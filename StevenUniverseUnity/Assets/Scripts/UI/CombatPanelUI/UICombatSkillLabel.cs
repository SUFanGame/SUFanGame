using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using SUGame.Characters.Skills;

namespace SUGame.StrategyMap.UI.CombatPanelUI
{
    /// <summary>
    /// Script for skill label prefabs on the combat UI.
    /// </summary>
    public class UICombatSkillLabel : MonoBehaviour
    {
        Text text_;
        //Image image_;
        CanvasGroup canvasGroup_;
        public CanvasGroup CanvasGroup_ { get { return canvasGroup_; } }

        void Awake()
        {
            text_ = GetComponentInChildren<Text>();
            //image_ = GetComponentInChildren<Image>();
            canvasGroup_ = GetComponent<CanvasGroup>();
        }

        public void SetFromSkill(CombatSkill skill)
        {
            text_.text = skill.name;

        }

        public void SetAlpha(float f)
        {
            canvasGroup_.alpha = f;
        }
    }
}