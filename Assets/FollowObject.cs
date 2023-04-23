using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public Transform obj;
    public bool rotation;
    public bool x;
    public bool y;
    public bool z;

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = new Vector3(obj.position.x * x.GetHashCode(), obj.position.y * y.GetHashCode(), obj.position.z * z.GetHashCode());
        transform.position = pos;
        if(rotation) transform.rotation = obj.rotation;
    }
}
