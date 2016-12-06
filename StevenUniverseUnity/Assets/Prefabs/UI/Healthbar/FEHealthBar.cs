using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[ExecuteInEditMode]
public class FEHealthBar : MonoBehaviour 
{
    public int MaxHealth_
    {
        get
        {
            return maxHealth_;
        }
        set
        {
            maxHealth_ = value;

            // One "health unit" is equivalent to 2 pixels, +1 to account for the "border" pixels
            float size = maxHealth_ == 0 ? 0 : 1 + maxHealth_ * 2;

            layout_.minWidth = size;
            rt_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size );
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
            currentHealth_ = value;

            // One "health unit" is equivalent to 2 pixels
            positiveHealthRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentHealth_ * 2 );
        }
    }

    [SerializeField]
    int currentHealth_;

    RectTransform rt_;
    [SerializeField]
    RectTransform positiveHealthRT_;
    LayoutElement layout_;


    void Awake()
    {
        rt_ = transform as RectTransform;
        layout_ = GetComponent<LayoutElement>();
        if (rt_ == null)
        {
            Debug.Log("Health Bar must be on a UI Element with a Rect Transform");
            enabled = false;
            return;
        }

       
        MaxHealth_ = maxHealth_;
    }

    void OnValidate()
    {
        if (!isActiveAndEnabled)
            return;

        maxHealth_ = Mathf.Clamp( maxHealth_, 0, int.MaxValue);

        MaxHealth_ = maxHealth_;

        currentHealth_ = Mathf.Clamp(currentHealth_, 0, maxHealth_);

        CurrentHealth_ = currentHealth_;
    }
}
