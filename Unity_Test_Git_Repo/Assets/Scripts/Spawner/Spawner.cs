using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float timeBetweenWaves = 5f;
    [SerializeField] public Wave[] waves;
    [SerializeField] private bool developerMode;
    
    private Wave currentWave;
    private int currentWaveNumber;

    private LivingEntity playerEntity;
    private Transform playerTransform;
    
    private int enemiesRemainingToSpawn;
    private int enemiesRemainingAlive;
    private bool bossIsSpawned = false;
    private bool bossStillAlive = true;
    [SerializeField] private float nextSpawnTime = 2;
    [SerializeField] private float spawnDelay = 1;
    [SerializeField] private float tileFlashSpeed = 4;

    bool isDisabled;
    private MapGenerator map;
    public event System.Action<int> OnNewWave;
    public event System.Action OnBossSpawning;

    private void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerTransform = playerEntity.transform;
        playerEntity.OnDeath += OnPlayerDeath;
        map = FindObjectOfType<MapGenerator>();
        NextWave();
    }

    private void Update()
    {
        if (!isDisabled)
        {
            if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
            {
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;
                StartCoroutine("SpawnEnemy");
            }
            else if (enemiesRemainingAlive == 0 && bossIsSpawned == false && Time.time > nextSpawnTime)
            {
                bossIsSpawned = true;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;
                StartCoroutine("SpawnBoss");
            }
        }
        
        if (developerMode)
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                StopCoroutine("SpawnEnemy");
                StopCoroutine("SpawnBoss");
                foreach (Enemy item in FindObjectsOfType<Enemy>())
                {
                    GameObject.Destroy(item.gameObject);
                }
                NextWave();
            }
        }
    }
    IEnumerator SpawnEnemy()
    {
        Transform randomTile = map.GetRandomOpenTile();
        Material tileMat = randomTile.GetComponent<Renderer>().material;
        Color initialColor = tileMat.color;
        Color flashColor = Color.red;
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(Color.clear, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));
            spawnTimer += Time.deltaTime;
            yield return null;
        }
        tileMat.color = initialColor;
        if (!isDisabled)
        {
            int randomMob = Random.Range(0, currentWave.smallEnemy.Length);
            Enemy spawnedEnemy = Instantiate(currentWave.smallEnemy[randomMob], randomTile.position + Vector3.up, Quaternion.identity) as Enemy;
            if (spawnedEnemy != null)
            {
                spawnedEnemy.SetCharacteristics(currentWave.moveSpeed[randomMob], currentWave.hitsToKillPlayer[randomMob], currentWave.enemyHealth[randomMob]);
                spawnedEnemy.OnDeath += OnEnemyDeath;
            }
        }
    }

    IEnumerator SpawnBoss()
    {
        Vector3 mapCenter = map.GetTileFromPosition(Vector3.zero).position + Vector3.up;

        if (OnBossSpawning!= null)
        {
            OnBossSpawning();
            yield return new WaitForSeconds(3);
        }

        int randomBoss = Random.Range(0, currentWave.bigEnemy.Length);
        Enemy spawnedEnemy = Instantiate(currentWave.bigEnemy[randomBoss], mapCenter + Vector3.up, Quaternion.identity) as Enemy;
        if (spawnedEnemy != null)
        {
            spawnedEnemy.SetCharacteristics(currentWave.bossMoveSpeed[randomBoss], currentWave.bossHitsToKillsPlayer[randomBoss], currentWave.bossHealth[randomBoss]);
            spawnedEnemy.OnDeath += OnBossDeath;
        }
    }

    void OnPlayerDeath()
    {
        isDisabled = true;
    }

    private void OnEnemyDeath()
    {
        enemiesRemainingAlive--;
    }

    private void OnBossDeath()
    {
        bossStillAlive = false;

        if (bossStillAlive == false)
        {
            StartCoroutine(NextWaveInterval());
        }
    }

    private void ResetPlayerPosition()
    {
        playerTransform.position = map.GetTileFromPosition(Vector3.zero).position;
    }
    private void NextWave()
    {
        currentWaveNumber++;
        if (currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;
            bossIsSpawned = false;
            bossStillAlive = true;
            if (OnNewWave != null)
            {
                OnNewWave(currentWaveNumber);
            }
            ResetPlayerPosition();
        } 
    }

    IEnumerator NextWaveInterval()
    {
        yield return new WaitForSeconds(timeBetweenWaves);

        NextWave();
    }


    [System.Serializable]
    public class Wave
    {
        public Enemy[] smallEnemy;
        public Enemy[] bigEnemy;

        public int enemyCount;
        public float timeBetweenSpawns;

        public float[] moveSpeed;
        public int[] hitsToKillPlayer;
        public float[] enemyHealth;

        // boss info
        public float[] bossHealth;
        public float[] bossMoveSpeed;
        public int[] bossHitsToKillsPlayer; // hitsToKillPlayer


        //public Color skinColor;

    }
}
