using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GD.MinMaxSlider;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [Header("Swarm Settings")] [SerializeField]
    private List<SwarmerType> swarmerTypes;
    [Serializable]
    private class SwarmerType
    {
        public GameObject swarmerType;
        [MinMaxSlider(0f, 100f)] public Vector2Int randomSpawnChance;
        [Range(0, 100)] public float respawnForLoses;
        public float distanceBetweenSwarmers;
        
        [Header("Pas toucher")]
        public int isAlive;
        public int randomSwarmSpawnPart;
    }

    [SerializeField] private int swarmSize;
    [SerializeField] private float swarmSpawnRadius;
    [SerializeField] private float swarmSpawnSpeed;
    
    [SerializeField] private List<GameObject> swarmersDead = new();
    [SerializeField] private List<GameObject> swarmersAlive = new();

    [ExecuteAlways]
    private void OnValidate()
    {
        if (swarmSize < 0) swarmSize = 0;
        if (swarmSpawnRadius < 0) swarmSpawnRadius = 0;
        if (swarmSpawnSpeed < 0) swarmSpawnSpeed = 0;
        foreach (SwarmerType swarmerType in swarmerTypes)
        {
            if (swarmerType.randomSpawnChance.x < 0) swarmerType.randomSpawnChance.x = 0;
            if (swarmerType.randomSpawnChance.y < 0) swarmerType.randomSpawnChance.y = 0;
            if (swarmerType.respawnForLoses < 0) swarmerType.respawnForLoses = 0;
            if (swarmerType.distanceBetweenSwarmers < 0) swarmerType.distanceBetweenSwarmers = 0;
        }
    }

    private void Start()
    {
        Debug.Log("Started Spawning");
        ChooseRandomSwarmSpawnPart();
        SwarmerPool();
        StartCoroutine(SpawnSwarm());
        
    }


    private void SwarmerPool()
    {
        foreach (SwarmerType swarmerType in swarmerTypes)
        {
            for (int i = 0; i < swarmSize - swarmSize / swarmerType.randomSwarmSpawnPart; i++)
            {
                GameObject swarmer = Instantiate(swarmerType.swarmerType, transform);
                swarmer.GetComponent<NavMeshAgent>().radius = swarmerType.distanceBetweenSwarmers;
                swarmer.SetActive(false);
                swarmersDead.Add(swarmer);
            }
        }
    }

    private void ChooseRandomSwarmSpawnPart()
    {
        int sum = 0;
        foreach (SwarmerType swarmerType in swarmerTypes)
        {
            swarmerType.randomSwarmSpawnPart = Random.Range(swarmerType.randomSpawnChance.x, swarmerType.randomSpawnChance.y);
            sum += swarmerType.randomSwarmSpawnPart;
        }

        if (sum != 100)
        {
            foreach (SwarmerType swarmerType in swarmerTypes)
            {
                swarmerType.randomSwarmSpawnPart = swarmerType.randomSwarmSpawnPart * 100 / sum;
            }
        }
    }

    private IEnumerator SpawnSwarm()
    {
        var randomPosInRadius = new Vector3(Random.insideUnitCircle.x * swarmSpawnRadius,
            transform.position.y, Random.insideUnitCircle.y * swarmSpawnRadius) + transform.position;
        if (Physics.CheckSphere(randomPosInRadius, 1.5f, LayerMask.GetMask("Wall")))
        {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(SpawnSwarm());
        }
        else
        {
            if (swarmersDead.Count > 0)
            {
                GameObject swarmer = swarmersDead[0];
                swarmer.transform.position = randomPosInRadius;
                swarmer.SetActive(true);
                swarmersDead.Remove(swarmer);
                swarmersAlive.Add(swarmer);
                yield return new WaitForSeconds(swarmSpawnSpeed);
                StartCoroutine(SpawnSwarm());
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
                StartCoroutine(SpawnSwarm());
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, swarmSpawnRadius);
    }
    
    public void SwarmerDeath(GameObject swarmer)
    {
        swarmersAlive.Remove(swarmer);
        swarmersDead.Add(swarmer);
        swarmer.SetActive(false);
        foreach (SwarmerType swarmerType in swarmerTypes)
        {
            if (swarmerType.swarmerType == swarmer)
            {
                swarmerType.isAlive--;
                if (swarmerType.isAlive < 0)
                {
                    swarmerType.isAlive = 0;
                }
            }
        }
    }
}