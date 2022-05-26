using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Wave[] waves;
    public Enemy Enemy;
    
    Wave currentWave;
    int currentWaveNumber;

    LivingEntity playerEntity;
    Transform playerTransform;
    
    private int enemiesRemainingToSpawn;
    private int enemiesRemainingAlive;
    public float nextSpawnTime = 2;
    public float spawnDelay = 1;
    public float tileFlashSpeed = 4;

    private MapGenerator map;

    public event System.Action<int> OnNewWave;
    private void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerTransform = playerEntity.transform;
        map = FindObjectOfType<MapGenerator>();
        NextWave();
    }

    private void Update()
    {
        if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
        {
            enemiesRemainingToSpawn--;
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;
            StartCoroutine(SpawnEnemy());
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
            tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

            spawnTimer += Time.deltaTime;
            yield return null;
        }

        Enemy spawnedEnemy = Instantiate(Enemy, randomTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeath += OnEnemyDeath;
    }
    private void OnEnemyDeath()
    {
        enemiesRemainingAlive--;
        if (enemiesRemainingAlive == 0)
        {
            NextWave();
        }
    }
    private void ResetPlayerPosition()
    {
        playerTransform.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up;
    }
    private void NextWave()
    {
        currentWaveNumber++;
        if (currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];
            print($"Wave {currentWaveNumber}" );

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;
            if (OnNewWave != null)
            {
                OnNewWave(currentWaveNumber);
                ResetPlayerPosition();
            }
        } 
    }

    [System.Serializable]
    public class Wave
    {
        public int enemyCount;
        public float timeBetweenSpawns;
    }
}
