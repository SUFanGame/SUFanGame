using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base class for skill effects
/// </summary>
public class SkillEffect : ScriptableObject
{
    public virtual void Execute() { }
}
