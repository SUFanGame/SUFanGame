using UnityEngine;
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
public class PixelGridEffect : ImageEffectBase
{
    public float pixelsPerUnit_ = 16;
    [Range(0, 1f)]
    public float tValue_ = .5f;
    public Color gridColor_ = new Color(0, 0, 0, 1);
    [Range(-20, 20f)]
    public float xOffset_ = 0;
    [Range(-10f, 10f)]
    public float yOffset_ = 0;

    public bool showGUI_ = true;

    string pixelGridShaderName_ = "Hidden/Pixel Grid Effect";

    void Awake()
    {
        shader = Shader.Find(pixelGridShaderName_);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest) 
    {
        Graphics.Blit(src, dest, material );
    }

    void OnEnable()
    {
        SetMaterialValues();
    }

    void OnValidate()
    {
        if (!isActiveAndEnabled)
            return;

        SetMaterialValues();
    }

    void SetMaterialValues()
    {
        material.SetFloat("_PixelsPerUnit", pixelsPerUnit_);
        material.SetFloat("_TValue", tValue_);
        material.SetColor("_GridColor", gridColor_);
        material.SetFloat("_XOffset", xOffset_);
        material.SetFloat("_YOffset", yOffset_);
    }

    void OnGUI()
    {
        if (!showGUI_)
            return;

        //var screenRes = new IntVector2(Screen.width, Screen.height);
        //var orthoSize = Camera.main.orthographicSize;
        //Vector2 divedRes = screenRes / (orthoSize * 2);
        //float scaleX = (float)screenRes.x / ((float)orthoSize * 2) / (float)PxGlobals.pixelsPerUnit_;
        //float scaleY = (float)screenRes.y / ((float)orthoSize * 2) / (float)PxGlobals.pixelsPerUnit_;

        //GUILayout.Label(string.Format("Res: {0}, OrthoSize: {1}", screenRes, orthoSize) );
        //GUILayout.Label(string.Format("Dived Res: {0}", divedRes));
        //GUILayout.Label(string.Format("ScaleX: {0}, FracX: {1}", scaleX, scaleX - Mathf.FloorToInt(scaleX)));
        //GUILayout.Label(string.Format("ScaleY: {0}, FracY: {1}", scaleY, scaleY - Mathf.FloorToInt(scaleY)));


    }

}
