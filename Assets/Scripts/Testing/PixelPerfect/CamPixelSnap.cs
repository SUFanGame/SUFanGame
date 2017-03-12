using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CamPixelSnap : MonoBehaviour 
{
    [Range(1,1920)]
    public int x_;
    [Range(1,1080)]
    public int y_;

    public int scale_ = 1;
    public int pixelsPerUnit_ = 16;

    public bool showGUI_ = true;

    //void LateUpdate()
    //{
    //    var p = transform.position;

    //    float x = 1f / (float)x_ * .5f;
    //    float y = 1f / (float)y_ * .5f;

    //    p.x = Increment(p.x, x);
    //    p.y = Increment(p.y, y);

    //    transform.position = p;
    //}

    void LateUpdate()
    {
        var cam = Camera.main;

        var camSize = GetCamSize();
        cam.orthographicSize = camSize;

        //cam.orthographicSize = camSize;
        float inc = 1f / (float)pixelsPerUnit_;

        var pos = cam.transform.position;
        if( pos.x % inc != 0 )
            pos.x = Increment(pos.x, 1f / (float)pixelsPerUnit_ );
        if( pos.y % inc != 0 )
            pos.y = Increment(pos.y, 1f / (float)pixelsPerUnit_);
        //pos.y = Increment(pos.y, )
        cam.transform.position = pos;
        //cam.rect = r;
    }
    
    float GetCamSize()
    {
        //float x = Screen.width;
        //float y = Screen.height;
        //float camSize = x / (((x / y) * 2f) * ((float)pixelsPerUnit_ * (float)scale_));
        //return camSize;

        //float h = Screen.height;

        //return h / (2f * pixelsPerUnit_ * scale_);

        float h = Screen.height;
        return (h / ((float)pixelsPerUnit_ * (float)scale_)) * .5f;
    }
    
    void OnGUI()
    {
        if (!showGUI_)
            return;

        GUILayout.Label("CamPixelSnap GUI:");

        string str = string.Format("Size: {0}x{1}", Screen.width, Screen.height);
        GUILayout.Label(str);

        var camSize = GetCamSize();

        var oldColor = GUI.color;

        GUI.contentColor = Color.black;

        GUILayout.Label(string.Format("{0},{1}", x_, y_));
        GUILayout.Label("CamSize: " + camSize.ToString() );

        GUI.color = oldColor;
    }

    public static float Increment(float val, float inc)
    {
        var mod = (val % inc);
        return val - Mathf.Abs(mod) + (inc < 0 && mod != 0 ? Mathf.Abs(inc) : 0) + inc;
    }


    void OnValidate()
    {
        if (!isActiveAndEnabled)
            return;

        scale_ = Mathf.Clamp(scale_, 1, int.MaxValue);
    }
    
}
