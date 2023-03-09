using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    
    private Vector3 target;
    private Vector3 direction;
    private float distance;
    
    [SerializeField] private float avoidanceDistance = 1f;
    
    private NavMeshAgent agent;
    public float stopDistance = 1.5f;
    
    // [SerializeField] private Camera mainCamera;
    // private CameraControl cameraControl;
    
    private void Start()
    {
        // cameraControl = mainCamera.GetComponent<CameraControl>();
        agent = GetComponent<NavMeshAgent>();
    }
    
    private void Update()
    {
        if (!agent.isActiveAndEnabled) return;
        agent.speed = speed;
        target = GameObject.Find("Player").transform.position;
        direction = target - transform.position;
        distance = direction.magnitude;
        //check if activated agent has been placed on a NavMesh
        if (agent.isOnNavMesh)
        {
            agent.SetDestination(target);
        }
        if (distance < stopDistance)
        {
            agent.speed = 0f;
        }
    }

    private void Avoidance()
    {
        
    }

    // private void OnCollisionEnter(Collision collision)
    // {
    //     //if collision with "Wall"
    //     if (collision.gameObject.GetComponent<Wall>() != null)
    //     {
    //         agent.speed = 0f;
    //         StartCoroutine(cameraControl.ScreenShake());
    //         agent.speed = speed;
    //     }
    //
    //     if (collision.gameObject.GetComponent<EnemyTest>() != null)
    //     {
    //         //from where the collision happened and the force of the collision 
    //     }
    // }
}
