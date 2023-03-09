using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    public class Swarmer : Enemy
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private float attackRange;
        

        private void Update()
        {
            if (!agent.isActiveAndEnabled) return;

            if (target is null)
            {
                agent.isStopped = true;
                return;
            }
            
            if ((target.transform.position - transform.position).sqrMagnitude < attackRange * attackRange)
            {
                Attack(target.GetComponent<PlayerController>());
                return;
            }

            agent.isStopped = false;
            agent.speed = moveSpeed;
            agent.SetDestination(target.position);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

        protected override void Attack(PlayerController player)
        {
            agent.isStopped = true;
            agent.enabled = false;
            StartCoroutine(AttackCoroutine(player));
        }

        private IEnumerator AttackCoroutine(PlayerController player)
        {
            print("start attack");

            yield return new WaitForSeconds(attackSpeed);

            if ((player.transform.position - transform.position).sqrMagnitude < attackRange * attackRange)
            {
                player.GetComponent<Rigidbody>().AddForce((player.transform.position - transform.position).normalized * 1500f);
            }
            
            agent.enabled = true;
            agent.isStopped = false;
            print("finished attack");
        }

        protected override IEnumerator Die()
        {
            agent.enabled = false;
            yield return base.Die();
        }
    }
}