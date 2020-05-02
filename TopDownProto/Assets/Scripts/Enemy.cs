using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    NavMeshAgent pathFinder;
    Transform target;
    void Start()
    {
        pathFinder = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(UpdatePath());
    }

    IEnumerator UpdatePath()
    {
        
        while(target != null)
        {
            Vector3 targetPosition = new Vector3(target.position.x, 0, target.position.z);
            pathFinder.SetDestination(targetPosition);
            yield return new WaitForSeconds(0.25f);
        }
    }
}
