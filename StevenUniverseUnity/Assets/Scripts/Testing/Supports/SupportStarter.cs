using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.Interactions;

public class SupportStarter : MonoBehaviour 
{
    void Start()
    {
        var scene = SupportLoader.ImportSupport("marth_rekts_pirate");
        SupportPanel.Dialog = scene;
        StartCoroutine(SupportPanel.DoDialogWithCanvas() );
    }
}
