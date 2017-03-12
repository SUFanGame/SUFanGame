using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Note: Currently only uses combat events, eventually this can have
// things like "OnTurnEnd", "OnMoved", etc that can occur in the
// strategy map as well. This way we can have things like
// "poison" that might occur in combat but tick off outside
// of combat.

/// <summary>
/// Common game events
/// </summary>
public enum GameEvent
{
    NONE = 0,
    // Occurs ONCE per combat, before the first attack
    EVT_PRE_COMBAT = 1,
    // Occurs just before a unit attacks, on every attack.
    EVT_PRE_ATTACK = 2,
    // Occurs when an attacker misses.
    EVT_MISS = 3,
    // Occurs when an attacker deals damage.
    EVT_DEALT_DAMAGE = 4,
    // Occurs when a defender takes damage
    EVT_TOOK_DAMAGE = 5,
    // Occurs when an attacker kills a defender.
    EVT_KILL = 6,
    // Occurs when a defender dies.
    EVT_KILLED = 7,
    // Occurs after an attack, on every attack.
    EVT_POST_ATTACK = 8,
    // Occurs ONCE per combat, after the last attack.
    EVT_POST_COMBAT = 9,
};