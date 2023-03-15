using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooler : MonoBehaviour
{
    public static Pooler instance;
    
    private Dictionary<string, Pool> pools = new();
    [SerializeField] private List<PoolKey> poolKeys = new();

    private GameObject objectInstance;
    private int i;

    private void Awake()
    {
        instance = this;

        InitPools();
        PopulatePools();
    }

    private void PopulatePools()
    {
        foreach (var pool in pools)
        {
            PopulatePool(pool.Value);
        }
    }

    private void PopulatePool(Pool pool)
    {
        for (i = 0; i < pool.baseCount; i++)
        {
            AddInstance(pool);
        }
    }

    private void AddInstance(Pool pool)
    {
        objectInstance = Instantiate(pool.prefab, transform);
        objectInstance.SetActive(false);
        
        pool.queue.Enqueue(objectInstance);
    }

    private void InitPools()
    {
        for (i = 0; i < poolKeys.Count; i++)
        {
            pools.Add(poolKeys[i].key, poolKeys[i].pool);
        }
    }

    private void Start()
    {
        InitRefreshCount();
    }

    private void InitRefreshCount()
    {
        foreach (KeyValuePair<string, Pool> pool in pools)
        {
            StartCoroutine(RefreshPool(pool.Value));
        }
    }

    IEnumerator RefreshPool(Pool pool)
    {
        yield return new WaitForSeconds(pool.refreshSpeed);

        if (pool.queue.Count < pool.baseCount)
        {
            AddInstance(pool);
            pool.refreshSpeed = pool.baseRefreshSpeed * pool.queue.Count / pool.baseCount;
        }
        /*else if (pool.queue.Count > pool.baseCount)
        {
            AddInstance(pool);
            pool.refreshSpeed = pool.baseRefreshSpeed * pool.queue.Count / pool.baseCount;
        }*/

        StartCoroutine(RefreshPool(pool));
    }

    public GameObject Pop(string key)
    {
        if (pools[key].queue.Count == 0)
        {
            AddInstance(pools[key]);
        }
        
        objectInstance = pools[key].queue.Dequeue();
        objectInstance.SetActive(true);

        return objectInstance;
    }

    public GameObject Pop(string key, Vector3 position)
    {
        GameObject go = Pop(key);
        go.transform.position = position;

        return go;
    }

    public void DePop(string key, GameObject go)
    {
        pools[key].queue.Enqueue(go);

        go.transform.parent = transform;
        go.SetActive(false);
    }

    public void DelayedDePop(float t, string key, GameObject go)
    {
        StartCoroutine(DelayedDePopCoroutine(t, key, go));
    }

    private IEnumerator DelayedDePopCoroutine(float t, string key, GameObject go)
    {
        yield return new WaitForSeconds(t);
        
        DePop(key, go);
    }
    
    [System.Serializable]
    private class Pool
    {
        public GameObject prefab;
        public Queue<GameObject> queue = new();

        public int baseCount;
        public float baseRefreshSpeed = 5;
        [HideInInspector] public float refreshSpeed;
    }
    
    [System.Serializable]
    private class PoolKey
    {
        public string key;
        public Pool pool;
    }
}
