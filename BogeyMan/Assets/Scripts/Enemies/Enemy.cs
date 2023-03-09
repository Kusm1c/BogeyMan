using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Enemies
{
    public abstract class Enemy : MonoBehaviour
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

        [SerializeField, Min(0)] protected int maxHP = 1;
        [SerializeField, Min(0)] protected float moveSpeed = 3.5f;
        [SerializeField, Min(0)] protected float attackSpeed = 0.5f;
        [SerializeField, Min(0)] protected float damage = 10f;
        [SerializeField, Min(0)] protected float focusRange = 15f;
        [SerializeField, Min(0)] protected float weight = 1f;
        [SerializeField] private SphereCollider focusSphere;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField, Min(0)] private float disappearanceTime = 2f;

        protected bool hasTarget;
        private MaterialPropertyBlock propertyBlock => _propertyBlock ??= new MaterialPropertyBlock();
        private MaterialPropertyBlock _propertyBlock;
        private Transform _target;
        private bool isDead;

        private int hp;
        private static readonly int color = Shader.PropertyToID("_BaseColor");

        private void Awake()
        {
            hp = maxHP;
        }

        protected virtual void OnEnable()
        {
            isDead = false;
            hasTarget = false;
        }

        private void OnValidate()
        {
            focusSphere.radius = focusRange;
        }

        public void TakeHit(float strength, Vector3 direction)
        {
            if (isDead) return;
            
            transform.Translate(direction * strength / weight);
            TakeHit();
        }

        [ContextMenu("Take Hit")]
        private void TakeHit()
        {
            hp--;

            if (hp >= 1) return;
            
            StopAllCoroutines();
            StartCoroutine(Die());
        }

        protected virtual IEnumerator Die()
        {
            isDead = true;
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            meshRenderer.GetPropertyBlock(propertyBlock);
            Color oldColor = _propertyBlock.GetColor(color);

            float length = disappearanceTime;

            while (length > 0f)
            {
                meshRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor(color, new Color(oldColor.r, oldColor.g, oldColor.b, length / disappearanceTime));
                meshRenderer.SetPropertyBlock(propertyBlock);
                
                yield return null;
                length -= Time.deltaTime;
            }

            propertyBlock.SetColor(color, new Color(oldColor.r, oldColor.g, oldColor.b, 0f));
            meshRenderer.SetPropertyBlock(propertyBlock);
            gameObject.SetActive(false);
        }
    }
}