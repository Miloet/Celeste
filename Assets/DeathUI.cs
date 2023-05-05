using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathUI : MonoBehaviour
{
    public static float currentTime;
    public static float baseTime;
    public static bool doSelf;
    public static float speed = 1;

    Image image;

    void Start()
    {
        baseTime = speed;
        currentTime = speed;
        doSelf = true;
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTime >= 0)
        {
            if (doSelf) currentTime -= Time.deltaTime;
            image.color = new Color(0, 0, 0, currentTime / baseTime);
        }
    }
}
