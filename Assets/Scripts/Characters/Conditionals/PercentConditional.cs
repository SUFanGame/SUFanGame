using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SUGame.Characters.Skills.Conditionals
{

    [CreateAssetMenu(fileName = "PercentCondition", menuName = "Conditionals/Percent Condition", order = 9002)]
    public class PercentConditional : Conditional
    {
        [SerializeField]
        [Range(0, 100)]
        int chance_ = 50;

        public override bool Success()
        {
            return Random.Range(1, 101) <= chance_;
        }
    }
}