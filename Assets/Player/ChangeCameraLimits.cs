using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ChangeCameraLimits : MonoBehaviour
{
    public Vector2 size;
    Vector2 max;
    Vector2 min;
    Camera camera;
    public BoxCollider2D box;

    private void Awake()
    {
        //box = GetComponent<BoxCollider2D>();
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
        // Get the size of the viewing frustum
        float horizontalSize = camera.orthographicSize * camera.aspect;
        float verticalSize = camera.orthographicSize;

        // Calculate the half-width and half-height
        float halfWidth = horizontalSize;
        float halfHeight = verticalSize;

        box.size = new Vector2(size.x + halfWidth*2, size.y + halfHeight*2);
        max = new Vector2(size.x/2, size.y/2);
        min = new Vector2(size.x, size.y);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 v = new Vector2(CameraController.buffer, CameraController.buffer);
            Camera.main.GetComponent<CameraController>().NewCameraLimits((Vector2)transform.position + max+ v, (Vector2)transform.position - max - v);
        } 
    }
}
