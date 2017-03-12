using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace SUGame.Characters.Skills.Conditionals
{

    /// <summary>
    /// A conditional is essentially asset representation of a predicate.
    /// </summary>
    //[CreateAssetMenu(fileName = "Conditional", menuName = "Logic/Conditional", order = 9002)]
    public abstract class Conditional : ScriptableObject
    {
        public abstract bool Success();
    }
}
