using UnityEngine;
using SUGame.StrategyMap;
using SUGame.Interactions;
using System.Collections;

namespace SUGame.StrategyMap.Characters.Actions
{
    //DEBUGGING CLASS, hooks into Marth's "Start Drama" action
    class CutsceneAction : CharacterAction
    {
        public CutscenePanel cutsceneRunner = null;

        protected override void Awake()
        {
            base.Awake();
            //cutsceneJsonLoadTest();
        }

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
