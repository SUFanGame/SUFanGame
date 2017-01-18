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

        Vector3 startPos_;

        public float shakeIntensity_ = 5f;
        public float shakeDropOffTime_ = 1f;
        

        public int repeats_ = 5;

        public CanvasGroup frontPanel_;

        public CanvasGroup terrainsCanvasGroup_;
        public float showTerrainAnimTime_ = .15f;

        public static System.Action OnAttackAnimsComplete_;

        CanvasGroup canvasGroup_;

        /// <summary>
        /// Flag for when a unit is currently performing an animation.
        /// </summary>
        bool animating_ = false;

        /// <summary>
        /// A buffered skill that has been triggered from an animation event.
        /// </summary>
        Combat.CombatEvent triggeredEvent_ = null;
        

        WaitForSeconds wait_ = new WaitForSeconds(.1f);

        /// <summary>
        /// This is for handling combat animations.
        /// We need to account for skills that occur before an attack does (PRE_ATTACK).
        /// The UI relies on the animations themselves to know when it needs to do stuff
        /// relating to the skill (Effects, show the label, etc.). So we buffer to the skill
        /// knowing that an event will be coming up in the future to trigger it.
        /// </summary>
        Dictionary<int, Combat.CombatEvent> eventBuffer_ = new Dictionary<int, Combat.CombatEvent>();

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

            for( int i = 0; i < unitPanels_.Length; ++i )
            {
                //unitPanels_[i].Initialize(testUnits_[i]);
            }

            //CombatPanel.DoAttacks();

            startPos_ = (transform as RectTransform).anchoredPosition;
            //ClearImmediate();

            //StartCoroutine(TweenTest());
            //StartCoroutine(ShowTerrainsTest());
        }

        public static void Toggle()
        {
            instance_.animator_.SetTrigger("Toggle");
        }

        public static void ShowTerrains()
        {
            var leftPos = instance_.unitPanels_[0].character_.transform.position;
            var rightPos = instance_.unitPanels_[1].character_.transform.position;
            instance_.ShowTerrainsInternal( leftPos, rightPos );
        }

        void OnAnimationEvent( string eventType, int side )
        {
            if( eventType == "AttackHit")
            {
                //Debug.Log("ATTACK HIT EVENT");
                ScreenShakeTween(side);
                ScreenFlashTween();
                //StartCoroutine(ScreenFlash());
            }
            if( eventType == "AnimComplete")
            {
                animating_ = false;
            }
            // TODO : Make this generic so it will work for all events.
            if( eventType == "PreAttack")
            {
                //Debug.Log("Preattack event");
                foreach( var pair in eventBuffer_)
                {
                    //Debug.LogFormat("Pair: {0}: {1}", (GameEvent)pair.Key, pair.Value.name);
                }
                Combat.CombatEvent bufferedEvent;
                if( eventBuffer_.TryGetValue((int)GameEvent.EVT_PRE_ATTACK, out bufferedEvent) )
                {
                    eventBuffer_.Remove((int)GameEvent.EVT_PRE_ATTACK);
                    // Set up our triggered skill so our process routine can catch it.
                    //Debug.Log("Set triggered skill to " + skill.name);
                    triggeredEvent_ = bufferedEvent;
                }
                else
                {
                    Debug.LogFormat("Couldn't find a skill in buffer at {0}", GameEvent.EVT_PRE_ATTACK);
                }
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

        IEnumerator TweenTest()
        {
            while( true )
            {
                ScreenShakeTween(1);
                yield return new WaitForSeconds(screenShakeTime_ + 1f);
            }

        }

        void ScreenShakeTween(int side)
        {
            var rt = transform as RectTransform;
            rt.anchoredPosition = startPos_;

            var tweenVec = new Vector3(shakeIntensity_ * -side, shakeIntensity_, 0);

            // Loop the shake routine a few times.
            var tween = LeanTween.move(transform as RectTransform, tweenVec, screenShakeTime_ / (float)repeats_).setEase(shakeEaseType_).setRepeat(repeats_);
            // Tween down the scale of the shake over the duration.
            LeanTween.value(gameObject, (v) => tween.setScale(v), 1f, 0f, screenShakeTime_).setEase(scaleEaseType_);
        }

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

        public static void ProcessCombatEvents( IList<Combat.CombatEvent> events )
        {
            //instance_.PrintCombatEvents(events);
            instance_.StartCoroutine(instance_.ProcessEvents(events));
        }

        IEnumerator ProcessEvents( IList<Combat.CombatEvent> events )
        {
            yield return new WaitForSeconds(.1f);

            Combat.CombatEvent e = null;
            foreach( var combatEvent in events )
            {
                e = combatEvent;
                var panel = unitPanels_[e.side_];
                var unit = panel.character_;

                // Watch for skill triggers while processing
                if (triggeredEvent_ != null )
                {
                    yield return DoSkillTrigger(triggeredEvent_.skill_, triggeredEvent_.side_);
                }

                switch ( e.type_ )
                {
                    case Combat.CombatEventType.HIT:
                    case Combat.CombatEventType.CRIT:
                        {
                            // If we're in the middle of an animation and we process another attack request
                            // we have to wait until the first animation completes
                            if( animating_ )
                            {
                                yield return WaitForAnimation();
                            } 
                            animating_ = true;
                            panel.PlayAnimation("Attack");
                        }
                        break;

                    case Combat.CombatEventType.SKILL:
                        {
                            Debug.LogFormat("Adding {0} to buffer, trigger: {1}", e.skill_.name, e.skill_.TriggerEvent_);
                            eventBuffer_[(int)e.skill_.TriggerEvent_] = e;
                        }
                        break;
                }
            }
            if (animating_)
                yield return WaitForAnimation();

            Debug.LogFormat("Combat panel is done processing events");
            yield return null;
        }

        IEnumerator WaitForAnimation()
        {
            while( true )
            {
                // Watch for skill triggers while waiting for animatinos to complete.
                if (triggeredEvent_ != null)
                {
                    yield return DoSkillTrigger(triggeredEvent_.skill_, triggeredEvent_.side_);
                }

                if (!animating_)
                    break;

                yield return wait_;
            }
        }

        IEnumerator DoSkillTrigger(CombatSkill skill, int side)
        {
            Debug.Log("Skill triggered!");
            triggeredEvent_ = null;

            // Run the skills coroutine "parallel" to the label
            if( skill.ShowLabel_ )
            {
                StartCoroutine(skill.UIRoutine(unitPanels_[side]));
                yield return unitPanels_[side].DoSkillLabel(skillLabelPrefab_, skill);
                
            }
            else
            {
                yield return skill.UIRoutine(unitPanels_[side]);
            }

            // Once it does, do our skill's thing
            //yield return skill.UIRoutine(unitPanels_[side]);
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

        public static void StopAnimations()
        {
            var p = instance_;
            for( int i = 0; i < 2; ++i )
            {
                // Check if character is alive.
                if (p.unitPanels_[i].character_ == null)
                    continue;

                //var a = p.unitPanels_[i].unitAnimator_;

                

                //// I've attempted several different ways to stop and reset the animations,
                //// this seems to be the only one that works. The one thing I didn't try is using Unity's new
                //// "Playables" API, that may be the way to go in the long run.

                //// Note you MUST use hashed ids for this, it silently fails otherwise.
                //var id = Animator.StringToHash("Attack");
                //a.Play(id, -1, 0);
                //a.speed = 0;

            }
        }

        void PrintCombatEvents( IList<Combat.CombatEvent> events )
        {
            foreach( var e in events )
            {
                Debug.Log(e.type_);
            }
        }
    }
}