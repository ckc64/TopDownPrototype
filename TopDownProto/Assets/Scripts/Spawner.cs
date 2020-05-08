using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public Wave[] waves;
    public Enemy enemy;

    Wave currentWave;
    int currentWaveNumber;

    int enemiesRemainingToSpawn;
    int enemiesRemainngAlive;
    float nextSpawnTime;

    MapGenerator map;

    //if player stays the same area
    float timeBetweenCampingChecks = 2;
    float campThresholdDistance = 1.5f;
    float nextCampCheckTime;
    Vector3 campPositionOld;
    bool isCamping;

    bool isPlayerDead;

    public event System.Action<int> OnNewWave;

    //Player component
    LivingEntity playerEntity;
    Transform playerTransform;
   
     void Start()
    {
        playerEntity = FindObjectOfType<LivingEntity>();
        playerTransform = playerEntity.transform;

        nextCampCheckTime = timeBetweenCampingChecks + Time.time;
        campPositionOld = playerTransform.position;
        playerEntity.OnDeath += OnPlayerDeath;

        map = FindObjectOfType<MapGenerator>();
        NextWave();
    }
    void Update()
    {
        if (!isPlayerDead)
        {
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;
                isCamping = (Vector3.Distance(playerTransform.position, campPositionOld) < campThresholdDistance);
                campPositionOld = playerTransform.position;
            }
            if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
            {
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;
                StartCoroutine(SpawnEnemy());

            }
        }
      
    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1;
       
        Transform spawnTile = map.GetRandomOpenTile();

        if (isCamping)
        {
            spawnTile = map.GetTileFromPosition(playerTransform.position);
        }

        float spawnTimer = 0;

        while(spawnTimer < spawnDelay)
        {
            spawnTimer += Time.deltaTime;
            yield return null;
        }
        Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeath += OnEnemyDeath;
    }

    void OnPlayerDeath()
    {
        isPlayerDead = true;
    }

    void OnEnemyDeath()
    {
        enemiesRemainngAlive--;
        if(enemiesRemainngAlive == 0)
        {
            NextWave();
        }
    }

    void ResetPlayerPosition()
    {
        playerTransform.position = Vector3.zero + Vector3.up*3;
    }

    void NextWave()
    {
        currentWaveNumber++;
        if(currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainngAlive = enemiesRemainingToSpawn;

            if(OnNewWave != null)
            {
                OnNewWave(currentWaveNumber);
            }
            ResetPlayerPosition();
        }
        
    }

    [System.Serializable]
    public class Wave
    {
        public int enemyCount;
        public float timeBetweenSpawns;
    }
}
