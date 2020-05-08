using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectiles : MonoBehaviour
{

    float speed = 10;
    public LayerMask collisionMask;
    float damage = 1;

    float bulletLifeTime = 3;
    float skinWidth = .1f;
    void Start()
    {
        Destroy(gameObject, bulletLifeTime);

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, .1f, collisionMask);
        if(initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0], transform.position) ;
        }
    }

    public void newProjectileSpeed(float newSpeed)
    {
        newSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        float movementDistance = speed * Time.deltaTime;
        CheckCollisions(movementDistance);
        transform.Translate(Vector3.forward * movementDistance);
    }

    void CheckCollisions(float movementDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit,movementDistance+skinWidth,collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit.collider,hit.point);
        }
    }



    void OnHitObject(Collider c,Vector3 hitPoint)
    {
        IDamageable damageable = c.GetComponent<IDamageable>();


        if (damageable != null)
        {
            damageable.TakeHit(damage,hitPoint,transform.forward);
        }
        GameObject.Destroy(gameObject);
    }
}
