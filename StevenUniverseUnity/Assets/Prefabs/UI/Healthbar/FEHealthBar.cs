using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


namespace SUGame.StrategyMap.UI.CombatPanelUI
{
    /// <summary>
    /// Animated health bar matching the one from Sacred Stones.
    /// </summary>
    [ExecuteInEditMode]
    public class FEHealthBar : MonoBehaviour
    {
        public float animTime_ = .35f;

        public int MaxHealth_
        {
            get
            {
                return maxHealth_;
            }
            set
            {
                value = Mathf.Clamp(value, 0, int.MaxValue);
                maxHealth_ = value;

                // One "health unit" is equivalent to 2 pixels, +1 to account for the "border" pixels
                float size = maxHealth_ == 0 ? 0 : 1 + maxHealth_ * 2;

                borderTransform_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
                frameLayout_.minWidth = size;
                //LayoutRebuilder.MarkLayoutForRebuild(positiveHealthTransform_);
            }
        }

        [SerializeField]
        int maxHealth_;

        public int CurrentHealth_
        {
            get
            {
                return currentHealth_;
            }
            set
            {
                value = Mathf.Clamp(value, 0, int.MaxValue);
                currentHealth_ = value;

                // One "health unit" is equivalent to 2 pixels
                float size = currentHealth_ * 2;
                //healthPositiveLayout_.minWidth = size;
                positiveHealthTransform_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
                healthValueText_.text = value.ToString();

                if (value == 0 && HealthZeroed_ != null)
                    HealthZeroed_.Invoke();

            }
        }

        [SerializeField]
        int currentHealth_;

        [SerializeField]
        RectTransform borderTransform_;
        [SerializeField]
        RectTransform positiveHealthTransform_;
        [SerializeField]
        LayoutElement frameLayout_;

        [SerializeField]
        Text healthValueText_;

        // Callback used for tweening.
        System.Action<float> SetCurrentHealthCallback_;

        /// <summary>
        /// Callback for when health reaches zero.
        /// </summary>
        public System.Action HealthZeroed_;

        void Start()
        {

        }

        void Awake()
        {
            if (borderTransform_ == null)
            {
                Debug.Log("Health Bar must be on a UI Element with a Rect Transform");
                enabled = false;
                return;
            }


            MaxHealth_ = maxHealth_;
            SetCurrentHealthCallback_ = (f) =>
            {
                CurrentHealth_ = (int)f;
            };
        }

        void OnValidate()
        {
            if (!isActiveAndEnabled)
                return;

            maxHealth_ = Mathf.Clamp(maxHealth_, 0, int.MaxValue);

            MaxHealth_ = maxHealth_;

            currentHealth_ = Mathf.Clamp(currentHealth_, 0, maxHealth_);

            CurrentHealth_ = currentHealth_;
        }

        public void TweenCurrentHealth(int val)
        {
            LeanTween.value(gameObject, SetCurrentHealthCallback_, CurrentHealth_, val, animTime_).setEase(LeanTweenType.linear);

            StartCoroutine(ScaleHealthText());
        }

        IEnumerator ScaleHealthText()
        {
            var scale = healthValueText_.transform.localScale;
            scale.y = 2f;
            healthValueText_.transform.localScale = scale;

            yield return new WaitForSeconds(animTime_);

            healthValueText_.transform.localScale = Vector3.one;

            yield return null;
        }

    }
}