using System;
using UnityEngine;

namespace Enemies
{
    public class AttackTrigger : MonoBehaviour
    {
        [SerializeField] private Enemy enemy;

        private void OnTriggerEnter(Collider other)
        {
            enemy.HitPlayer();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, GetComponent<SphereCollider>().radius);
        }
    }
}