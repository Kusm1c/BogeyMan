using System;
using System.Collections;
using UnityEngine;

namespace Enemies
{
    public class Swarmer : Enemy
    {
        private WaitForSeconds attackWait;

        protected override void Awake()
        {
            attackWait = new WaitForSeconds(settings.attackSpeed);
#if UNITY_EDITOR
            currentAttackSpeed = settings.attackSpeed;
#endif
            base.Awake();
        }

        protected override void Attack(Player player)
        {
            agent.isStopped = true;
            isStopped = true;
            
            StartCoroutine(AttackCoroutine(player));
        }

        private IEnumerator AttackCoroutine(Player player)
        {
            yield return attackWait;

            if (isDead || isGrabbed)
            {
                yield break;
            }
            
            try
            {
                if ((player.transform.position - transform.position).sqrMagnitude <
                    settings.attackRange * settings.attackRange)
                {
                    player.TakeHit((int) settings.damage, (player.transform.position - transform.position).normalized);
                }
            }
            finally
            {
                agent.isStopped = false;
                isStopped = false;
            }
        }

#if UNITY_EDITOR
        private float currentAttackSpeed;
        
        protected override void Debug()
        {
            if (Math.Abs(currentAttackSpeed - settings.attackSpeed) > 0.001f)
            {
                attackWait = new WaitForSeconds(settings.attackSpeed);
            }
            
            base.Debug();
        }
#endif
    }
}