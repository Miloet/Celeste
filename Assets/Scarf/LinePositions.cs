using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinePositions : MonoBehaviour
{
    LineRenderer LR;
    public Transform[] points;
    public GameObject Camera;
    PlayerMovement PM;
    public Gradient noDash;
    public Gradient withDash;

    // Start is called before the first frame update
    void Start()
    {
        LR = GetComponent<LineRenderer>();
        LR.positionCount = points.Length;
        PM = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < LR.positionCount; i++)
        {
            LR.SetPosition(i, points[i].position);
        }
        Camera.transform.position = points[0].position - new Vector3(0,0,10);
        if (PM.CanDash) LR.colorGradient = withDash;
        else LR.colorGradient = noDash;
        
    }
}
