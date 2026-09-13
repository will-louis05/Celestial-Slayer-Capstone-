using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.Behavior;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    private List<Transform> spawnLocations = new List<Transform>();
    [Header("Enemy Types")]
    [SerializeField] private GameObject basicEnemy;
    [SerializeField] private GameObject bruteEnemy;
    [SerializeField] private GameObject flyingEnemy;
    [SerializeField] private WaveData[] waveData;
    [SerializeField] private GameObject tempBruteSpawnerEffect;
    [SerializeField] private GameObject tempBasicSpawnerEffect;
    private int totalWaves;
    private int currentWave;
    public int currentEnemyCount;

    [Header("DeveloperTools")]
    public bool spawnEnemies;
    [SerializeField] private GameObject combatEvent;

    
    void Start()
    {
        totalWaves = waveData.Length;
    }

    void Update()
    {
        if (currentWave == totalWaves && currentEnemyCount <= 0)
        {
            if (combatEvent != null)
            {
                combatEvent.GetComponent<ICombatEvent>().PostCombatEvent();
            }
            Destroy(gameObject);
            PlayerController.inCombat = false;

        }
        else if (spawnEnemies && currentEnemyCount <= 0)
        {
            PlayerController.inCombat = true;
            if (currentWave == totalWaves)
            {
                Destroy(gameObject);
                return;
            }
            SpawnEnemies();
            currentWave++;
        }
    }

    private void SpawnEnemies()
    {
        List<Transform> availableSpawns = new List<Transform>();
        GameObject[] enemiesTypes = { basicEnemy, bruteEnemy, flyingEnemy };

        if (waveData[currentWave].waveSpawns != null)
        {
            availableSpawns = spawnLocations;
            int spawnLocationsCount = waveData[currentWave].waveSpawns.childCount;
            for (int i = 0; i < spawnLocationsCount; i++)
            {
                availableSpawns.Add(waveData[currentWave].waveSpawns.GetChild(i));
            }
        }
        else
        {
            //Populate SpawnLocation Array
            Transform spawnLocationsParent = transform.Find("SpawnLocations");
            int spawnLocationsCount = spawnLocationsParent.childCount;
            for (int i = 0; i < spawnLocationsCount; i++)
            {
                spawnLocations.Add(spawnLocationsParent.GetChild(i));
            }
            availableSpawns = spawnLocations;
        }

        bool randomEnemySpawns = waveData[currentWave].randomSpawns;

        if (randomEnemySpawns)
            RandomSpawn(availableSpawns, enemiesTypes);
        else
            RegularSpawn(availableSpawns);
    }

    private void RandomSpawn(List<Transform> availableSpawns, GameObject[] enemyTypes)
    {
        for (int i = 0; i < waveData[currentWave].enemyTypeSpawnNumberForRandomOnly.Length; i++)
        {
            for (int j = 0; j < waveData[currentWave].enemyTypeSpawnNumberForRandomOnly[i]; j++)
            {
                //Get random location from set locations
                int randomIndex = UnityEngine.Random.Range(0, (spawnLocations.Count - 1));
                Vector3 randomSpawnLocation = availableSpawns[randomIndex].position;

                //SpawnEnemy at random Location and set its parent as the spawner
                GameObject spawnedEnemy = Instantiate(enemyTypes[i], randomSpawnLocation, Quaternion.identity);
                Enemy enemyScrp = spawnedEnemy.GetComponent<Enemy>();
                enemyScrp.spawner = this;
                availableSpawns.RemoveAt(randomIndex);
                currentEnemyCount++;
            }
            BehaviorGraphAgent behaviorGraph = enemyTypes[i].GetComponent<BehaviorGraphAgent>();

            if (behaviorGraph != null)
            {
                GameObject player = GameObject.Find("Player");
                behaviorGraph.BlackboardReference.SetVariableValue("Target (Player)", player);
            }
        }
    }

    private void RegularSpawn(List<Transform> availableSpawns)
    {
        bool firstBasicEnemySpawn = true;
        bool firstBruteEnemySpawn = true;
        int availableSpawnsCount = availableSpawns.Count;
        for (int i = 0; i < availableSpawnsCount; i++)
        {
            GameObject enemySpawned = null;
            GizmoSpawnLocations.EnemyType enemyType = availableSpawns[0].GetComponent<GizmoSpawnLocations>().enemyType;
            GameObject spawnVFX = null;
            switch (enemyType)
            {
                case GizmoSpawnLocations.EnemyType.basic:
                    enemySpawned = Instantiate(basicEnemy, availableSpawns[0].position, Quaternion.identity);
                    spawnVFX = Instantiate(tempBasicSpawnerEffect, availableSpawns[0].position + Vector3.up * 0.6f, Quaternion.identity);
                    //if (firstBasicEnemySpawn)
                    //{
                    //    BehaviorGraphAgent behaviorGraph = enemySpawned.GetComponent<BehaviorGraphAgent>();
                    //    GameObject player = GameObject.Find("Player");
                    //    behaviorGraph.BlackboardReference.SetVariableValue("Target (Player)", player);
                    //    firstBasicEnemySpawn = false;
                    //}
                    enemySpawned.GetComponent<Enemy>().spawner = this;
                    break;
                case GizmoSpawnLocations.EnemyType.brute:
                    enemySpawned = Instantiate(bruteEnemy, availableSpawns[0].position, Quaternion.identity);
                    spawnVFX = Instantiate(tempBruteSpawnerEffect, availableSpawns[0].position + Vector3.up * 2.5f, Quaternion.identity);
                    //if (firstBruteEnemySpawn)
                    //{
                    //    BehaviorGraphAgent behaviorGraph = enemySpawned.GetComponent<BehaviorGraphAgent>();
                    //    GameObject player = GameObject.Find("Player");
                    //    behaviorGraph.BlackboardReference.SetVariableValue("Target (Player)", player);
                    //    firstBruteEnemySpawn = false;
                    //}
                    enemySpawned.GetComponent<Enemy>().spawner = this;
                    break;
                case GizmoSpawnLocations.EnemyType.flying:
                    enemySpawned = Instantiate(flyingEnemy, availableSpawns[0].position, Quaternion.identity);
                    enemySpawned.GetComponent<FlyingEnemy>().spawner = this;
                    break;
            }
            availableSpawns.RemoveAt(0);
            currentEnemyCount++;
            if (spawnVFX != null)
            {
                StartCoroutine(VFXDestroyer(spawnVFX));
            }
            BehaviorGraphAgent behaviorGraph = enemySpawned.GetComponent<BehaviorGraphAgent>();
            GameObject player = GameObject.Find("Player");
            behaviorGraph.BlackboardReference.SetVariableValue("Target (Player)", player);
        }
    }

    private IEnumerator VFXDestroyer(GameObject spawnVFX)
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(spawnVFX);
    }
}

[Serializable]
public struct WaveData
{
    public Transform waveSpawns;
    public bool randomSpawns;
    public int[] enemyTypeSpawnNumberForRandomOnly;
}
