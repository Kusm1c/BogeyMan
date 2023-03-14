using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class Spawner : MonoBehaviour
    {
        [FormerlySerializedAs("swarmerType")] [Header("Swarm Settings")] [SerializeField]
        private List<UnitSpawnSettings> units;
        [Serializable]
        private class UnitSpawnSettings
        {
            public string unitName;
            /*[MinMaxSlider(0f, 100f)]*/ public Vector2Int swarmSpawnPartRange;
            public int randomSwarmSpawnPart;
            [Range(0, 100)] public float respawnForLoses;
            
        }

        [SerializeField, Min(0)] private int swarmSize;
        [FormerlySerializedAs("swarmSpawnRadius")] [SerializeField, Min(0f)] private float spawnRadius;
        [FormerlySerializedAs("swarmSpawnSpeed")] [SerializeField, Min(0f)] private float spawnDelay = 2f;
        
        private int aliveCount;
        private readonly WaitForSeconds waitForPointOneSeconds = new(0.1f);
        private WaitForSeconds waitForSpawnDelay;

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                waitForSpawnDelay = new WaitForSeconds(spawnDelay);
            }
            foreach (UnitSpawnSettings unitSettings in units)
            {
                if (unitSettings.swarmSpawnPartRange.x < 0) unitSettings.swarmSpawnPartRange.x = 0;
                if (unitSettings.swarmSpawnPartRange.y < 0) unitSettings.swarmSpawnPartRange.y = 0;
            }
        }

        private void Awake()
        {
            waitForSpawnDelay = new WaitForSeconds(spawnDelay);
        }

        private void Start()
        {
            ChooseRandomSwarmSpawnPart();
            StartCoroutine(SpawnSwarm());
        
        }

        private void ChooseRandomSwarmSpawnPart()
        {
            int sum = 0;
            foreach (UnitSpawnSettings swarmerType in units)
            {
                swarmerType.randomSwarmSpawnPart =
                    Random.Range(swarmerType.swarmSpawnPartRange.x, swarmerType.swarmSpawnPartRange.y);
                sum += swarmerType.randomSwarmSpawnPart;
            }
            if (sum != 100)
            {
                ChooseRandomSwarmSpawnPart();
            }
        }

        private IEnumerator SpawnSwarm()
        {
            if (aliveCount >= swarmSize)
            {
                yield return waitForPointOneSeconds;
                StartCoroutine(SpawnSwarm());
                yield break;
            }

            Vector3 randomPosInRadius;
            {
                Vector3 spawnerPosition = transform.position;
                randomPosInRadius = new Vector3(Random.insideUnitCircle.x * spawnRadius,
                    spawnerPosition.y, Random.insideUnitCircle.y * spawnRadius) + spawnerPosition;
            }
            
            if (Physics.CheckSphere(randomPosInRadius, 1.5f, LayerMask.GetMask("Wall")))
            {
                yield return null;
                StartCoroutine(SpawnSwarm());
                yield break;
            }

            if (aliveCount >= swarmSize) yield break;
            
            GameObject swarmer = Pooler.instance.Pop(units[0].unitName, randomPosInRadius);
            swarmer.SetActive(true);
            aliveCount++;
            yield return waitForSpawnDelay;
            StartCoroutine(SpawnSwarm());
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
    
        public void SwarmerDeath(GameObject swarmer)
        {
            aliveCount--;
            swarmer.SetActive(false);
        }
    }
}