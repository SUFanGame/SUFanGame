using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using SUGame.Characters.Skills;
using System;
using SUGame.Characters;

namespace SUGame.StrategyMap.UI.CombatPanelUI
{
    /// <summary>
    /// Behaviour for combat panels containing unit stats/image.
    /// Each panel is squared off against the other for combat animations.
    /// </summary>
    public class CombatPanelUnit : MonoBehaviour
    {
        // TODO : NOT LIKE THIS? This is a lot of components to have to manually assign.
        // In theory I should be able to drop this component onto
        // a gameobject and it should be able to assign all it's own stuff. Maybe by name?
        // Not sure what would be better, but having to manually re-set fields is a bummer.
        // Though doing it this way does prevent things breaking from hierarchy and name changes...
        // I'm sure a lot of this could be handled with GetComponentInChildren...

        [SerializeField]
        Text dmgValueText_;
        [SerializeField]
        Text hitValueText_;
        [SerializeField]
        Text critValueText_;
        [SerializeField]
        Text weaponNameText_;
        [SerializeField]
        Image unitImage_;
        [SerializeField]
        Image portraitImage_;

        UIStats lastStats_;

        [SerializeField]
        public FEHealthBar healthBar_;
        [SerializeField]
        Animator unitAnimator_;

        [SerializeField]
        Text unitNameText_;

        [SerializeField]
        AnimationCallback animCallback_;

        [SerializeField]
        CombatPanelUnit otherPanel_;
        [SerializeField]
        FEHealthBar otherPanelHealthBar_;

        [SerializeField]
        public RectTransform terrain_;
        
        public MapCharacter attacker_;
        MapCharacter defender_;

        IEnumerable<string> cachedClipNames_;

        [SerializeField]
        UICombatSkillPanel skillLabelPanel_;
        

        // For death "animation"
        public LeanTweenType fadeTweenType_;
        public float fadeTweenTime_ = 1f;
        public float fadeTweenDelay_ = .1f;

        bool tweening_ = false;


        void Awake()
        {
        }

        public void Initialize( MapCharacter attacker, MapCharacter defender )
        {
            lastStats_ = new UIStats(attacker, defender);
            RefreshValues(lastStats_);
            attacker_ = attacker;
            defender_ = defender;
            var sprite = attacker_.CombatSprite_;
            var controller = attacker_.CombatAnimController_;
            if( sprite == null )
            {
                Debug.LogErrorFormat(attacker_.gameObject, "Error initializing combat UI, {0} has no combat sprite", attacker_.name);
            }
            if( controller == null )
            {
                Debug.LogErrorFormat(attacker_.gameObject, "Error initializing combat UI, {0} has no combat animator controller", attacker_.name);
            }

            unitAnimator_.runtimeAnimatorController = controller;
            unitImage_.sprite = sprite;

            cachedClipNames_ = controller.animationClips.Select(c => c.name);

            healthBar_.HealthZeroed_ -= OnHealthZero;
            //healthBar_.HealthZeroed_ += OnHealthZero;

            //if ( character == null )
            //{
            //    Debug.Log("Character was null in " + name + " init");
            //}
            //Debug.LogFormat("Setting character for {0} to {1}", name, character_.name);
            unitNameText_.text = attacker.name;
        }

        void RefreshValues( UIStats stats )
        {
            healthBar_.MaxHealth_ = stats.hpMax_;
            healthBar_.CurrentHealth_ = stats.hpCurr_;
            hitValueText_.text = stats.hit_.ToString();
            critValueText_.text = stats.crit_.ToString();
            dmgValueText_.text = stats.damage_.ToString();


            //LeanTween.value(null, (f) => healthBar_.CurrentHealth_ = Mathf.RoundToInt(f), 20, 5, 1f);
            //LeanTween.value(20, 5, 1f).setOnUpdate(f => healthBar_.CurrentHealth_ = Mathf.RoundToInt(f));
        }

        void TweenTo(UIStats stats, float time)
        {
            LeanTween.value(lastStats_.damage_, stats.damage_, time).setOnUpdate(
                (f) => SetTextValue(dmgValueText_, f));
            LeanTween.value(lastStats_.crit_, stats.crit_, time).setOnUpdate(
                (f) => SetTextValue(critValueText_, f));
            LeanTween.value(lastStats_.hit_, stats.hit_, time).setOnUpdate(
                (f) => SetTextValue(hitValueText_, f));
            LeanTween.value(lastStats_.hpCurr_, stats.hpCurr_, time).setOnUpdate(
                (f) => healthBar_.CurrentHealth_ = Mathf.RoundToInt(f)).setOnComplete(()=>tweening_ = false);
        }
        
        void SetTextValue(Text txt, float f)
        {
            txt.text = Mathf.RoundToInt(f).ToString();
        }

