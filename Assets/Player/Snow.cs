using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snow : MonoBehaviour
{

    public int snowAmount = 10;
    public Vector3 size;
    public Vector2 wind;
    public float offsetAmount = 10f;
    public float baseHeight = 10f;

    ParticleSystem PS;
    ParticleSystemForceField PF;

    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        PF = GetComponent<ParticleSystemForceField>();
        PS = GetComponent<ParticleSystem>();

        player = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        transform.position = new Vector3(
            player.position.x - wind.x * offsetAmount,
            player.position.y - wind.y * offsetAmount + baseHeight + size.z/2, 0);

        var shape = PS.shape;
        shape.scale = size;
        var emission = PS.emission;
        emission.rateOverTime = snowAmount;

        PS.maxParticles = (int)(PS.startLifetime * (float)snowAmount);

        PF.directionX = wind.x;
        PF.directionY = wind.y;
        PF.endRange = Mathf.Max(size.x,size.y)*2;



    }


    public void ChangeWind(Vector2 direction, float speed)
    {
        StartCoroutine(Change(direction,speed));
    }

    public IEnumerator Change(Vector2 dir, float speed)
    {
        float time = 0;

        while(wind != dir)
        {
            wind = new Vector2(
                Mathf.Lerp(wind.x, dir.x, time), 
                Mathf.Lerp(wind.y, dir.y, time));


            yield return null;
            time += Time.deltaTime * speed; 
        }
    }
}
