using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{

    public Transform playerTransform;
    private Vector3 cameraOffset;

    [Range(0.01f,1.0f)]
    public float smoothFactor = 0.5f;
    public bool lookAtPlayer = false;
    // Start is called before the first frame update
    void Start()
    {
        cameraOffset = transform.position - playerTransform.position;
    }

    // LateUpdate is called after Update Methods
    void LateUpdate()
    {
        if(GameObject.FindGameObjectWithTag("Player") != null)
        {
            Vector3 newPos = playerTransform.position + cameraOffset;
            transform.position = Vector3.Slerp(transform.position, newPos, smoothFactor);

            if (lookAtPlayer)
                transform.LookAt(playerTransform);
        }
       
    }
}
