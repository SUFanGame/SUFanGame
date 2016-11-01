using UnityEngine;
using StevenUniverse.FanGame.StrategyMap;

namespace StevenUniverse.FanGame.Interactions
{
    //DEBUGGING CLASS, hooks into Marth's "Start Drama" action
    class InteractionTests : CharacterAction
    {
        public CutsceneRunner cutsceneRunner;

        public override void Execute()
        {
            //cutsceneJsonLoadTest();
            cutsceneFullTest();
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
