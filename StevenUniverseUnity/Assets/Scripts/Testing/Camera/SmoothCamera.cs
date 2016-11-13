using UnityEngine;
using System.Collections;

public class SmoothCamera : MonoBehaviour
{
    public float dampTime = 0.15f;
    private Vector3 velocity = Vector3.zero;
    public Transform target;
    Camera camera_;
    public bool follow_ = false;
    public float scrollSpeed_ = 5f;

    public void SnapToTarget()
    {
        Vector3 point = camera_.WorldToViewportPoint(target.position);
        Vector3 delta = (target.position + (Vector3)Vector2.one * .5f) - camera_.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
        transform.position = transform.position + delta;
    }

    void Awake()
    {
        camera_ = GetComponent<Camera>();

        if( target != null )
        {
            SnapToTarget();
        }
    }

    void Update()
    {
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(hor, ver, 0);

        transform.Translate(move * scrollSpeed_ * Time.deltaTime);
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!follow_)
            return;

        if (target)
        {
            Vector3 point = camera_.WorldToViewportPoint(target.position);
            Vector3 delta = (target.position + (Vector3)Vector2.one * .5f) - camera_.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
            Vector3 destination = transform.position + delta;
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
        }

    }


}