using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.Characters.Skills;

/// <summary>
/// Layout panel that <seealso cref="UICombatSkillLabel"/>s will be parented to
/// when skills occur in combat.
/// </summary>
public class UICombatSkillPanel : MonoBehaviour 
{
    UICombatSkillLabel label_;
    public void ShowLabel( UICombatSkillLabel prefab, CombatSkill skill )
    {
        label_ = Instantiate(prefab);
        label_.SetFromSkill(skill);
        label_.transform.SetParent(transform);
    }

    public void RemoveLabel()
    {
        DestroyImmediate(label_.gameObject);
    }
}
