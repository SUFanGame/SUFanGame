using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace SUGame.StrategyMap.UI.CombatPanelUI
{
    /// <summary>
    /// Behaviour for combat panels containing unit stats/image.
    /// Each panel is squared off against the other for combat animations.
    /// </summary>
    public class CombatPanelUnit : MonoBehaviour
    {
        // TODO : NOT LIKE THIS? In theory I should be able to drop this component onto
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
        public Animator unitAnimator_;

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

        // For death "animation"
        public LeanTweenType fadeTweenType_;
        public float fadeTweenTime_ = 1f;
        public float fadeTweenDelay_ = .1f;


        void Awake()
        {
            animCallback_.onAnimationEvent_ += OnAnimationEvent;
        }

        public void Initialize( MapCharacter character )
        {
            healthBar_.MaxHealth_ = character.Data.Stats[Stat.Type.HP].MaxLevelValue_;
            healthBar_.CurrentHealth_ = character.Data.Stats[Stat.Type.HP].Current_;
            //hitValueText_.text = character.Data.Stats.accuracy.ToString();
            //critValueText_.text = character.Data.Stats.crit.ToString();
            //dmgValueText_.text = character.Data.Stats.str.ToString();
            //weaponNameText_.text = character.weaponName_;
            //unitImage_.sprite = character.combatSprite_;
            LeanTween.alphaCanvas(unitImage_.GetComponent<CanvasGroup>(), 1, 0.001f);
            character_ = character;


            healthBar_.HealthZeroed_ -= OnHealthZero;
            healthBar_.HealthZeroed_ += OnHealthZero;

            //if ( character == null )
            //{
            //    Debug.Log("Character was null in " + name + " init");
            //}
            //Debug.LogFormat("Setting character for {0} to {1}", name, character_.name);
            unitNameText_.text = character.name;
        }

        void OnAnimationEvent( string eventName, int side )
        {
            // TODO: Externalize these "animation event names" in some way, maybe through an editor?
            //  having to go into the animation itself to see it is tedious and dumb
            if( eventName == "AttackHit")
            {
                otherPanelHealthBar_.TweenCurrentHealth(otherPanelHealthBar_.CurrentHealth_ - character_.Data.Stats[Stat.Type.STR]);
            }
        }

        public void ShowAnimation( Vector3 worldPos, float animTime )
        {
            //var rt = transform as RectTransform;
            var canvasPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPos);
            terrain_.localScale = Vector3.zero;
            

            //float x = terrain_.sizeDelta.x * (terrain_.pivot.x - .5f);

            //canvasPoint.x += x;

            //terrain_.position = canvasPoint;
            //rt.anc = canvasPoint;
            //Debug.Log(canvasPoint);
            //Util.TransformUtil.RectTransformUtil.MoveCentered(terrain_, canvasPoint);

            // terrain_.position = canvasPoint;
            //terrain_.anchoredPosition = terrain_.InverseTransformPoint(canvasPoint);
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

        //public Animator[] characterAnimators_;

        //static CombatPanel instance_;

        //MapCharacter attacker_;
        //MapCharacter defender_;

        //[SerializeField]
        //Image attackerImage_;
        //[SerializeField]
        //Image defenderImage_;

        //Animator animator_;



        //void Awake()
        //{
        //    characterAnimators_ = new Animator[]
        //    {
        //        attackerImage_.GetComponent<Animator>(),
        //        defenderImage_.GetComponent<Animator>(),
        //    };

        //    StartCoroutine(PlayAnims(characterAnimators_));
        //    instance_ = this;
        //    animator_ = GetComponent<Animator>();
        //}

        //IEnumerator PlayAnims(Animator[] animators)
        //{


        //    for (int i = 0; i < animators.Length; ++i)
        //    {
        //        var a = animators[i];
        //        a.SetTrigger("Attack");

        //        yield return new WaitUntil(() => (a.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !a.IsInTransition(0)));
        //    }



        //    yield return null;
        //}

        //public static void Toggle()
        //{
        //    //animator
        //}

        //public static void Initialize( MapCharacter attacker, MapCharacter defender )
        //{
        //    instance_.attackerImage_.sprite = attacker.combatSprite_;
        //    instance_.defenderImage_.sprite = defender.combatSprite_;
        //}
    }
}