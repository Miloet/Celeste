using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Save : MonoBehaviour
{
    public static Vector2 respawnPoint;
    public static Vector2 min;
    public static Vector2 max;
    public static GameObject player;


    private void Start()
    {
        player = GameObject.Find("Player");
    }
    public void SavePos()
    {
        respawnPoint = transform.position;
        var c = Camera.main.GetComponent<CameraController>();
        min = c.min;
        max = c.max;
    }
    public static void Respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            SavePos();
        }
    }
}
