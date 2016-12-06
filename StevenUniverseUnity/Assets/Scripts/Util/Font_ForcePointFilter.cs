using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace SUGame.Util
{
    /// <summary>
    /// Forces the font on all text UI elements on this object and all children of this object to be
    /// point-filter. This prevents annoying anti-aliasing artifacts when using low-resolution text.
    /// Note the canvas which contains these texts should also be set to "Pixel-Perfect" to avoid
    /// other artifacts.
    /// </summary>
    [ExecuteInEditMode]
    public class Font_ForcePointFilter : MonoBehaviour
    {

        public bool allChildren_ = true;

        void Awake()
        {
            if( allChildren_ )
            {
                var texts = GetComponentsInChildren<Text>();
                
                foreach( var t in texts )
                {
                    if (t == null || t.font == null)
                        continue;
                    t.font.material.mainTexture.filterMode = FilterMode.Point;
                }
            }
            else
            {
                var text = GetComponent<Text>();

                if (text == null)
                    return;

                text.material.mainTexture.filterMode = FilterMode.Point;
            }
        }

    }
}