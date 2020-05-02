using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform muzzle;
    public Projectiles projectiles;
    public float millisecondsBetweenShots = 100;
    public float muzzleVelocity = 35;

    float nextShotTime;
    public void Shoot()
    {
        if(Time.time > nextShotTime)
        {
            nextShotTime = Time.time + millisecondsBetweenShots / 1000;
            Projectiles newProjectiles = Instantiate(projectiles, muzzle.position, muzzle.rotation) as Projectiles;
            newProjectiles.newProjectileSpeed(muzzleVelocity);
        }
        
    }
}
