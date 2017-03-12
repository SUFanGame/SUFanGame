using UnityEngine;
using System.Collections;

public class Orbit : MonoBehaviour
{

    public Transform target_;
    public float fRadius = 3.0f;
    public Vector3 targetPos_;
    public float direction = 1;
    public float speed_ = 5f;

    void Awake()
    {
        if( target_ != null )
        {
            targetPos_ = target_.position;
        }
    }

    void Update()
    {
        float angle = Mathf.Repeat(Time.time * speed_, 360f);// * Mathf.Rad2Deg;
        Vector3 tarPos = targetPos_ + Quaternion.AngleAxis(angle, Vector3.forward * direction) * (Vector3.right * fRadius);
        var pos = transform.position;
        pos.x = tarPos.x;
        pos.y = tarPos.y;
        transform.position = pos;
    }
}