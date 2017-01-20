using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.Characters.Skills;
using SUGame.StrategyMap.UI.CombatPanelUI;
using SUGame.Interactions;

namespace SUGame.Characters.Skills
{


    [CreateAssetMenu(fileName = "CutsceneSkill", menuName = "Skills/Combat/Cutscene", order = 9001)]
    public class PlayCutsceneSkill : CombatSkill
    {

        [SerializeField]
        SupportCanvas canvasPrefab_;

        [SerializeField]
        TextAsset supportAsset_;

        public override IEnumerator UIRoutine(CombatPanelUnit uiPanel)
        {
            var runner = new GameObject("SupportRunner", typeof(SupportRunner)).GetComponent<SupportRunner>();
            var support = SupportLoader.ImportSupport(supportAsset_.name);
            runner.Dialog = support;

            CombatPanel.Pause();
            var canvas = Instantiate(canvasPrefab_);
            yield return runner.DoDialogueWithCanvas(canvas);
            DestroyImmediate(runner.gameObject);
            DestroyImmediate(canvas.gameObject);
            CombatPanel.Unpause();

        }
    }
}
