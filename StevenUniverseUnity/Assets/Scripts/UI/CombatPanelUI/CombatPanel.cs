using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.Characters.Skills;

namespace SUGame.StrategyMap.UI.CombatPanelUI
{
    public class CombatPanel : MonoBehaviour
    {
        static CombatPanel instance_;

        Animator animator_;

        [SerializeField]
        CombatPanelUnit[] unitPanels_;

        /// <summary>
        /// The two panels used for showing skill labels as skills occur in combat.
        /// </summary>
        [SerializeField]
        RectTransform[] combatSkillPanel_;

        [SerializeField]
        UICombatSkillLabel skillLabelPrefab_;

        //public MapCharacter[] testUnits_;

        public float screenShakeTime_;
        public float screenFlashTime_;

        public LeanTweenType shakeEaseType_;
        public LeanTweenType scaleEaseType_;
        public LeanTweenType flashEaseType_;
        
        public float shakeIntensity_ = 5f;
        public float shakeDropOffTime_ = 1f;
        

        public int repeats_ = 5;

        public CanvasGroup frontPanel_;

        public CanvasGroup terrainsCanvasGroup_;
        public float showTerrainAnimTime_ = .15f;

        public static System.Action OnAttackAnimsComplete_;

        CanvasGroup canvasGroup_;


        /// <summary>
        /// Last animation event that was triggered.
        /// </summary>
        CombatAnimEvent triggeredEvent_ = CombatAnimEvent.NONE;

        void Awake()
        {
            instance_ = this;
            animator_ = GetComponent<Animator>();
            canvasGroup_ = GetComponent<CanvasGroup>();

            var callbacks = GetComponentsInChildren<AnimationCallback>();

            foreach( var cb in callbacks )
            {
                cb.onAnimationEvent_ += OnAnimationEvent;
            }
        }

        public static void Toggle()
        {
            instance_.animator_.SetTrigger("Toggle");
        }

        /// <summary>
        /// Do the "Show Combat Units" animation where the Combat sprites standing on the terrain tween in
        /// from the map. The combat panel should be initialized before
        /// this is called.
        /// </summary>
        public static void ShowTerrains()
        {
            var leftPos = instance_.unitPanels_[0].attacker_.transform.position;
            var rightPos = instance_.unitPanels_[1].attacker_.transform.position;
            instance_.ShowTerrainsInternal( leftPos, rightPos );
        }

        /// <summary>
        /// Play the given animation for the character on the given side.
        /// </summary>
        /// <param name="side">0 for left, 1 for right.</param>
        public static void PlayAnim(string anim, int side )
        {
            Debug.Log("Play anim " + anim + " Side: " + side );
            instance_.unitPanels_[side].PlayAnimation(anim);
        }

        /// <summary>
        /// Coroutine that returns control once the given animation event is received
        /// from the current AnimationCallbacks
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        public static IEnumerator WaitForAnimEvent( CombatAnimEvent evt )
        {
            while( true )
            {
                if( instance_.triggeredEvent_ == evt )
                {
                    instance_.triggeredEvent_ = CombatAnimEvent.NONE;
                    yield break;
                }
                yield return null;
            }
        }

        /// <summary>
        /// Handle incoming events from the AnimationCallback objects on the combat sprites.
        /// </summary>
        void OnAnimationEvent( CombatAnimEvent evt, int side )
        {
            switch( evt )
            {
                case CombatAnimEvent.ATTACK_HIT:
                    triggeredEvent_ = CombatAnimEvent.ATTACK_HIT;
                    //Debug.Log("ATTACK HIT EVENT");
                    ScreenShakeTween(side);
                    ScreenFlashTween();
                    //StartCoroutine(ScreenFlash());
                    break;

                case CombatAnimEvent.ANIM_COMPLETE:
                    Debug.Log("ANIMCOMPLETE");
                    triggeredEvent_ = CombatAnimEvent.ANIM_COMPLETE;
                    break;

                case CombatAnimEvent.PRE_ATTACK:
                    triggeredEvent_ = CombatAnimEvent.PRE_ATTACK;
                    break;
            }
        }
        
        IEnumerator ScreenFlash()
        {
            frontPanel_.alpha = 1;
            yield return new WaitForSeconds(.1f);
            frontPanel_.alpha = 0;
        }


        void ScreenFlashTween()
        {
            LeanTween.alphaCanvas(frontPanel_, 1f, screenFlashTime_).setEase(flashEaseType_).setOnComplete(() =>
            LeanTween.alphaCanvas(frontPanel_, 0f, screenFlashTime_).setEase(flashEaseType_)
                );
        }
        
