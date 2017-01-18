using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// YOU SPIN ME RIGHT ROUND BABY RIGHT ROUND
/// </summary>
public class SPIN : MonoBehaviour 
{
    void Update()
    {
        transform.Rotate(Vector3.forward, 5f);
    }
}
