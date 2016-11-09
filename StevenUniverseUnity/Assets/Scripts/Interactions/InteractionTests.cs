using UnityEngine;
using System.IO;
using StevenUniverse.FanGame.Util;
using StevenUniverse.FanGame.StrategyMap;
using System.Collections;

namespace StevenUniverse.FanGame.Interactions
{
    //DEBUGGING CLASS, hooks into Marth's "Start Drama" action
    class InteractionTests : CharacterAction
    {

        public IEnumerator Execute( string cutsceneName )
        {
            executeTest1(cutsceneName);
            yield return null;
        }

        public void executeTest1( string cutsceneName )
        {
            string absolutePath = Utilities.ConvertAssetPathToAbsolutePath("Assets/Resources/Cutscenes/" + cutsceneName + ".json");
            if (!File.Exists(absolutePath))
            {
                throw new UnityException("Cutscene " + cutsceneName + " was not found.");
            }

            Scene[] parsedScenes = JsonHelper.FromJson<Scene>(File.ReadAllText(absolutePath));

            foreach (Scene sce in parsedScenes)
            {
                Debug.Log(sce.ToString());
                
            }

        }
    }
}
