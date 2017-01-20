using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Events sent from animations for the UI to respond to
/// </summary>
public enum CombatAnimEvent
{
    NONE,
    PRE_ATTACK,
    ATTACK_HIT,
    ANIM_COMPLETE,
}
