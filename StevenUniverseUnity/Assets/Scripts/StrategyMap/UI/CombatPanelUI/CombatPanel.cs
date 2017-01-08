using UnityEngine;
using System.Collections;

namespace SUGame.StrategyMap.UI.CombatPanelUI
{
    public class CombatPanel : MonoBehaviour
    {
        static CombatPanel instance_;

        Animator animator_;

        [SerializeField]
        CombatPanelUnit[] unitPanels_;

        public MapCharacter[] testUnits_;

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
            ClearImmediate();

            //StartCoroutine(TweenTest());
            //StartCoroutine(ShowTerrainsTest());
        }

        public static void Toggle()
        {
            instance_.animator_.SetTrigger("Toggle");
        }

        public static void DoAttacks()
        {
            instance_.StartCoroutine(instance_.DoAttacksRoutine());
        }


        IEnumerator DoAttacksAfterTerrainAnim()
        {
            yield return new WaitForSeconds(showTerrainAnimTime_);

            yield return DoAttacksRoutine();
        }

        IEnumerator DoAttacksRoutine()
        {
            for( int i = 0; i < 2; ++i )
            {
                if (unitPanels_[i].character_ == null)
                    continue;

                unitPanels_[i].terrain_.SetAsLastSibling();
                
                var a = unitPanels_[i].unitAnimator_;

                var id = Animator.StringToHash("Attack");

                a.Play(id, -1, 0 );
                a.speed = 1;
                //a.SetTrigger("Attack");


                yield return null;
                //yield return new WaitWhile(() => a.isPlaying);
                yield return new WaitUntil(() => (a.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !a.IsInTransition(0)));
            }

            if (OnAttackAnimsComplete_ != null)
                OnAttackAnimsComplete_.Invoke();

            //unitPanels_[0].transform.SetAsLastSibling();

        }

        IEnumerator ShowTerrainsTest()
        {

            yield return new WaitForSeconds(.5f);
            BeginAttackAnimations(testUnits_[0].transform.position, testUnits_[1].transform.position);

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
                Debug.Log("Defender was null in COmbat Panel init");
            }
            
            instance_.unitPanels_[0].Initialize(attacker);
            instance_.unitPanels_[1].Initialize(defender);
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
            unitPanels_[0].ShowAnimation(attackerPosition + Vector3.up * .5f + Vector3.right * .5f, showTerrainAnimTime_ );
            unitPanels_[1].ShowAnimation(defenderPosition + Vector3.right * .5f + Vector3.up * .5f, showTerrainAnimTime_ );
            StartCoroutine(DoAttacksAfterTerrainAnim());
        }

        public static void StopAnimations()
        {
            var p = instance_;
            for( int i = 0; i < 2; ++i )
            {
                // Check if character is alive.
                if (p.unitPanels_[i].character_ == null)
                    continue;

                var a = p.unitPanels_[i].unitAnimator_;

                

                // I've attempted several different ways to stop and reset the animations,
                // this seems to be the only one that works. The one thing I didn't try is using Unity's new
                // "Playables" API, that may be the way to go in the long run.

                // Note you MUST use hashed ids for this, it silently fails otherwise.
                var id = Animator.StringToHash("Attack");
                a.Play(id, -1, 0);
                a.speed = 0;

            }
        }

    }
}