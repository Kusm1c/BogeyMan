using UnityEngine;

namespace Enemies
{
    public class FocusTrigger : MonoBehaviour
    {
        [SerializeField] private Enemy enemy;

        // private void OnTriggerEnter(Collider other)
        // {
        //     enemy.target = other.transform;
        // }
        //
        // private void OnTriggerExit(Collider other)
        // {
        //     enemy.target = null;
        // }
    }
}