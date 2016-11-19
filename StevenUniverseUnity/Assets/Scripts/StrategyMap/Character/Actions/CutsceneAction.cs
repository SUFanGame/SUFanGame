﻿using UnityEngine;
using StevenUniverse.FanGame.StrategyMap;
using StevenUniverse.FanGame.Interactions;
using System.Collections;

namespace StevenUniverse.FanGame.Actions
{
    //DEBUGGING CLASS, hooks into Marth's "Start Drama" action
    class CutsceneAction : CharacterAction
    {
        public CutsceneRunner cutsceneRunner;

        public override IEnumerator Execute()
        {
            //cutsceneJsonLoadTest();
            cutsceneFullTest();
            yield return null;
        }

        // Test if a json file outputs correctly
        public void cutsceneJsonLoadTest()
        {
            string cutsceneName = "TestScript1";

            Scene[] parsedScenes = CutsceneLoader.ImportCutscene(cutsceneName);

            foreach (Scene sce in parsedScenes)
            {
                Debug.Log(sce.ToString());
                
            }
        }

        //Tests if the scene executes
        public void cutsceneFullTest()
        {
            string cutsceneName = "TestScript1";

            Scene[] parsedScenes = CutsceneLoader.ImportCutscene(cutsceneName);

            cutsceneRunner.Cutscene = parsedScenes;
            StartCoroutine(cutsceneRunner.execute());
        }

        
    }
}