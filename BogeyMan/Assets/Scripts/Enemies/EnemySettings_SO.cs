using UnityEngine;

namespace Enemies
{
    [CreateAssetMenu(fileName = "EnemySettings", menuName = "Scriptable Objects/Enemy Settings", order = 0)]
    public class EnemySettings_SO : ScriptableObject
    {
        [Min(0)] public int maxHP = 1;
        [Min(0)] public float attackSpeed = 0.5f;
        [Min(0)] public float attackCooldown = 0.3f;
        [Min(0)] public float damage = 10f;
        [Min(0)] public float focusRange = 15f;
        [Min(0)] public float attackRange = 1.5f;
        
        [Header("Movement")]
        [Min(0)] public float moveSpeed = 3.5f;
        [Min(0)] public float angularSpeed = 120f;
        [Min(0)] public float acceleration = 8f;
        
        [Header("Take Hit")]
        [Min(0)] public float weight = 1f;
        [Min(0)] public float linearDrag = 5f;
        [Min(0)] public float disappearanceTime = 2f;

        [Header("Throw & Impact")]
        public bool throwable = true;
        [Min(0)] public float throwSpeed = 10f;
        [Min(0)] public float throwDuration = 1.5f;
        [Min(0)] public int damageOnCollisionWhenFlying = 1;
        [Min(0)] public float forceOnCollisionWhenFlying = 15f;
        [Min(0)] public int damageOnImpact = 2;
        [Min(0)] public float impactRadius = 3f;
        [Min(0)] public float impactForce = 20f;

#if UNITY_EDITOR
        [Space]
        [Header("=== Click this to Balance in real time ===")]
        public bool debug;
#endif
        
    }
}