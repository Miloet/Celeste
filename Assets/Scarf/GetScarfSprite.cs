using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetScarfSprite : MonoBehaviour
{
    //method to render from camera
    public Camera renderCamera;
    SpriteRenderer spriteRenderer;
    Texture2D texture;
    RenderTexture renderTexture;
    private void Start()
    {
        renderTexture = renderCamera.targetTexture;
        spriteRenderer = GetComponent<SpriteRenderer>();
        texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        texture.filterMode = FilterMode.Point;
    }
    void Update()
    {
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();   
        spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f,8);
    }
}
