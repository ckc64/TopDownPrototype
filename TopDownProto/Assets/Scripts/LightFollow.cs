using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class LightFollow : MonoBehaviour
{
    public GameObject player;
    public float lightRange;
    public Light l;
    // Update is called once per frame

    void Start()
    {
        l = GetComponent<Light>();
        lightRange = 6;
        
    }

    void Update()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null) {
            transform.position = player.transform.position + Vector3.up * 2f;
            l.range = lightRange;
        }
       
    }
}
