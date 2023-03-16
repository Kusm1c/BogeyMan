using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class Spawner : MonoBehaviour
    {
        [Header("Swarm Settings")] 
        [SerializeField] private List<UnitSpawnSettings> units;
        [SerializeField] private bool automatic = true;
        [SerializeField, Min(0)] private int swarmSize;
        [SerializeField, Min(0f)] private float spawnRadius = 15f;
        [SerializeField, Min(0f)] private float spawnDelay = 2f;

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
                if (unitSettings.randomSpawnChance.x < 0) unitSettings.randomSpawnChance.x = 0;
                if (unitSettings.randomSpawnChance.y < 0) unitSettings.randomSpawnChance.y = 0;
            }
        }

        private void Awake()
        {
            waitForSpawnDelay = new WaitForSeconds(spawnDelay);
        }

        private void Start()
        {
            InitRandomSwarmSpawnPart();
            if (automatic)
            {
                StartCoroutine(SpawnLoopCoroutine());
            }
        }

        private void InitRandomSwarmSpawnPart()
        {
            int sum = 0;
            foreach (UnitSpawnSettings swarmerType in units)
            {
                swarmerType.randomSwarmSpawnPart = Random.Range(swarmerType.randomSpawnChance.x, swarmerType.randomSpawnChance.y);
                sum += swarmerType.randomSwarmSpawnPart;
            }

            if (sum == 100) return;

            foreach (UnitSpawnSettings swarmerType in units)
            {
                swarmerType.randomSwarmSpawnPart = swarmerType.randomSwarmSpawnPart * 100 / sum;
            }
        }

        private IEnumerator SpawnLoopCoroutine()
        {
            if (aliveCount >= swarmSize)
            {
                yield return waitForPointOneSeconds;
                StartCoroutine(SpawnLoopCoroutine());
                yield break;
            }

            SpawnUnit();
            
            yield return waitForSpawnDelay;
            StartCoroutine(SpawnLoopCoroutine());
        }

        public void SpawnSwarm(int unitCount)
        {
            for (int i = 0; i < unitCount; i++)
            {
                SpawnUnit();
            }
        }

        private void SpawnUnit()
        {
            Vector3 randomPosInRadius;
            {
                Vector3 spawnerPosition = transform.position;
                randomPosInRadius = new Vector3(Random.insideUnitCircle.x * spawnRadius,
                    spawnerPosition.y, Random.insideUnitCircle.y * spawnRadius) + spawnerPosition;
            }
            
            // if (Physics.CheckSphere(randomPosInRadius, 1.5f, LayerMask.GetMask("Wall")))
            // {
            //     yield return null;
            //     StartCoroutine(SpawnSwarm());
            //     yield break;
            // }
            
            GameObject swarmer = Pooler.instance.Pop(units[0].unitName, randomPosInRadius);
            swarmer.SetActive(true);
            aliveCount++;
            swarmer.GetComponent<Enemy>().onDeath += SwarmerDeath;
            
            GameObject summonPS = Pooler.instance.Pop("Summon", swarmer.transform.position + Vector3.down * 1.5f);
            summonPS.GetComponent<ParticleSystem>().Play();
            Pooler.instance.DelayedDePop(5f, "Summon", summonPS);
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }

        private void SwarmerDeath(Enemy enemy)
        {
            aliveCount--;
            enemy.onDeath -= SwarmerDeath;
        }
        
        [Serializable]
        private class UnitSpawnSettings
        {
            public string unitName;
            public Vector2Int randomSpawnChance;
            [HideInInspector] public int randomSwarmSpawnPart;
            [Range(0, 100)] public float respawnForLoses;
        }
    }
}
