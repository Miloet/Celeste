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

    Vector3 position;

    public Vector2 max;
    public Vector2 min;
    private void Start()
    {
        if (Save.respawnPoint != Vector2.zero)
        {
            position = Save.respawnPoint;
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
            Vector2 pos = position + (Following.position+ (Vector3)rb.velocity.normalized*2 - position) / speed;
            position = new Vector3(pos.x, pos.y, -10);

            position = new Vector3(
            Mathf.Clamp(position.x, min.x, max.x),
            Mathf.Clamp(position.y, min.y, max.y), -10);
            
            
        }
        else
        {
            if (IsVectorBetween()) newPos = false;
            Vector2 pos = position + (new Vector3(
            Mathf.Clamp(position.x, min.x, max.x),
            Mathf.Clamp(position.y, min.y, max.y),-10) - position) / (speed/2);
            position = new Vector3(pos.x, pos.y, -10);
            
        }
        transform.position = position + new Vector3(ScreenShake.x, ScreenShake.x);
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
            Mathf.Clamp(position.x, min.x-1, max.x+1), 
            Mathf.Clamp(position.y, min.y-1, max.y+1));
        return (Vector2)position == v;
    }
}
