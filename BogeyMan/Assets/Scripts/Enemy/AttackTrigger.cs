using UnityEngine;

namespace Enemy
{
    public class AttackTrigger : MonoBehaviour
    {
        [SerializeField] private Enemy enemy;

        private void OnTriggerEnter(Collider other)
        {
            enemy.target = other.transform;
        }

        private void OnTriggerExit(Collider other)
        {
            enemy.target = null;
        }
    }
}