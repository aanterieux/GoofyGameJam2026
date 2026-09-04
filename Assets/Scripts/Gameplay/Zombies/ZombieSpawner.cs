using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] private GameObject zombiePrefab = null;
    [SerializeField] private int maxSpawnedZombies = 10;
    [SerializeField] private int zombiesPerSpawn = 1;
    [SerializeField] private float spawnWaitingTime = 3.5f;
    [SerializeField] private float spawnRadius = 3f;
    [SerializeField] private bool isActive = true;

    private ZombieSpawner self = null;
    private ZombieManager manager = null;
    private float spawnTimer = 0f;
    private int zombieSpawnedCount = 0;

    private void Awake()
    {
        self = GetComponent<ZombieSpawner>();
    }

    private void Start()
    {
        manager = FindAnyObjectByType<ZombieManager>();

        if (!manager)
        {
            manager = Instantiate(new ZombieManager());
        }
    }

    private void Update()
    {
        if (!manager.IsSpawnAllowed)
        {
            return;
        }

        isActive = (zombieSpawnedCount < maxSpawnedZombies);

        if (!isActive)
        {
            return;
        }

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnWaitingTime)
        {
            SpawnZombie();

            spawnTimer = 0f;
        }
    }


    private void SpawnZombie()
    {
        Vector2 horizontalOffset;
        Vector3 selfPos = transform.position;
        Vector3 spawnPos;
        Zombie newZombie;

        for (int i = 0; i < zombiesPerSpawn; ++i)
        {
            horizontalOffset = spawnRadius * Random.insideUnitCircle;
            spawnPos =
                selfPos
                + new Vector3(
                    horizontalOffset.x,
                    transform.position.y,
                    horizontalOffset.y
                );

            spawnPos = spawnRadius * Random.insideUnitSphere;
            spawnPos.x += 0.1f * selfPos.x * (Random.Range(0, 2) == 0 ? 1f : -1f);
            spawnPos.y = transform.position.y;
            spawnPos.z += 0.1f * selfPos.z * (Random.Range(0, 2) == 0 ? 1f : -1f);

            newZombie =
                Instantiate(
                    zombiePrefab,
                    spawnPos,
                    Quaternion.identity,
                    transform
                ).GetComponent<Zombie>();
            newZombie.LinkToSpawner(self);

            zombieSpawnedCount++;
            manager.IncrementZombieCount();
        }
    }

    public void NotifyZombieDeath()
    {
        zombieSpawnedCount--;
        manager.DecrementZombieCount();
    }
}
