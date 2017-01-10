using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.StrategyMap;

/// <summary>
/// Base class for effects which trigger in combat.
/// </summary>
public abstract class CombatEffect : SkillEffect
{
    public abstract void Execute(Combat combat);
}
