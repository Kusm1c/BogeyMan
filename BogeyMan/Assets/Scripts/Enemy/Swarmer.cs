using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class Swarmer : Enemy
    {
        [SerializeField] private NavMeshAgent agent;

        private void Update() // TODO : stop if target is null
        {
            if (!agent.isActiveAndEnabled) return;
            if (target is null) return;

            agent.speed = moveSpeed;
            agent.SetDestination(target.position);
        }
    }
}