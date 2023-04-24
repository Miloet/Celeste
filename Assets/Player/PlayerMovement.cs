using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    //-----------------------------------------------------
    //  PlayerMovement is a script meant to give the player
    //  movement options beyond just walking and jumping
    //  it has dashes, fast fall and wall jump too.
    //  Knockback might be something to implement later.
    //-----------------------------------------------------

    #region Variables
    //-----------------------------------------------------

    public float BaseSpeed;
    public float JumpHeight;
    public float Gravity = 2;

    public AudioClip[] jumpSounds;
    public AudioClip Landing;

    //Keys
    float hMove;
    float hMoveRaw;
    bool DashKey;
    float DashTime;
    bool JumpKey;
    float FastFallKey;
    bool FastFallPressed;

    float JumpTime;


    bool CanDash;

    int MaxWallJumps = 3;
    int WallJumpTimes;
    bool WallJumping;

    bool EndOfFixedUpdate;
    bool Dashing;    

    public LayerMask GroundLayerMask;

    bool g;

    Rigidbody2D RB;
    SpriteRenderer SR;

    //-----------------------------------------------------
    #endregion


    #region Functions
    //-----------------------------------------------------

    // Getting the componants
    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        RB = GetComponent<Rigidbody2D>();
        //Get componants
        RB.gravityScale = Gravity;
    }

    //Updating the keys
    void Update()
    {
        if(Input.GetButtonDown("Jump")) JumpTime = Time.time + 0.25f;
        hMove = Input.GetAxis("Horizontal");
        hMoveRaw = Input.GetAxisRaw("Horizontal");
        if(Input.GetButtonDown("Fire2")) DashTime = Time.time + 0.15f;
        FastFallKey = Input.GetAxis("Vertical");
        FastFallPressed = Input.GetButtonDown("Vertical");

        if(Mathf.Abs(RB.velocity.x) > 0.1f )
        {
            if(RB.velocity.x > 0.1f) SR.flipX = false;
            if(RB.velocity.x < 0.1f) SR.flipX = true;
        }
    }


    private void FixedUpdate()
    {
        if (JumpTime > Time.time) JumpKey = true;
        else JumpKey = false;
        if (DashTime > Time.time) DashKey = true;
        else DashKey = false;

        g = Grounded();

        RB.gravityScale = Gravity;
        //Horizontal Movement
        if (g && !Dashing && !WallJumping) RB.velocity = new Vector2(hMove * BaseSpeed, RB.velocity.y);
        else if(!Dashing && !WallJumping && hMove != 0) RB.velocity = new Vector2(hMove * BaseSpeed, RB.velocity.y);

        //Jump
        if (g && JumpKey)
        {
            JumpTime = 0;
            RB.velocity = new Vector2(hMoveRaw * BaseSpeed, JumpHeight);
            
            var r = Random.Range(0, jumpSounds.Length-1);
            SoundManager.play(jumpSounds[r],gameObject);
        }

        //Wall jump
        if (!g && !WallJumping && !Dashing)
        {
            var RayLeft = WallRaycast(1);
            var RayRight = WallRaycast(-1);

            if (RayLeft && !g) if (hMoveRaw == 1 || hMoveRaw == 0) CanWallJump(1);
            if (RayRight && !g) if (hMoveRaw == -1 || hMoveRaw == 0) CanWallJump(-1);
        }

        //Dash constant force in direction of the mouse
        if (CanDash && DashKey)
        {
            Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = MousePos - transform.position;
            StartCoroutine(Dash(dir,0.35f));
        }

        //Fast fall
        if(!g)
        {
            if (FastFallPressed) RB.velocity = new Vector2(RB.velocity.x, -Gravity);
            RB.gravityScale = Gravity - (Gravity * FastFallKey);
        }

        //Crouch
        //Yet to be implemented
        //Will most likely be done with sprites and the shrinking of the hitboxes


        EndOfFixedUpdate = true;
    }

    
    public bool Grounded()
    {
        bool ray = Physics2D.Raycast(transform.position - new Vector3(0, SR.bounds.size.y/2), Vector2.down, 0.1f ,GroundLayerMask) ||
            Physics2D.Raycast(transform.position - new Vector3(SR.bounds.size.x/2, SR.bounds.size.y / 2), Vector2.down, 0.1f, GroundLayerMask) ||
            Physics2D.Raycast(transform.position - new Vector3(-SR.bounds.size.x/2, SR.bounds.size.y / 2), Vector2.down, 0.1f, GroundLayerMask);
        if (ray && !Dashing) CanDash = ray;
        if (ray) WallJumpTimes = MaxWallJumps;
        if (ray == true && g != ray) SoundManager.play(Landing, gameObject);
        return ray;
    }

    #region Wall jump
    //-----------------------------------------------------

    public bool WallRaycast(int dir)
    {
        bool ray = Physics2D.Raycast(transform.position,
            new Vector2(dir,0),
            0.3f + SR.bounds.size.x / 2,
            GroundLayerMask);
        return ray;
    }
    public Vector2 WallRayPos(int dir)
    {
        var ray = Physics2D.Raycast(transform.position,
           new Vector2(dir, 0), 
           0.3f + SR.bounds.size.x / 2, 
           GroundLayerMask);

        return new Vector2(ray.point.x - SR.bounds.size.x/2*dir, ray.point.y);
    }
    public void CanWallJump(int dir)
    {
        transform.position = WallRayPos(dir);
        RB.gravityScale = Gravity / 2;
        RB.velocity = new Vector2(RB.velocity.x, Mathf.Min(5 * Mathf.Sign(RB.velocity.y), RB.velocity.y));
        if(JumpKey && WallJumpTimes >= 1)
        {
            WallJumpTimes--;
            RB.velocity = new Vector2(-dir * BaseSpeed, JumpHeight);
            StartCoroutine(Jump(-dir));
            var r = Random.Range(0, jumpSounds.Length - 1);
            SoundManager.play(jumpSounds[r], gameObject);
            JumpTime = 0;
        }
    }
    IEnumerator Jump(float Axis)
    {
        WallJumping = true;
        var time = Time.time + 0.4f;
        while (true)
        {
            yield return new WaitUntil(() => EndOfFixedUpdate == true);

            WallJumping = true;
            RB.velocity = new Vector2(Axis * BaseSpeed, RB.velocity.y);

            if (Time.time > time)
            {
                WallJumping = false;
                RB.velocity = new Vector2(RB.velocity.x, RB.velocity.y);
                break;
            }
        }
    }

    //-----------------------------------------------------
    #endregion


    public IEnumerator Dash(Vector2 Direction, float time)
    {
        var BaseTime = time;
        time = Time.time + time;
        while(true)
        {
            yield return new WaitUntil(() => EndOfFixedUpdate == true);

            CanDash = false;
            EndOfFixedUpdate = false;
            Dashing = true;

            RB.velocity = Direction.normalized * BaseSpeed*1.7f;
            RB.gravityScale = 0;


            if (time < Time.time)
            {
                Dashing = false;
                RB.gravityScale = Gravity;
                RB.velocity = new Vector2(RB.velocity.x,RB.velocity.y/2);
                break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Spikes"))
        {
            Save.Respawn();
        }
    }

    //-----------------------------------------------------
    #endregion
}
