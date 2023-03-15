using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class Swarmer : Enemy
    {
        [SerializeField] private Animator animator;
        
        private WaitForSeconds attackWait;
        
        private static readonly int runOffset = Animator.StringToHash("RunOffset");
        private static readonly int isRunning = Animator.StringToHash("IsRunning");
        private static readonly int hit = Animator.StringToHash("Hit");

        protected override void Awake()
        {
            attackWait = new WaitForSeconds(settings.attackSpeed);
#if UNITY_EDITOR
            currentAttackSpeed = settings.attackSpeed;
#endif
            base.Awake();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            animator.SetFloat(runOffset, Random.Range(0, .5f));
        }

        protected override void Attack(Player player)
        {
            agent.isStopped = true;
            isStopped = true;
            
            StartCoroutine(AttackCoroutine(player));
        }

        private IEnumerator AttackCoroutine(Player player)
        {
            animator.SetTrigger(hit);
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