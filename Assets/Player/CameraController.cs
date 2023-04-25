using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Following;
    public float speed;

    public static float buffer = 2;
    
    bool newPos;

    Rigidbody2D rb;

    public Vector2 max;
    public Vector2 min;
    private void Start()
    {
        if (Save.respawnPoint != Vector2.zero)
        {
            transform.position = Save.respawnPoint;
            max = Save.max;
            min = Save.min;
        }
        rb = Following.GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        if (!newPos)
        {
            Vector2 pos = transform.position + (Following.position+ (Vector3)rb.velocity.normalized*4 - transform.position) / speed;
            transform.position = new Vector3(pos.x, pos.y, -10);

            transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, min.x, max.x),
            Mathf.Clamp(transform.position.y, min.y, max.y), -10);
            
        }
        else
        {
            if (IsVectorBetween()) newPos = false;
            Vector2 pos = transform.position + (new Vector3(
            Mathf.Clamp(transform.position.x, min.x, max.x),
            Mathf.Clamp(transform.position.y, min.y, max.y),-10) - transform.position) / (speed/2);
            transform.position = new Vector3(pos.x, pos.y, -10);
            
        }

    }

    public void NewCameraLimits(Vector2 newMax, Vector2 newMin)
    {
        if (max != newMax || min != newMin)
        {
            max = newMax;
            min = newMin;
            newPos = true;
        }
    }

    bool IsVectorBetween()
    {
        Vector2 v = new Vector2(
            Mathf.Clamp(transform.position.x, min.x-1, max.x+1), 
            Mathf.Clamp(transform.position.y, min.y-1, max.y+1));
        return (Vector2)transform.position == v;
    }
}
