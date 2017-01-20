using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.Interactions;

public class CutsceneStarter : MonoBehaviour 
{
    [SerializeField]
    SupportRunner runner_;
    void Start()
    {
        var scene = SupportLoader.ImportSupport("marth_rekts_pirate");
        runner_.Dialog = scene;
        StartCoroutine(runner_.DoDialog() );
    }
}
