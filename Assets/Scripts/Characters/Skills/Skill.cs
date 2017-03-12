
using UnityEngine;
using System.Collections.Generic;
using SUGame.Characters.Skills.Conditionals;

/// <summary>
/// Base class for all skills.
/// </summary>
namespace SUGame.Characters.Skills
{

    public class Skill : ScriptableObject
    {
        [SerializeField]
        protected List<Conditional> conditions_ = new List<Conditional>();
    }
}
