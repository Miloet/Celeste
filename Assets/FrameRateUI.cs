using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FrameRateUI : MonoBehaviour
{
    TextMeshProUGUI text;
    public Gradient color;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        float frames = 1.0f / Time.deltaTime;
        text.text = (1.0f / Time.deltaTime).ToString("0.0");
        text.color = color.Evaluate(frames / 60f);
        
    }
}
