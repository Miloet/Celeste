using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour
{
    public static float x;
    public static float y;
    public IEnumerator Shake(float Magnitude, float Duration)
    {
        float time = 0f;

        while(time < Duration)
        {
            x = Random.Range(-1f, 1f) * Magnitude;
            y = Random.Range(-1f, 1f) * Magnitude;

            time += Time.deltaTime;

            yield return null;
        }
        x = 0;
        y = 0;
    }

}
