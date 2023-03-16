using System;
using UnityEngine;

namespace Enemies
{
    public class AttackTrigger : MonoBehaviour
    {
        [SerializeField] private Swarmer swarmer;

        private void OnTriggerEnter(Collider other)
        {
            swarmer.HitPlayer();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, GetComponent<SphereCollider>().radius);
        }
    }
}