using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public ObjectPool objectPool;

    [Header("Spawn Area")]
    public float minX = -5f;
    public float maxX = 5f;
    public float spawnY = 0f;

    [Header("Spawn Timing")]
    public float initialMinSpawnInterval = 1f;
    public float initialMaxSpawnInterval = 3f;
    public float minimumPossibleInterval = 0.2f;
    public float intensityIncreaseRate = 0.01f; // how much faster it gets per second

    private float currentMinSpawnInterval;
    private float currentMaxSpawnInterval;
    private float nextSpawnTime;

    private Ninja player;

    void Start()
    {
        currentMinSpawnInterval = initialMinSpawnInterval;
        currentMaxSpawnInterval = initialMaxSpawnInterval;
        ScheduleNextSpawn();
    }

    void Update()
    {
        if (GameManager.Instance.GameRunning)
        {
            player = FindObjectOfType<Ninja>();
        }
        else
        {
            return;
        }
        
        // Gradually decrease intervals
        currentMinSpawnInterval = Mathf.Max(minimumPossibleInterval,
            currentMinSpawnInterval - intensityIncreaseRate * Time.deltaTime);
        currentMaxSpawnInterval = Mathf.Max(minimumPossibleInterval,
            currentMaxSpawnInterval - intensityIncreaseRate * Time.deltaTime);

        if (Time.time >= nextSpawnTime)
        {
            SpawnObject();
            ScheduleNextSpawn();
        }
    }

    void ScheduleNextSpawn()
    {
        float randomInterval = Random.Range(currentMinSpawnInterval, currentMaxSpawnInterval);
        nextSpawnTime = Time.time + randomInterval;
    }

    void SpawnObject()
    {
        if (objectPool == null) return;

        float randomX = Random.Range(minX, maxX);
        Vector3 spawnPos = new Vector3(randomX, spawnY, 0f);

        GameObject obj = objectPool.GetFromPool(spawnPos);

        if (obj.TryGetComponent<Fruit>(out var fruit))
        {
            fruit.SetPool(objectPool, player);
        }
    }
    
    public void OnRestart()
    {
        Fruit[] spawnedFruits = FindObjectsOfType<Fruit>();
        foreach (Fruit fruit in spawnedFruits)
        {
            if (fruit.gameObject.activeSelf)
            {
                objectPool.ReturnToPool(fruit.gameObject);
            }
        }
        currentMinSpawnInterval = initialMinSpawnInterval;
        currentMaxSpawnInterval = initialMaxSpawnInterval;
    }
}