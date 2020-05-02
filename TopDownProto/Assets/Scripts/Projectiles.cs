using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectiles : MonoBehaviour
{

    float speed = 10;
    public LayerMask collisionMask;

    public void newProjectileSpeed(float newSpeed)
    {
        newSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        float movementDistance = speed * Time.deltaTime;
        transform.Translate(Vector3.forward * movementDistance);
    }

    void CheckCollisions(float movementDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit,movementDistance,collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
        }
    }

    void OnHitObject(RaycastHit hit)
    {
        
            GameObject.Destroy(gameObject);
        
            
    }
}
