using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    public class Swarmer : Enemy
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private float attackRange;

        private bool isStopped;

        private void Update()
        {
            if (!agent.isActiveAndEnabled) return;

            if (!hasTarget || isStopped)
            {
                agent.isStopped = true;
                return;
            }
            
            if ((target.transform.position - transform.position).sqrMagnitude < attackRange * attackRange)
            {
                Attack(target.GetComponent<Player>());
                return;
            }

            agent.isStopped = false;
            agent.speed = moveSpeed;
            agent.SetDestination(target.position);
        }

        protected override void OnEnable()
        {
            isStopped = false;
            base.OnEnable();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

        private void Attack(Player player)
        {
            agent.isStopped = true;
            isStopped = true;
            //agent.enabled = false;
            StartCoroutine(AttackCoroutine(player));
        }

        private IEnumerator AttackCoroutine(Player player)
        {
            print("start attack");

            yield return new WaitForSeconds(attackSpeed);

            if ((player.transform.position - transform.position).sqrMagnitude < attackRange * attackRange)
            {
                player.TakeHit((int) damage, (player.transform.position - transform.position).normalized);
            }
            
            //agent.enabled = true;
            agent.isStopped = false;
            isStopped = false;
            print("finished attack");
        }

        protected override IEnumerator Die()
        {
            //agent.enabled = false;
            agent.isStopped = true;
            isStopped = true;
            yield return base.Die();
        }
    }
}