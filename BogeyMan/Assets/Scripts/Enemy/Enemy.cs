using UnityEngine;

namespace Enemy
{
    public abstract class Enemy : MonoBehaviour
    {
        [HideInInspector] public Transform target;
        
        [SerializeField, Min(0)] protected int maxHP = 1;
        [SerializeField, Min(0)] protected float moveSpeed = 3.5f;
        [SerializeField, Min(0)] protected float attackSpeed = 0.5f;
        [SerializeField, Min(0)] protected float damage = 10f;
        [SerializeField, Min(0)] protected float focusRange = 15f;
        [SerializeField, Min(0)] protected float weight = 1f;
        [SerializeField] private SphereCollider focusSphere;

        private int hp;

        private void OnValidate()
        {
            focusSphere.radius = focusRange;
        }

        public void TakeHit(float strength, Vector3 direction)
        {
            TakeHit();
        }
        
        protected abstract void Attack(PlayerController player);

        private void TakeHit()
        {
            hp--;
        }
    }
}