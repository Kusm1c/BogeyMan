using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.AI;

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

        [SerializeField] protected EnemySettings_SO settings;
        [SerializeField] private SphereCollider focusSphere;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private new Rigidbody rigidbody;
        [SerializeField] protected NavMeshAgent agent;

        protected bool hasTarget;
        private Spawner spawner;
        private MaterialPropertyBlock propertyBlock => _propertyBlock ??= new MaterialPropertyBlock();
        private MaterialPropertyBlock _propertyBlock;
        private Transform _target;
        private bool isDead;
        protected bool isStopped;

        private int hp;
        private static readonly int color = Shader.PropertyToID("_BaseColor");

        protected virtual void Awake()
        {
            hp = settings.maxHP;
            rigidbody.Sleep();
            spawner = gameObject.GetComponentInParent<Spawner>();
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            if (settings.debug)
            {
                Debug();
            }
#endif
            
            if (!rigidbody.IsSleeping() && rigidbody.velocity.magnitude < 0.1f)
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

            if ((target.transform.position - transform.position).sqrMagnitude <
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
            meshRenderer.GetPropertyBlock(propertyBlock);
            Color oldColor = _propertyBlock.GetColor(color);
            propertyBlock.SetColor(color, new Color(oldColor.r, oldColor.g, oldColor.b, 1f));
            meshRenderer.SetPropertyBlock(propertyBlock);
            meshRenderer.shadowCastingMode = ShadowCastingMode.On;

            hp = settings.maxHP;
            isDead = false;
            hasTarget = false;
            isStopped = false;
        }
        
        private void OnDisable()
        {
            if (spawner != null)
            {
                spawner.SwarmerDeath(gameObject);
            }
        }

        private void OnValidate()
        {
            focusSphere.radius = settings.focusRange;
            agent.speed = settings.moveSpeed;
            agent.angularSpeed = settings.angularSpeed;
            agent.acceleration = settings.acceleration;
            rigidbody.mass = settings.weight;
            rigidbody.drag = settings.linearDrag;
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, settings.attackRange);
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

        private IEnumerator Die()
        {
            agent.isStopped = true;
            isStopped = true;
            isDead = true;
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            meshRenderer.GetPropertyBlock(propertyBlock);
            Color oldColor = _propertyBlock.GetColor(color);

            float length = settings.disappearanceTime;

            while (length > 0f)
            {
                meshRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor(color, new Color(oldColor.r, oldColor.g, oldColor.b, length / settings.disappearanceTime));
                meshRenderer.SetPropertyBlock(propertyBlock);
                
                yield return null;
                length -= Time.deltaTime;
            }

            propertyBlock.SetColor(color, new Color(oldColor.r, oldColor.g, oldColor.b, 0f));
            meshRenderer.SetPropertyBlock(propertyBlock);
            gameObject.SetActive(false);
        }

		#region IGrabableImplementation
		public void Grab()
        {
            GetComponent<NavMeshAgent>().isStopped = true;
        }

        public void Release()
        {
            transform.parent = null;
            GetComponent<NavMeshAgent>().isStopped = false;
        }

        public void Throw()
        {

        }

        public void Impact()
        {
            GetComponent<NavMeshAgent>().isStopped = false;
        }
        #endregion IGrabableImplementation

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