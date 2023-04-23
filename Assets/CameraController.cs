using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Following;
    public float speed;

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = transform.position + (Following.position - transform.position)/speed;
        transform.position = new Vector3(pos.x, pos.y, -10);
    }
}
