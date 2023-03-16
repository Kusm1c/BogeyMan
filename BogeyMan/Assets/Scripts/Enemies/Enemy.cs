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

        private new Collider collider {
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
        [HideInInspector] public bool isDead;
        protected bool isStopped;
        protected bool isGrabbed;

        private int hp;
        private static readonly int color = Shader.PropertyToID("_BaseColor");

        protected virtual void Awake()
        {
            hp = settings.maxHP;
            rigidbody.Sleep();
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            if (settings.debug)
            {
                Debug();
            }
#endif

            if (!rigidbody.IsSleeping() && rigidbody.velocity.magnitude < 0.1f && isGrabbed == false)
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.Sleep();
                agent.enabled = true;
            }

            if (!agent.isActiveAndEnabled) return;

            if (!hasTarget || isStopped)
            {
                agent.isStopped = true;
                return;
            }

            if (!isGrabbed &&
                (target.transform.position - transform.position).sqrMagnitude <
                settings.attackRange * settings.attackRange)
            {
                Attack(target.GetComponent<Player>());
            }

            agent.isStopped = false;
            agent.speed = settings.moveSpeed;
            agent.SetDestination(target.position);
        }

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

        private void OnDrawGizmosSelected()
        {
            if (settings != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, settings.attackRange);
            }
        }

        protected abstract void Attack(Player player);

        public void TakeHit(float strength, Vector3 direction, int damage = 1)
        {
            if (isDead) return;

            agent.enabled = false;
            rigidbody.WakeUp();
            rigidbody.velocity = direction * strength;

            TakeHit(damage);
        }

        private void TakeHit(int damage = 1)
        {
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
                propertyBlock.SetColor(color, new Color(oldColor.r, oldColor.g, oldColor.b, length / settings.disappearanceTime));
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

        }

        public void OnImpact()
        {
            agent.enabled = true;
            isGrabbed = false;

            Collider[] enemiesHit;
            enemiesHit = Physics.OverlapSphere(transform.position, settings.impactRadius);
            foreach(Collider hit in enemiesHit)
			{
                Enemies.Enemy enemy = hit.transform.GetComponent<Enemies.Enemy>();
                if (enemy != null)
				{
                    enemy.TakeHit(settings.impactForce, (enemy.transform.position - transform.position).normalized, settings.damageOnImpact);
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