        // TODO: Allow for the "intensity" to be passed in for big/small hits to give corresponding shakes.
        /// <summary>
        /// Do the screen shake animation for when a unit gets hit.
        /// </summary>
        void ScreenShakeTween(int side)
        {
            var tweenVec = new Vector3(shakeIntensity_ * -side, shakeIntensity_, 0);

            // Loop the shake routine a few times.
            var tween = LeanTween.move(transform as RectTransform, tweenVec, screenShakeTime_ / (float)repeats_).setEase(shakeEaseType_).setRepeat(repeats_);
            // Tween down the scale of the shake over the duration.
            LeanTween.value(gameObject, (v) => tween.setScale(v), 1f, 0f, screenShakeTime_).setEase(scaleEaseType_);
        }

        /// <summary>
        /// Initialize the combat panel with the given units. This will set up the given units' combat sprites.
        /// </summary>
        public static void Initialize( MapCharacter attacker, MapCharacter defender )
        {
            if( attacker == null )
            {
                Debug.Log("Attacker was null in CombatPanel init");
            }

            if( defender == null )
            {
                Debug.Log("Defender was null in Combat Panel init");
            }
            
            instance_.unitPanels_[0].Initialize(attacker, defender);
            instance_.unitPanels_[1].Initialize(defender, attacker);
        }
        
        /// <summary>
        /// Allows executing skills to do UI effects and optionall shows a label of the currently
        /// executing skill <seealso cref="CombatSkill.showLabel_"/>.
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        public static IEnumerator HandleSkillTrigger(CombatSkill skill, int side)
        {
            //Debug.Log("Skill triggered!");

            // Run the skills coroutine "parallel" to the label
            if( skill.ShowLabel_ )
            {
                instance_.StartCoroutine(skill.UIRoutine(instance_.unitPanels_[side]));
                yield return instance_.unitPanels_[side].DoSkillLabel(instance_.skillLabelPrefab_, skill);
                
            }
            else
            {
                yield return skill.UIRoutine(instance_.unitPanels_[side]);
            }
        }
        

        public static void ShowPanel()
        {
            instance_.canvasGroup_.alpha = 1;
            instance_.terrainsCanvasGroup_.alpha = 0;
            instance_.animator_.SetTrigger("ShowPanel");
            ShowPortraits();
        }

        public static void ShowPortraits()
        {
            instance_.animator_.SetTrigger("ShowPortraits");
        }

        public static void HidePortraits()
        {
            instance_.animator_.SetTrigger("HidePortraits");
        }

        public static void HidePanel()
        {

            instance_.animator_.SetTrigger("HidePanel");
        }

        public static void Clear()
        {
            HidePanel();
            HidePortraits();
            HideTerrains();
            instance_.StartCoroutine(instance_.ResetStupidFuckingTriggers());
        }

        public static void ClearImmediate()
        {
            //instance_.canvasGroup_.alpha = 0;
            Clear();
        }

        /// <summary>
        /// Triggers can get "stuck" on if an animation is cancelled in the middle. Even more fun:
        /// ResetTrigger will do nothing if called during the same frame that a trigger gets stuck on.
        /// So we yield and reset.
        /// </summary>
        // http://answers.unity3d.com/answers/977718/view.html
        IEnumerator ResetStupidFuckingTriggers()
        {
            yield return null;

            instance_.animator_.ResetTrigger("ShowPortraits");
            instance_.animator_.ResetTrigger("ShowPanel");
            instance_.animator_.ResetTrigger("HidePortraits");
            instance_.animator_.ResetTrigger("HidePanel");
        }

        public static void BeginAttackAnimations(Vector3 attackerPosition, Vector3 defenderPosition)
        {
            instance_.ShowTerrainsInternal(attackerPosition, defenderPosition);
        }

        /// <summary>
        /// Hide the UI terrains via a fade animation.
        /// </summary>
        public static void HideTerrains()
        {
            LeanTween.alphaCanvas(instance_.terrainsCanvasGroup_, 0, instance_.showTerrainAnimTime_);
        }

        /// <summary>
        /// Scale the UI Terrains into the panel starting from the unit positions.
        /// </summary>
        void ShowTerrainsInternal(Vector3 attackerPosition, Vector3 defenderPosition)
        {
            terrainsCanvasGroup_.alpha = 1f;
            unitPanels_[0].DoTerrainAnim(attackerPosition + Vector3.up * .5f + Vector3.right * .5f, showTerrainAnimTime_ );
            unitPanels_[1].DoTerrainAnim(defenderPosition + Vector3.right * .5f + Vector3.up * .5f, showTerrainAnimTime_ );
        }

        /// <summary>
        /// Pause all combat animations.
        /// </summary>
        public static void Pause()
        {
            for( int i = 0; i < 2; ++i )
            {
                instance_.unitPanels_[i].Pause();
            }
        }

        /// <summary>
        /// Unpause all combat animations.
        /// </summary>
        public static void Unpause()
        {
            for (int i = 0; i < 2; ++i)
            {
                instance_.unitPanels_[i].Unpause();
            }
        }
    }
}