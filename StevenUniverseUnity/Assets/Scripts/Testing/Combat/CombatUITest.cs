using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.StrategyMap;
using SUGame.StrategyMap.UI.CombatPanelUI;

public class CombatUITest : MonoBehaviour 
{
    public MapCharacter attacker_;
    public MapCharacter defender_;
    //public CombatPanel ui_;

    public void Start()
    {
        Run();
    }

    void Run()
    {
        if (attacker_ == null)
            Debug.LogError("Attacker was null");
        if (defender_ == null)
            Debug.LogError("Defender was null");

        CombatPanel.Initialize(attacker_, defender_);
        CombatPanel.ShowTerrains();
        var combat = new Combat(attacker_, defender_);
        combat.Initialize();
        combat.Resolve();
        CombatPanel.ProcessCombatEvents(combat.CombatEvents_);
    }
}
