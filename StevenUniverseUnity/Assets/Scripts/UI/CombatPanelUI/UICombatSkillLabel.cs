using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using SUGame.Characters.Skills;

/// <summary>
/// Script for skill label prefabs on the combat UI.
/// </summary>
public class UICombatSkillLabel : MonoBehaviour 
{
    Text text_;
    Image image_;

    void Awake()
    {
        text_ = GetComponentInChildren<Text>();
        image_ = GetComponentInChildren<Image>();
    }

    public void SetFromSkill( CombatSkill skill )
    {
        text_.text = skill.name;

    }
}
