using System;
using System.Collections;
using UnityEngine;

namespace Enemies
{
    public class Summoner : Enemy
    {
        [SerializeField] private Spawner spawner;
        [SerializeField] private int firstSpawnCount;
        [SerializeField] private float spawnDelay;
        
        
        private WaitForSeconds attackWait;
        private readonly Clock spawnClock = new();
        

        protected override void Awake()
        {
            attackWait = new WaitForSeconds(settings.attackSpeed);
#if UNITY_EDITOR
            currentAttackSpeed = settings.attackSpeed;
#endif
            base.Awake();
        }

        private void Start()
        {
            spawner.SpawnSwarm(firstSpawnCount);
            spawnClock.Start();
        }

        protected override void Update()
        {
            base.Update();

            if (spawnClock.GetTime() > spawnDelay)
            {
                spawner.SpawnSwarm(1);
                spawnClock.Restart();
            }
        }

        protected override void Attack(Player player)
        {
            agent.isStopped = true;
            isStopped = true;

            UnityEngine.Debug.Log("attack");
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

        protected override IEnumerator Die()
        {
            //agent.isStopped = true;
            isStopped = true;
            isDead = true;

            float length = settings.disappearanceTime;

            Transform meshTransform = transform.GetChild(0);
            while (length > 0f)
            {
                meshTransform.Rotate(meshTransform.forward, Time.deltaTime / settings.disappearanceTime * 90);
                yield return null;
                length -= Time.deltaTime;
            }

            Pooler.instance.DePop("Summoner", gameObject); // TODO
            
            onDeath?.Invoke(this);
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