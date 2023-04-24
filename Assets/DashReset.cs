using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashReset : MonoBehaviour
{

    public AudioClip clip;
    PlayerMovement player;
    Animator An;
    public GameObject particle;
    float active = 0;

    private void Start()
    {
        An = GetComponent<Animator>();
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();

    }
    private void Update()
    {
        if (active > 0)
        {
            active -= Time.deltaTime;
            if (active <= 0)
            {
                An.SetTrigger("Respawn");
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(active <= 0 && player.CanDash == false)
            {
                GameObject p;
                player.ResetDash();
                An.SetTrigger("Die");
                Destroy(p = Instantiate(particle),2);

                p.transform.position = transform.position;
                DestroySelf();
            }
        }
    }

    public void DestroySelf()
    {
        active = 2f;
    }
}
