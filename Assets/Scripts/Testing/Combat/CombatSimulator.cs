using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.StrategyMap;

public class CombatSimulator : MonoBehaviour 
{
    public MapCharacter attacker_;
    public MapCharacter defender_;
    void Start()
    {
        if (attacker_ == null)
            Debug.LogError("Attacker was null");
        if (defender_ == null)
            Debug.LogError("Defender was null");

        var combat = new Combat(attacker_, defender_);
        //combat.Initialize();
        StartCoroutine(combat.Resolve());
    }
}
