using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    public class EnemyTest : MonoBehaviour
    {
        [SerializeField] private float speed = 1f;
    
        private Vector3 target;
        private Vector3 direction;
        private float distance;
    
        private NavMeshAgent agent;
    
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

            if (distance > 1f)
            {
                agent.SetDestination(target);
            }
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
}
