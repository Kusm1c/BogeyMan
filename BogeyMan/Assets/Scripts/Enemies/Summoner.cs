using System;
using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Enemies
{
    public class Summoner : Enemy
    {
        [SerializeField] private Spawner spawner;
        [SerializeField] private SummonerArms arm1;
        [SerializeField] private SummonerArms arm2;
        [SerializeField] private int firstSpawnCount;
        [SerializeField] private float spawnDelay;
        [SerializeField, Tooltip("x is inner radius, y is outer radius")] private Vector2 comfortZoneRadius;

        private Transform myTransform;
        private WaitForSeconds attackWait;
        private readonly Clock spawnClock = new();
        

        protected override void Awake()
        {
            attackWait = new WaitForSeconds(settings.attackSpeed);
            myTransform = transform;
#if UNITY_EDITOR
            currentAttackSpeed = settings.attackSpeed;
#endif
            base.Awake();
        }

        private void Start()
        {
            spawner.SpawnSwarm(firstSpawnCount);
            spawnClock.Start();
            agent.updateRotation = false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            spawnClock.Start();
        }

        protected override void Update()
        {
#if UNITY_EDITOR
            if (settings.debug)
            {
                Debug();
            }
#endif

            if (UpdateChecks())
                return;

            myTransform.LookAt(target.transform, Vector3.up);
            myTransform.eulerAngles = Vector3.up * myTransform.eulerAngles.y;

            Vector3 targetDirection = target.transform.position - myTransform.position;
            if (!isGrabbed && attackCooldownClock.GetTime() > settings.attackCooldown + settings.attackSpeed &&
                targetDirection.sqrMagnitude < settings.attackRange * settings.attackRange)
            {
                agent.ResetPath();
                Attack();
                attackCooldownClock.Restart();
                return;
            }

            if (targetDirection.sqrMagnitude < comfortZoneRadius.x * comfortZoneRadius.x || 
                targetDirection.sqrMagnitude > comfortZoneRadius.y * comfortZoneRadius.y)
            {
                MoveTo(target.transform.position + (-targetDirection).normalized * ((comfortZoneRadius.y + comfortZoneRadius.x) * 0.5f));
            }

            if (spawnClock.GetTime() > spawnDelay)
            {
                spawner.SpawnSwarm(1);
                spawnClock.Restart();
            }
        }
        
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            Vector3 position = transform.position;
            Vector3 up = transform.up;
                
#if UNITY_EDITOR
            Handles.color = Color.green;
            Handles.DrawSolidDisc(position, up, comfortZoneRadius.y);
            Handles.color = Color.red;
            Handles.DrawSolidDisc(position, up, comfortZoneRadius.x);
#endif
        }

        private void MoveTo(Vector3 position)
        {
            agent.isStopped = false;
            agent.speed = settings.moveSpeed;
            agent.SetDestination(position);
        }

        protected override void StopMoving()
        {
            
        }

        protected override void Attack()
        {
            agent.isStopped = true;
            isStopped = true;

            UnityEngine.Debug.Log("attack");
            StartCoroutine(AttackCoroutine());
        }

        private IEnumerator AttackCoroutine()
        {
            yield return attackWait;

            if (isDead || isGrabbed)
            {
                yield break;
            }
            
            try
            {
                var player = target.GetComponent<Player>();
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

        protected override IEnumerator Die() // Stunned
        {
            //agent.isStopped = true;
            isStopped = true;
            isDead = true;

            yield return new WaitForSeconds(settings.disappearanceTime);
            
            arm1.gameObject.SetActive(true);
            arm2.gameObject.SetActive(true);
            UnityEngine.Debug.Log("grab");

            yield return new WaitForSeconds(arm1.summonerReleaseTime);
            // only if not dead (DieForReal method)
            
            UnityEngine.Debug.Log("ungrab");
            arm1.Cancel();
            arm2.Cancel();
            arm1.gameObject.SetActive(false);
            arm2.gameObject.SetActive(false);
            isStopped = false;
            isDead = true;
            hp = settings.maxHP;

            // float length = settings.disappearanceTime;
            //
            // Transform meshTransform = transform.GetChild(0);
            // while (length > 0f)
            // {
            //     meshTransform.Rotate(meshTransform.forward, Time.deltaTime / settings.disappearanceTime * 90);
            //     yield return null;
            //     length -= Time.deltaTime;
            // }
        }

        public void DieForReal()
        {
            StopAllCoroutines();
            
            Pooler.instance.DePop("Summoner", gameObject);

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