using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class Swarmer : Enemy
    {
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject attackCollider;

        private WaitForSeconds attackWait;
        private readonly WaitForSeconds waitForPointOneSeconds = new(0.1f);
        
        private static readonly int runOffset = Animator.StringToHash("RunOffset");
        private static readonly int isRunning = Animator.StringToHash("IsRunning");
        private static readonly int hit = Animator.StringToHash("Hit");

        protected override void Awake()
        {
            attackWait = new WaitForSeconds(settings.attackSpeed);
            attackCollider.SetActive(false);
#if UNITY_EDITOR
            currentAttackSpeed = settings.attackSpeed;
#endif
            base.Awake();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            animator.SetFloat(runOffset, Random.Range(0, .5f));
            attackCollider.SetActive(false);
        }

        protected override void Move()
        {
            base.Move();
            animator.SetBool(isRunning, true);
        }

        protected override void StopMoving()
        {
            animator.SetBool(isRunning, false);
        }

        protected override void Attack()
        {
            agent.isStopped = true;
            isStopped = true;
            
            StartCoroutine(AttackCoroutine());
        }

        private IEnumerator AttackCoroutine()
        {
            animator.SetTrigger(hit);
            yield return attackWait;

            if (isDead || isGrabbed)
            {
                yield break;
            }

            attackCollider.SetActive(true);
            agent.isStopped = false;
            isStopped = false;
            
            yield return waitForPointOneSeconds;
            attackCollider.SetActive(false);
        }
        
        public void HitPlayer()
        {
            var player = target.GetComponent<Player>();
            
            if ((player.transform.position - transform.position).sqrMagnitude <
                settings.attackRange * settings.attackRange)
            {
                player.TakeHit((int) settings.damage, (player.transform.position - transform.position).normalized);
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