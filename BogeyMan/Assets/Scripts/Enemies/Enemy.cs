using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

namespace Enemies
{
    public abstract class Enemy : MonoBehaviour, IGrabable
    {
        public Transform target
        {
            get => _target;
            set
            {
                if (value == null)
                {
                    hasTarget = false;
                    _target = null;
                    return;
                }

                hasTarget = true;
                _target = value;
            }
        }

        protected bool isStopped
        {
            get => _isStopped;
            set
            {
                _isStopped = value;
                if (!value)
                {
                    StopMoving();
                }
            }
        }

        private new Collider collider
        {
            get
            {
                if (_collider == null)
                {
                    _collider = GetComponent<Collider>();
                }

                return _collider;
            }
        }

        [SerializeField] protected EnemySettings_SO settings;
        [SerializeField] private SphereCollider focusSphere;
        [SerializeField] private new Renderer renderer;
        [SerializeField] private new Rigidbody rigidbody;
        [SerializeField] protected NavMeshAgent agent;

        private bool hasTarget;
        private MaterialPropertyBlock propertyBlock => _propertyBlock ??= new MaterialPropertyBlock();
        private MaterialPropertyBlock _propertyBlock;
        private Transform _target;
        private Collider _collider;
        private bool _isStopped;
        protected readonly Clock attackCooldownClock = new();
        public bool isDead;
        protected bool isGrabbed;

        protected int hp;
        private static readonly int color = Shader.PropertyToID("_BaseColor");

        protected virtual void Awake()
        {
            hp = settings.maxHP;
            rigidbody.Disable();
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            if (settings.debug)
            {
                Debug();
            }
#endif

            if (UpdateChecks())
                return;

            if (!isGrabbed &&
                (target.transform.position - transform.position).sqrMagnitude <
                settings.attackRange * settings.attackRange)
            {
                if (attackCooldownClock.GetTime() < settings.attackCooldown + settings.attackSpeed) return;

                agent.ResetPath();
                Attack();
                attackCooldownClock.Restart();
                return;
            }

            Move();
        }
        
        public void HitPlayer()
        {
            if (isDead || isGrabbed)
                return;

            var player = target.GetComponent<Player>();
            
            if ((player.transform.position - transform.position).sqrMagnitude <
                settings.attackRange * settings.attackRange)
            {
                player.TakeHit((int) settings.damage, (player.transform.position - transform.position).normalized);
            }
        }

        protected bool UpdateChecks()
        {
            if (isDead)
                return true;
            
            if (!isGrabbed && !rigidbody.isKinematic && rigidbody.velocity.magnitude < 0.1f)
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.Disable();
                agent.enabled = true;
                return true;
            }
            
            if (!agent.isActiveAndEnabled) return true;

            if (!hasTarget || isStopped)
            {
                agent.isStopped = true;
                return true;
            }

            return false;
        }

        protected virtual void Move()
        {
            agent.isStopped = false;
            agent.speed = settings.moveSpeed;
            agent.SetDestination(target.position);
        }

        protected abstract void StopMoving();

        protected virtual void OnEnable()
        {
            renderer.GetPropertyBlock(propertyBlock);
            Color oldColor = _propertyBlock.GetColor(color);
            propertyBlock.SetColor(color, new Color(oldColor.r, oldColor.g, oldColor.b, 1f));
            renderer.SetPropertyBlock(propertyBlock);
            renderer.shadowCastingMode = ShadowCastingMode.On;

            hp = settings.maxHP;
            isDead = false;
            hasTarget = false;
            isStopped = false;
            attackCooldownClock.Start(settings.attackCooldown + settings.attackSpeed + 1f);
        }

        private void OnValidate()
        {
            if (focusSphere != null)
            {
                focusSphere.radius = settings.focusRange;
            }

            if (agent != null)
            {
                agent.speed = settings.moveSpeed;
                agent.angularSpeed = settings.angularSpeed;
                agent.acceleration = settings.acceleration;
            }

            if (rigidbody != null)
            {
                rigidbody.mass = settings.weight;
                rigidbody.drag = settings.linearDrag;
            }
        }

        protected virtual void OnDrawGizmosSelected()
        {
            if (settings != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, settings.attackRange);
            }
        }

        protected abstract void Attack();

        public void TakeHit(float strength, Vector3 direction, int damage = 1)
        {
            TakeHit(damage);
            
            if (isDead) return;
            
            agent.enabled = false;
            rigidbody.Enable();
            rigidbody.velocity = direction * strength;
        }

        private void TakeHit(int damage = 1)
        {
            if (isDead) return;
            
            hp -= damage;

            if (hp >= 1) return;

            StopAllCoroutines();
            StartCoroutine(Die());
        }

        protected virtual IEnumerator Die()
        {
            //agent.isStopped = true;
            isStopped = true;
            isDead = true;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.GetPropertyBlock(propertyBlock);
            Color oldColor = _propertyBlock.GetColor(color);

            float length = settings.disappearanceTime;

            while (length > 0f)
            {
                renderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor(color,
                    new Color(oldColor.r, oldColor.g, oldColor.b, length / settings.disappearanceTime));
                renderer.SetPropertyBlock(propertyBlock);

                yield return null;
                length -= Time.deltaTime;
            }

            propertyBlock.SetColor(color, new Color(oldColor.r, oldColor.g, oldColor.b, 0f));
            renderer.SetPropertyBlock(propertyBlock);
            Pooler.instance.DePop("Swarmer", gameObject); // TODO

            onDeath?.Invoke(this);
        }

        #region IGrabableImplementation

        public virtual bool IsThrowable()
        {
            return settings.throwable;
        }

        public void OnGrab(Player player)
        {
            agent.enabled = false;
            isGrabbed = true;
        }

        public float GetThrowSpeed()
        {
            return settings.throwSpeed;
        }

        public float GetThrowDuration()
        {
            return settings.throwDuration;
        }

        public int GetThrowDamage()
        {
            return settings.damageOnCollisionWhenFlying;
        }

        public float GetThrowForce()
        {
            return settings.forceOnCollisionWhenFlying;
        }

        public Collider GetCollider()
        {
            return collider;
        }

        public void OnRelease()
        {
            transform.parent = null;
            agent.enabled = true;
            isGrabbed = false;
        }

        public void OnThrow()
        {
            rigidbody.Enable();
        }

        public void OnImpact()
        {
            agent.enabled = true;
            isGrabbed = false;

            Collider[] enemiesHit = Physics.OverlapSphere(transform.position, settings.impactRadius);
            foreach (Collider hit in enemiesHit)
            {
                var enemy = hit.transform.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeHit(settings.impactForce, (enemy.transform.position - transform.position).normalized,
                        settings.damageOnImpact);
                }
            }
        }

        #endregion IGrabableImplementation

        public Action<Enemy> onDeath;

#if UNITY_EDITOR

        #region Debug

        protected virtual void Debug()
        {
            focusSphere.radius = settings.focusRange;
            agent.speed = settings.moveSpeed;
            agent.angularSpeed = settings.angularSpeed;
            agent.acceleration = settings.acceleration;
            rigidbody.mass = settings.weight;
            rigidbody.drag = settings.linearDrag;
        }

        #endregion

#endif
    }
}