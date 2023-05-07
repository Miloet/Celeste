using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    //-----------------------------------------------------//
    //  PlayerMovement is a script meant to give the player
    //  movement options beyond just walking and jumping
    //  it has dashes, fast fall and wall jump too.
    //  Knockback might be something to implement later.
    //-----------------------------------------------------//

            #region Variables
            //-----------------------------------------------------//

    #region Movement Attributes
    //-----------------------------------------------------//
        public float BaseSpeed;
        public float JumpHeight;
        public float dash = 1.7f;
        public float Gravity = 2;
        int MaxWallJumps = 3;
    //-----------------------------------------------------//
    #endregion


    #region Storage Varibles / Other
    //-----------------------------------------------------//
        int WallJumpTimes;
        bool WallJumping;
        bool EndOfFixedUpdate;
        [System.NonSerialized] public bool Dashing;
        bool g;
        public LayerMask GroundLayerMask;

        [System.NonSerialized] public static Vector2 playerLimitMax = new Vector2(Mathf.Infinity, Mathf.Infinity);
        [System.NonSerialized] public static Vector2 playerLimitMin = -new Vector2(Mathf.Infinity, Mathf.Infinity);
        [System.NonSerialized] public bool CanDash;
    //-----------------------------------------------------//
    #endregion


    #region Components & Sound
    //-----------------------------------------------------//
        Rigidbody2D RB;
        SpriteRenderer SR;

        ParticleSystem dashParticles;

        AudioClip[] soundsJump;
        AudioClip soundLanding;
        AudioClip soundDash;
    //-----------------------------------------------------//
    #endregion


    #region Keys
    //-----------------------------------------------------//
    float hMove;
        float hMoveRaw;
        bool DashKey;
        float DashTime;
        bool JumpKey;
        [System.NonSerialized] public float JumpTime;
        float FastFallKey;
        bool FastFallPressed;
    //-----------------------------------------------------//
    #endregion


    /*#region region blueprint
    //-----------------------------------------------------//
    varibles
    //-----------------------------------------------------//
    #endregion*/

            //-----------------------------------------------------//
            #endregion


            #region Functions
            //-----------------------------------------------------//

    // Getting the componants
    void Start()
    {
        //Components
        SR = GetComponent<SpriteRenderer>();
        RB = GetComponent<Rigidbody2D>();
        RB.gravityScale = Gravity;

        //Respawn & Save
        if (Save.respawnPoint != Vector2.zero)
        {
            RaycastHit2D ray;
            if(ray = Physics2D.Raycast(Save.respawnPoint,Vector2.down,Mathf.Infinity, GroundLayerMask))
            {
                transform.position = ray.point + new Vector2(0, SR.size.y/2);
            }
        }
        if (Save.pMin != Vector2.zero || Save.pMax != Vector2.zero)
        {
            playerLimitMin = Save.pMin;
            playerLimitMax = Save.pMax;
        }

        //Sound
        soundDash = Resources.Load<AudioClip>("Sounds/dash");
        soundLanding = Resources.Load<AudioClip>("Sounds/landing");
        soundsJump = new AudioClip[3];
        soundsJump[0] = Resources.Load<AudioClip>("Sounds/jump");
        soundsJump[1] = Resources.Load<AudioClip>("Sounds/jump (1)");
        soundsJump[2] = Resources.Load<AudioClip>("Sounds/jump (2)");


        //Flair

        dashParticles = GameObject.Find("DashParticles").GetComponent<ParticleSystem>();

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
        if (Input.GetButtonDown("Fire3")) Save.Respawn();


        if(Mathf.Abs(RB.velocity.x) > 0.1f )
        {
            if(RB.velocity.x > 0.1f) SR.flipX = false;
            if(RB.velocity.x < 0.1f) SR.flipX = true;
        }

        var emission = dashParticles.emission;
        emission.enabled = Dashing;
    }

    //General Movement
    private void FixedUpdate()
    {
        if (JumpTime > Time.time) JumpKey = true;
        else JumpKey = false;
        if (DashTime > Time.time) DashKey = true;
        else DashKey = false;

        g = Grounded();

        RB.gravityScale = Gravity;

        //Fast fall
        if (!g)
        {
            if (FastFallPressed) RB.velocity = new Vector2(RB.velocity.x, -Gravity);
            RB.gravityScale = Gravity - (Gravity * FastFallKey);
        }

        //Horizontal Movement
        if (g && !Dashing && !WallJumping) RB.velocity = new Vector2(hMove * BaseSpeed, RB.velocity.y);
        else if(!Dashing && !WallJumping && hMove != 0) RB.velocity = new Vector2(hMove * BaseSpeed, RB.velocity.y);

        //Jump
        if (g && JumpKey)
        {
            JumpTime = 0;
            RB.velocity = new Vector2(hMoveRaw * BaseSpeed, JumpHeight);
            
            var r = Random.Range(0, soundsJump.Length);
            SoundManager.play(soundsJump[r], gameObject);
        }

        //Wall jump
        if (!g && !WallJumping && !Dashing)
        {
            var RayLeft = WallRaycast(1);
            var RayRight = WallRaycast(-1);

            if (RayLeft && !g) if (hMoveRaw == 1 || hMoveRaw == 0) CanWallJump(1);
            if (RayRight && !g) if (hMoveRaw == -1 || hMoveRaw == 0) CanWallJump(-1);
        }

        //Dash
        if (CanDash && DashKey)
        {
            Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = MousePos - transform.position;
            CanDash = false;
            DashTime = 0;
            StartCoroutine(Dash(dir,0.25f));
        }

        EndOfFixedUpdate = true;

        if (transform.position.y < Mathf.Clamp(transform.position.y, playerLimitMin.y, playerLimitMax.y)) Save.Respawn();
        transform.position = new Vector3(
           Mathf.Clamp(transform.position.x, playerLimitMin.x, playerLimitMax.x),
           Mathf.Clamp(transform.position.y, playerLimitMin.y, playerLimitMax.y), 0);
    }
    public bool Grounded()
    {
        bool ray = Physics2D.Raycast(transform.position - new Vector3(0, SR.bounds.size.y/2), Vector2.down, 0.1f ,GroundLayerMask) ||
            Physics2D.Raycast(transform.position - new Vector3(SR.bounds.size.x/2, SR.bounds.size.y / 2), Vector2.down, 0.1f, GroundLayerMask) ||
            Physics2D.Raycast(transform.position - new Vector3(-SR.bounds.size.x/2, SR.bounds.size.y / 2), Vector2.down, 0.1f, GroundLayerMask);
        if (ray && !Dashing) CanDash = ray;
        if (ray) WallJumpTimes = MaxWallJumps;
        if (ray && !g) SoundManager.play(soundLanding, gameObject);
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
            var r = Random.Range(0, soundsJump.Length);
            SoundManager.play(soundsJump[r], gameObject);
            JumpTime = 0;
        }
    }
    IEnumerator Jump(float Axis)
    {
        WallJumping = true;
        var time = Time.time + 0.3f;
        while (true)
        {
            yield return new WaitUntil(() => EndOfFixedUpdate == true);

            WallJumping = true;
            RB.velocity = new Vector2(Axis * BaseSpeed/3f*2, RB.velocity.y);

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


    #region Dash
    public void ResetDash()
    {
        CanDash = true;
    }

    public IEnumerator Dash(Vector2 Direction, float time)
    {
        SoundManager.play(soundDash, gameObject);
        var shake = Camera.main.GetComponent<ScreenShake>();
        shake.StartCoroutine(shake.Shake(0.1f,0.25f));

        var BaseTime = time;
        time = Time.time + time;
        while(true)
        {
            yield return new WaitUntil(() => EndOfFixedUpdate == true);
            EndOfFixedUpdate = false;
            Dashing = true;

            RB.velocity = Direction.normalized * BaseSpeed * dash;
            RB.gravityScale = 0;


            if (time < Time.time)
            {
                Dashing = false;
                RB.gravityScale = Gravity;
                RB.velocity = new Vector2(RB.velocity.x/2,RB.velocity.y/2);
                break;
            }
        }
    }

    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Spikes"))
        {
            Save.Respawn();
        }
    }

            //-----------------------------------------------------//
            #endregion
}
