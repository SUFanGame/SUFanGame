using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlipLocalAxis : MonoBehaviour 
{

    void Awake()
    {
        var scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
