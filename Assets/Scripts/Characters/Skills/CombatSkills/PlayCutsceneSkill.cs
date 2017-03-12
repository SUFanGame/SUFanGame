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
            //var runner = new GameObject("SupportRunner", typeof(SupportPanel)).GetComponent<SupportPanel>();
            SupportPanel.Dialog = SupportLoader.ImportSupport(supportAsset_.name);

            CombatPanel.Pause();
            //var canvas = Instantiate(canvasPrefab_);
            yield return SupportPanel.DoDialogWithCanvas();
            //DestroyImmediate(runner.gameObject);
            //DestroyImmediate(canvas.gameObject);
            CombatPanel.Unpause();

        }
    }
}
