using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{   

    public enum State { Idle, Chasing, Attacking};
    State currentState;

    public ParticleSystem deathEffect;

    NavMeshAgent pathFinder;
    Transform target;
    LivingEntity playerEntity;

    Material skinMaterial;

    Color originalColor;

    float attackDistanceThreshold = .5f;
    float timeBeetweenAttacks = 1;
    float damage = 1;
    float nextAttackTime;
    float collisionRadius;
    float playerCollisionRadius;

    bool hasTarget;

    private void Awake()
    {
        pathFinder = GetComponent<NavMeshAgent>();
       
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
           
            hasTarget = true;
            target = GameObject.FindGameObjectWithTag("Player").transform;
            playerEntity = target.GetComponent<LivingEntity>();
          

            collisionRadius = GetComponent<CapsuleCollider>().radius;
            playerCollisionRadius = GetComponent<CapsuleCollider>().radius;

          
        }
    }
    protected override void Start()
    {
        base.Start();
   
      
        
        if(hasTarget)
        {
            currentState = State.Chasing;
           
            playerEntity.OnDeath += OnTargetDeath;

            StartCoroutine(UpdatePath());
        }
        
    }

    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {

        if(damage >= health)
        {
           Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection))as GameObject, deathEffect.main.startLifetime.constant);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }

    // Update is called once per frame
    void Update()
    {
        if (hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float sqrDistanceToTarget = (target.position - transform.position).sqrMagnitude;
                if (sqrDistanceToTarget < Mathf.Pow(attackDistanceThreshold + collisionRadius + playerCollisionRadius, 2))
                {
                    nextAttackTime = Time.time + timeBeetweenAttacks;
                    
                    StartCoroutine(Attack());
                }
            }
        } 
    }

    IEnumerator Attack()
    {
        currentState = State.Attacking;
        pathFinder.enabled = false;
        Vector3 currentPosition = transform.position;

        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - directionToTarget * (collisionRadius);


        float percent = 0;
        float attackSpeed = 3;

        skinMaterial.color = Color.red;
        bool hasAppliedDamage = false;
        while (percent <= 1)
        {

            if (percent >= .5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                playerEntity.TakeDamage(damage);
            }

            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(currentPosition, attackPosition, interpolation);

            yield return null;
        }
        skinMaterial.color = originalColor;
        currentState = State.Chasing;
        pathFinder.enabled = true;
    }


    public void SetCharacteristics(float moveSpeed, int hitsToKillPlayer, float enemyHealth, Color skinColor)
    {
        pathFinder.speed = moveSpeed;
        if (hasTarget)
        {
            damage = Mathf.Ceil(playerEntity.startingHealth / hitsToKillPlayer);
        }
        startingHealth = enemyHealth;
        skinMaterial = GetComponent<Renderer>().material;
        skinMaterial.color = skinColor;
        originalColor = skinMaterial.color;
    }
    IEnumerator UpdatePath()
    {
        
        while(hasTarget)
        {
            if(currentState == State.Chasing)
            {
                Vector3 directionToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - directionToTarget * (collisionRadius + playerCollisionRadius + attackDistanceThreshold/2);
                if (!dead)
                {
                    pathFinder.SetDestination(targetPosition);
                }
            }
           
            
            yield return new WaitForSeconds(0.25f);
        }
    }
}
