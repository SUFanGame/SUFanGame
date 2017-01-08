using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace SUGame.StrategyMap.UI.ConfirmTargetUI
{
    /// <summary>
    /// Each defender and attacker will have one of these panels during target selection
    /// </summary>
    public class TargetPanel : MonoBehaviour
    {
        [SerializeField]
        Text characterNameText_;
        [SerializeField]
        Text weaponNameText_;
        [SerializeField]
        Text atkValueText_;
        [SerializeField]
        Text hitValueText_;
        [SerializeField]
        Text critValueText_;
        [SerializeField]
        Text hpValueText_;
        [SerializeField]
        Image portrait_;

        public void SetTarget( MapCharacter character )
        {
            characterNameText_.text = character.name;
            weaponNameText_.text = character.weaponName_;
            atkValueText_.text = character.Data.Stats.str.ToString();
            hitValueText_.text = character.Data.Stats.accuracy.ToString();
            hpValueText_.text = character.Data.Stats.maxHP.ToString();
            portrait_.sprite = character.portrait_;
            critValueText_.text = character.Data.Stats.crit.ToString();
            //weaponNameText_.text = character.
        }
    }
}