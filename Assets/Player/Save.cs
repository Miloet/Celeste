using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Save : MonoBehaviour
{
    public static Vector2 respawnPoint;
    public static Vector2 min;
    public static Vector2 max;
    public static Vector2 pMin;
    public static Vector2 pMax;
    public static GameObject player;
    public Animator an;
    public GameObject Light;


    private void Start()
    {
        Collider2D col = GetComponent<BoxCollider2D>();
        if(gameObject.name != "UI") col.offset = new Vector2(0,(col.bounds.size.y - 1)/2); 
        player = GameObject.Find("Player");
        an = GetComponent<Animator>();
    }
    public void SavePos()
    {
        respawnPoint = transform.position;
        var c = Camera.main.GetComponent<CameraController>();
        min = c.min;
        max = c.max;
        pMin = PlayerMovement.playerLimitMin;
        pMax = PlayerMovement.playerLimitMax;
    }
    
    public static void Respawn()
    {
        GameObject particles = Resources.Load<GameObject>("Prefabs/DeathParticles");
        
        player.SetActive(false);
        Instantiate(particles).transform.position = player.transform.position;
        Save save = GameObject.Find("UI").AddComponent<Save>();
        save.StartCoroutine(save.ReloadScene(DeathUI.speed));
        SoundManager.play(Resources.Load<AudioClip>("Sounds/hitHurt"), save.gameObject);
    }
    public IEnumerator ReloadScene(float time)
    {
        var t = 0f;
        DeathUI.doSelf = false;
        DeathUI.baseTime = time;
        while (t < time)
        {
            yield return new WaitForFixedUpdate();

            t += Time.deltaTime;
            DeathUI.currentTime = t;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            SavePos();
            Save[] s = FindObjectsOfType<Save>();
            for(int i = 0; i < s.Length; i++)
            {
                if(s[i] != this && s[i].gameObject.name != "UI") s[i].Unlight();
            }
            an.SetBool("Light", true);
            Light.SetActive(true);
        }
    }

    public void Unlight()
    {
        an.SetBool("Light", false);
        Light.SetActive(false);
    }
}
