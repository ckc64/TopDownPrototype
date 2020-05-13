using SimpleInputNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    // Start is called before the first frame update

    public float moveSpeed = 5;

    public Crosshair crosshair;
    Camera viewCamera;
    PlayerController playerController;
    private Enemy closestEnemy;
    GunController gunController;
    Enemy[] allEnemies;
    private float distanceClosestToEnemy;

    public GameObject thumb;
    protected override void Start()
    {
        base.Start();
        playerController = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
      
        viewCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

        crosshair.ActivateCrosshair(false);
        closestEnemy = null;
        allEnemies = GameObject.FindObjectsOfType<Enemy>();
        distanceClosestToEnemy = Mathf.Infinity;
        Vector3 moveInput = new Vector3(SimpleInput.GetAxisRaw("Horizontal"), 0, SimpleInput.GetAxisRaw("Vertical"));
        
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        playerController.Move(moveVelocity);
        Debug.Log(thumb.transform.position);
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);
        float rayDistance;

        if(groundPlane.Raycast(ray,out rayDistance))
        {
           Vector3 point = ray.GetPoint(rayDistance);
            Debug.DrawLine(ray.origin, point, Color.red);
            playerController.LookAt(point);
            if((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1)
            {
                gunController.Aim(point);
            }
            
            
        }

        foreach(Enemy currentEnemy in allEnemies)
        {
            float distanceToEnemy = (currentEnemy.transform.position - this.transform.position).sqrMagnitude;
            if(distanceToEnemy < distanceClosestToEnemy)
            {
                distanceClosestToEnemy= distanceToEnemy;
                closestEnemy = currentEnemy;
                //Debug.Log(distanceClosestToEnemy);
               
                if (distanceClosestToEnemy < 25)
                {
                    crosshair.ActivateCrosshair(true);
                    crosshair.DetectTargets(ray);
                    playerController.LookAt(closestEnemy.transform.position);
                    
                    crosshair.transform.position = closestEnemy.transform.position;
                    gunController.Shoot();
                  
                    
                }
            }
        }
        
          
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gunController.Shoot();
        }
    }
}
