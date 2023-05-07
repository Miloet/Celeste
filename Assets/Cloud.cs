using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Cloud : MonoBehaviour
{
    public float setSpeed = 0;
    private GameObject player;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private PlayerMovement pm;
    private Collider2D col;
    private bool trigger;
    private static float speed = 2f; 
    Vector2 startPos;
    public bool active;

    private void Start()
    {
        if (setSpeed != 0) speed = setSpeed;
        player = GameObject.Find("Player");
        rb = player.GetComponent<Rigidbody2D>();
        startPos = transform.position;
        sr = player.GetComponent<SpriteRenderer>();
        pm = player.GetComponent<PlayerMovement>();
        col = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && pm.Dashing == false && active == false)
        {
            pm.CanDash = true;
            StartCoroutine(CloudJump());
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        trigger = !collision.CompareTag("Player");
    }

    private IEnumerator CloudJump()
    {
        active = true;
        trigger = true;
        float startTime = 1.5f;
        float time = startTime;
        float height;
        Vector2 offset = Vector2.zero;

        while (time > 0)
        {
            height = sr.bounds.size.y / 2f;

            offset = new Vector2(0, Mathf.Pow((startTime - time - 0.5f) * 2f, 2)-1);
            transform.position = startPos + offset;
            player.transform.position = new Vector2(player.transform.position.x, 
                transform.position.y - height + col.bounds.size.y/2);

            rb.velocity = new Vector2(rb.velocity.x, 0);

            yield return null;
            time -= Time.deltaTime;
            if (pm.JumpTime > Time.time || !trigger) break;
        }
        rb.velocity = new Vector2(rb.velocity.x, offset.y*speed);
        trigger = false;

        time = 0f;
        while(transform.position != (Vector3)startPos)
        {
            time += Time.deltaTime;
            transform.position = Vector2.Lerp(transform.position, startPos, time);
            yield return null;
        }
        transform.position = startPos;
        active = false;
    }
}


