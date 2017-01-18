using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using SUGame.Characters.Skills;

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
        
        public MapCharacter character_;

        IEnumerable<string> cachedClipNames_;

        [SerializeField]
        UICombatSkillPanel skillLabelPanel_;
        

        // For death "animation"
        public LeanTweenType fadeTweenType_;
        public float fadeTweenTime_ = 1f;
        public float fadeTweenDelay_ = .1f;


        void Awake()
        {
            animCallback_.onAnimationEvent_ += OnAnimationEvent;
        }

        public void Initialize( MapCharacter attacker, MapCharacter defender )
        {
            RefreshValues(attacker, defender);
            character_ = attacker;
            var sprite = character_.CombatSprite_;
            var controller = character_.CombatAnimController_;
            if( sprite == null )
            {
                Debug.LogErrorFormat(character_.gameObject, "Error initializing combat UI, {0} has no combat sprite", character_.name);
            }
            if( controller == null )
            {
                Debug.LogErrorFormat(character_.gameObject, "Error initializing combat UI, {0} has no combat animator controller", character_.name);
            }

            unitAnimator_.runtimeAnimatorController = controller;
            unitImage_.sprite = sprite;

            cachedClipNames_ = controller.animationClips.Select(c => c.name);

            healthBar_.HealthZeroed_ -= OnHealthZero;
            healthBar_.HealthZeroed_ += OnHealthZero;

            //if ( character == null )
            //{
            //    Debug.Log("Character was null in " + name + " init");
            //}
            //Debug.LogFormat("Setting character for {0} to {1}", name, character_.name);
            unitNameText_.text = attacker.name;
        }

        void RefreshValues(MapCharacter attacker, MapCharacter defender )
        {

            healthBar_.MaxHealth_ = attacker.Data.Stats_[Stats.PrimaryStat.HP].MaxValue_;
            healthBar_.CurrentHealth_ = attacker.Data.Stats_[Stats.PrimaryStat.HP].CurrentValue_;
            hitValueText_.text = Combat.GetHitChance(attacker, defender, attacker.Data.EquippedWeapon_).ToString();
            critValueText_.text = Combat.GetCritChance(attacker, defender, attacker.Data.EquippedWeapon_).ToString();
            dmgValueText_.text = Combat.GetDamage(attacker, defender, attacker.Data.EquippedWeapon_).ToString();
            //LeanTween.value(null, (f) => healthBar_.CurrentHealth_ = Mathf.RoundToInt(f), 20, 5, 1f);
            //LeanTween.value(20, 5, 1f).setOnUpdate(f => healthBar_.CurrentHealth_ = Mathf.RoundToInt(f));
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
            int newValue = Combat.GetDamage(character_, otherPanel_.character_, character_.Data.EquippedWeapon_);
            Debug.Log("New damage:" + newValue);
            LeanTween.value(gameObject,
                (f) => dmgValueText_.text = Mathf.RoundToInt(f).ToString(),
                oldValue,
                newValue,
                time
                );

        }
        

        void OnAnimationEvent( string eventName, int side )
        {
            // TODO: Externalize these "animation event names" in some way, maybe through an editor?
            //  having to go into the animation itself to see it is tedious and dumb
            if( eventName == "AttackHit")
            {
                otherPanelHealthBar_.TweenCurrentHealth(otherPanelHealthBar_.CurrentHealth_ - character_.Data.Stats_[Stats.PrimaryStat.STR]);
            }
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
            if( character_ == null )
            {
                Debug.LogFormat("Attempting to play death anim on null char in {0}", name);
            }

            var tween = LeanTween.alphaCanvas(unitImage_.GetComponent<CanvasGroup>(), 0, fadeTweenTime_).setEase(fadeTweenType_).setDelay(fadeTweenDelay_);
            tween.setOnComplete(character_.Kill);
            character_ = null;
        }

        public void PlayAnimation(string name)
        {
            terrain_.SetAsLastSibling();
            if( !cachedClipNames_.Contains(name) )
            {
                Debug.LogErrorFormat(character_.gameObject, "Error playing animation {0}, {1}s combat controller doesn't have that animation clip", name, character_.name);
            }

            // Note you MUST use hashed ids for this, it silently fails otherwise.
            var id = Animator.StringToHash(name);

            //Debug.Log("Playing animation " + name + " on " + unitImage_.name);
            unitAnimator_.Play(id, -1, 0);
            unitAnimator_.speed = 1;
            animCallback_.SignalWhenComplete(name);
        }

        float labelTime_ = 1.5f;
        public IEnumerator DoSkillLabel(UICombatSkillLabel prefab, CombatSkill skill)
        {
            unitAnimator_.enabled = false;
            skillLabelPanel_.ShowLabel(prefab, skill);
            yield return new WaitForSeconds(labelTime_);
            skillLabelPanel_.RemoveLabel();
            unitAnimator_.enabled = true;
        }

    }
}