        void Update()
        {
            if (attacker_ == null || defender_ == null )
                return;

            var stats = new UIStats(attacker_, defender_);
            if( !tweening_ && !stats.Equals(lastStats_))
            {
                tweening_ = true;
                TweenTo(stats, .35f);
                lastStats_ = stats;
            }
            
        }

        void HandleStatChange( Stats.PrimaryStat stat )
        {
        }

        public void TweenDamage(int newVal, float animTime )
        {
            var oldValue = int.Parse(dmgValueText_.text);
            LeanTween.value( gameObject,
                (f)=>dmgValueText_.text = Mathf.RoundToInt(f).ToString(),
                oldValue,
                newVal,
                animTime
                );
        }

        public void RefreshStatsTween(float time)
        {
            var oldValue = int.Parse(dmgValueText_.text);
            int newValue = Combat.GetDamage(attacker_, otherPanel_.attacker_, attacker_.Data.EquippedWeapon_);
            Debug.Log("New damage:" + newValue);
            LeanTween.value(gameObject,
                (f) => dmgValueText_.text = Mathf.RoundToInt(f).ToString(),
                oldValue,
                newValue,
                time
                );

        }

        /// <summary>
        /// Does a Sacred Stones-esque animation to show the two units facing off on their respective
        /// terrain tiles.
        /// </summary>
        public void DoTerrainAnim( Vector3 worldPos, float animTime )
        {
            var canvasPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPos);
            terrain_.localScale = Vector3.zero;
            
            terrain_.position = canvasPoint;

            LeanTween.scale(terrain_.gameObject, Vector3.one, animTime).setEaseLinear();
            LeanTween.move(terrain_.transform as RectTransform, Vector3.zero, animTime).setEaseLinear();
        }

        public Vector3 MoveCenter( RectTransform t, Vector3 pos )
        {
            return Vector3.zero;
        }

        void OnHealthZero()
        {
            healthBar_.HealthZeroed_ -= OnHealthZero;

            //Debug.Log("OnHealth zero " + name);
            if( attacker_ == null )
            {
                Debug.LogFormat("Attempting to play death anim on null char in {0}", name);
            }

            var tween = LeanTween.alphaCanvas(unitImage_.GetComponent<CanvasGroup>(), 0, fadeTweenTime_).setEase(fadeTweenType_).setDelay(fadeTweenDelay_);
            tween.setOnComplete(attacker_.Kill);
            //attacker_ = null;
        }

        public void PlayAnimation(string name)
        {
            if( name == "Killed" )
            {
                var tween = LeanTween.alphaCanvas(unitImage_.GetComponent<CanvasGroup>(), 0, fadeTweenTime_).setEase(fadeTweenType_).setDelay(fadeTweenDelay_);
                tween.setOnComplete(attacker_.Kill);
                return;
            }

            terrain_.SetAsLastSibling();
            if( !cachedClipNames_.Contains(name) )
            {
                Debug.LogErrorFormat(attacker_.gameObject, "Error playing animation {0}, {1}s combat controller doesn't have that animation clip", name, attacker_.name);
            }

            // Note you MUST use hashed ids for this, it silently fails otherwise.
            var id = Animator.StringToHash(name);
            //Debug.Log("Playing animation " + name + " on " + unitImage_.name);
            //Debug.LogFormat("Anim {0} should be playing on {1}", name, attacker_);
            unitAnimator_.Play(id, -1, 0);
            unitAnimator_.speed = 1;
            animCallback_.SignalWhenComplete(name);
        }

        public void Pause()
        {
            unitAnimator_.enabled = false;
        }

        public void Unpause()
        {
            unitAnimator_.enabled = true;
        }

        float labelTime_ = 1.5f;
        public IEnumerator DoSkillLabel(UICombatSkillLabel prefab, CombatSkill skill)
        {
            Pause();
            skillLabelPanel_.ShowLabel(prefab, skill, .35f);
            yield return new WaitForSeconds(labelTime_);
            Unpause();
            skillLabelPanel_.FadeAndRemoveLabel(1f);
        }

        struct UIStats
        {
            public int hpCurr_;
            public int hpMax_;
            public int hit_;
            public int crit_;
            public int damage_;
            public UIStats( MapCharacter attacker, MapCharacter defender )
            {
                var stats = attacker.Data.Stats_;
                hpCurr_ = stats[Stats.PrimaryStat.HP].CurrentValue_;
                hpMax_ = stats[Stats.PrimaryStat.HP].MaxValue_;
                hit_ = Combat.GetHitChance(attacker, defender, attacker.Data.EquippedWeapon_);
                crit_ = Combat.GetCritChance(attacker, defender, attacker.Data.EquippedWeapon_);
                damage_ = Combat.GetDamage(attacker, defender, attacker.Data.EquippedWeapon_);
            }
            
            public bool Equals( UIStats other )
            {
                return hpCurr_ == other.hpCurr_ && hpMax_ == other.hpMax_ && 
                       hit_ == other.hit_ && crit_ == other.crit_ && damage_ == other.damage_;
            }
        }
    }
}