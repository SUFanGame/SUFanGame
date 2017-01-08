using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.Util;

namespace SUGame.Util
{
    [ExecuteInEditMode]
    public class PixelPerfectCamera : MonoBehaviour
    {

        [SerializeField]
        int visualScale_ = 2;

        /// <summary>
        /// Note Screen.height/width seem to be bugged when called from OnValidate, it reports
        /// incorrect values. Setting them in OnGUI seems to work.
        /// </summary>
        int[] screenDims_ = new int[2];


        public bool snapPosition_;

        //static PixelPerfectCamera instance_;

        Vector3 preRenderPosition_;

        Camera cam_;

        //GUIStyle sliderBoxStyle_ = null;

        public bool scaleOrthoSize_ = true;
        public float targetOrthoSize_ = 10f;

        public bool showGUI_ = false;

        void OnEnable()
        {
            //instance_ = this;
        }

        void Awake()
        {
            cam_ = GetComponent<Camera>();
            preRenderPosition_ = transform.position;
        }

        public void OnValidate()
        {
            if (!isActiveAndEnabled)
                return;

            float ppu = Constants.PIXELS_TO_UNITS;

            //Debug.Log("ADJUSTING CAMERA");

            ppu = Mathf.Clamp(ppu, 1, int.MaxValue);

            visualScale_ = Mathf.Clamp(visualScale_, 1, int.MaxValue);

            float scaledPPU = GetScaledPPU();


            float ySize = screenDims_[1];//screenDims_[1] % 2 == 0 ? screenDims_[1] : screenDims_[1];
            cam_.orthographicSize = ySize / scaledPPU * .5f;
        }

        void Update()
        {
            preRenderPosition_ = transform.position;
            if (screenDims_[0] != Screen.width || screenDims_[1] != Screen.height)
            {
                screenDims_[0] = Screen.width;
                screenDims_[1] = Screen.height;
                OnValidate();
            }
        }

        float GetScaledPPU()
        {
            float scaledPPU = 0;
            float ppu = Constants.PIXELS_TO_UNITS;
            if (scaleOrthoSize_)
            {
                float tarScaleVal = Mathf.Floor((float)screenDims_[1] * .5f / (float)targetOrthoSize_);
                float scale = Mathf.Floor(tarScaleVal / ppu);
                scaledPPU = Mathf.Clamp(scale * ppu, ppu, float.MaxValue);
            }
            else
            {
                scaledPPU = ppu * visualScale_;
            }
            return scaledPPU;
        }

        // Snap to pixel position before rendering
        void OnPreRender()
        {

            if (!snapPosition_)
                return;


            preRenderPosition_ = transform.position;

            float ppu = GetScaledPPU();

            var pos = transform.position;

            for (int i = 0; i < 2; ++i)
            {
                pos[i] = Mathf.Round(pos[i] * ppu) / ppu;
            }

            transform.position = pos;
        }

        // Return to original position after rendering
        void OnPostRender()
        {
            if (snapPosition_)
            {
                transform.position = preRenderPosition_;
            }
        }

        void OnGUI()
        {
            if (!showGUI_)
                return;


            //GUILayout.Label(string.Format("ScreenDims: {0}, {1}", Screen.width, Screen.height));
            //GUILayout.Label(cam_.pixelRect.ToString());

            //GUILayout.Label("Orthographic Size: " + cam_.orthographicSize);

            var boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.normal.background = Texture2D.whiteTexture;
            var labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.normal.textColor = Color.black;
            GUILayout.BeginVertical(boxStyle);
            var toggleStyle = new GUIStyle(GUI.skin.toggle);
            // Fuck this....
            toggleStyle.normal.textColor = Color.black;
            toggleStyle.active.textColor = Color.black;
            toggleStyle.focused.textColor = Color.black;
            toggleStyle.hover.textColor = Color.black;
            toggleStyle.onHover.textColor = Color.black;
            toggleStyle.onActive.textColor = Color.black;
            toggleStyle.onNormal.textColor = Color.black;
            toggleStyle.onActive.textColor = Color.black;

            var toggle = GUILayout.Toggle(scaleOrthoSize_, "Auto Scale", toggleStyle);

            if (GUI.changed && toggle != scaleOrthoSize_)
            {
                GUI.changed = false;
                scaleOrthoSize_ = toggle;
                OnValidate();
            }

            if (scaleOrthoSize_)
            {
                float targetSize = GUILayout.HorizontalSlider(targetOrthoSize_, 1f, 12f);
                GUILayout.Label("Target Size " + targetOrthoSize_, labelStyle);
                if (GUI.changed && !Mathf.Approximately(targetSize, targetOrthoSize_))
                {
                    GUI.changed = false;
                    targetOrthoSize_ = targetSize;
                    OnValidate();
                }
            }
            else
            {
                int scale = (int)GUILayout.HorizontalSlider(visualScale_, 1, 16);
                GUILayout.Label("Scale: " + scale, labelStyle);
                if (GUI.changed && scale != visualScale_)
                {
                    GUI.changed = false;
                    visualScale_ = scale;
                    OnValidate();
                }
            }


            GUILayout.EndVertical();

            GUILayout.Label("Screen Height: " + screenDims_[1], labelStyle);


        }
    }
}