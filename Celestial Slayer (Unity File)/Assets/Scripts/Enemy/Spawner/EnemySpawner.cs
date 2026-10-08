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
    [SerializeField] private GameObject sentryEnemy;
    [Header("Enemy SpawnersVFX")]
    [SerializeField] private GameObject basicSpawnerVfx;
    [SerializeField] private GameObject bruteSpawnerVfx;
    [SerializeField] private GameObject flyingSpawnerVfx;
    [SerializeField] private GameObject sentrySpawnerVfx;
    [Header("WaveInfo")]
    [SerializeField] private WaveData[] waveData;
    [SerializeField] private GameObject tempBruteSpawnerEffect;
    [SerializeField] private GameObject tempBasicSpawnerEffect;
    private int totalWaves;
    private int currentWave;
    public int currentEnemyCount;

    [Header("DeveloperTools")]
    public bool spawnEnemies;
    [SerializeField] private GameObject combatEvent;
    [SerializeField] private bool isTut;

    
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
        else if (spawnEnemies &&  currentWave < totalWaves)
        {
            if(currentEnemyCount <= waveData[currentWave].enemyCountTillSpawnNextWave || currentWave == 0)
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
    }

    private void SpawnEnemies()
    {
        List<Transform> availableSpawns = new List<Transform>();
        //GameObject[] enemiesTypes = { basicEnemy, bruteEnemy, flyingEnemy,  };

        if (waveData[currentWave].waveSpawns != null)
        {
            availableSpawns = spawnLocations;
            int spawnLocationsCount = waveData[currentWave].waveSpawns.childCount;
            for (int i = 0; i < spawnLocationsCount; i++)
            {
                availableSpawns.Add(waveData[currentWave].waveSpawns.GetChild(i));
            }
        }
        //else
        //{
        //    //Populate SpawnLocation Array
        //    Transform spawnLocationsParent = transform.Find("SpawnLocations");
        //    int spawnLocationsCount = spawnLocationsParent.childCount;
        //    for (int i = 0; i < spawnLocationsCount; i++)
        //    {
        //        spawnLocations.Add(spawnLocationsParent.GetChild(i));
        //    }
        //    availableSpawns = spawnLocations;
        //}

        //bool randomEnemySpawns = waveData[currentWave].randomSpawns;

        //if (randomEnemySpawns)
        //    RandomSpawn(availableSpawns, enemiesTypes);
        //else
        RegularSpawn(availableSpawns);
    }

    //private void RandomSpawn(List<Transform> availableSpawns, GameObject[] enemyTypes)
    //{
    //    for (int i = 0; i < waveData[currentWave].enemyTypeSpawnNumberForRandomOnly.Length; i++)
    //    {
    //        for (int j = 0; j < waveData[currentWave].enemyTypeSpawnNumberForRandomOnly[i]; j++)
    //        {
    //            //Get random location from set locations
    //            int randomIndex = UnityEngine.Random.Range(0, (spawnLocations.Count - 1));
    //            Vector3 randomSpawnLocation = availableSpawns[randomIndex].position;

    //            //SpawnEnemy at random Location and set its parent as the spawner
    //            GameObject spawnedEnemy = Instantiate(enemyTypes[i], randomSpawnLocation, Quaternion.identity);
    //            Enemy enemyScrp = spawnedEnemy.GetComponent<Enemy>();
    //            enemyScrp.spawner = this;
    //            availableSpawns.RemoveAt(randomIndex);
    //            currentEnemyCount++;
    //        }
    //        BehaviorGraphAgent behaviorGraph = enemyTypes[i].GetComponent<BehaviorGraphAgent>();

    //        if (behaviorGraph != null)
    //        {
    //            GameObject player = GameObject.Find("Player");
    //            behaviorGraph.BlackboardReference.SetVariableValue("Target (Player)", player);
    //        }
    //    }
    //}

    private void RegularSpawn(List<Transform> availableSpawns)
    {
        int availableSpawnsCount = availableSpawns.Count;
        for (int i = 0; i < availableSpawnsCount; i++)
        {
            GameObject enemyToSpawn = null;
            GameObject vfxToSpawn = null;
            GizmoSpawnLocations spawnInfo = availableSpawns[0].GetComponent<GizmoSpawnLocations>();
            GizmoSpawnLocations.EnemyType enemyType = spawnInfo.enemyType;
            float spawnTime = spawnInfo.spawnTime;
            //GameObject spawnVFX = null;
            switch (enemyType)
            {
                case GizmoSpawnLocations.EnemyType.basic:
                    enemyToSpawn = basicEnemy;
                    vfxToSpawn = basicSpawnerVfx;
                    break;

                case GizmoSpawnLocations.EnemyType.brute:
                    enemyToSpawn = bruteEnemy;
                    vfxToSpawn = bruteSpawnerVfx;
                    break;
                        
                case GizmoSpawnLocations.EnemyType.flying:
                    enemyToSpawn = flyingEnemy;
                    vfxToSpawn = flyingSpawnerVfx;
                    break;

                case GizmoSpawnLocations.EnemyType.sentry:
                    enemyToSpawn = sentryEnemy;
                    vfxToSpawn = sentrySpawnerVfx;
                    break;
            }
            StartCoroutine(SpawnEnemy(enemyToSpawn, spawnTime, availableSpawns[0].position, vfxToSpawn));
            availableSpawns.RemoveAt(0);
            currentEnemyCount++;
        }
    }

    private IEnumerator VFXDestroyer(GameObject spawnVFX)
    {
        yield return new WaitForSeconds(1f);
        Destroy(spawnVFX);
    }

    private IEnumerator SpawnEnemy(GameObject enemyToSpawn, float spawnTime, Vector3 spawnLocation, GameObject vfxToSpawn)
    {
        yield return new WaitForSeconds(spawnTime);

        GameObject enemySpawned = Instantiate(enemyToSpawn, spawnLocation, Quaternion.identity);
        enemySpawned.GetComponent<Enemy>().spawner = this;

        GameObject vfxSpawned = Instantiate(vfxToSpawn, spawnLocation, Quaternion.Euler(-90, 0,0));
        StartCoroutine(VFXDestroyer(vfxSpawned));

        BehaviorGraphAgent behaviorGraph = enemySpawned.GetComponent<BehaviorGraphAgent>();

        if (behaviorGraph != null)
        {
            GameObject player = GameObject.Find("Player");
            behaviorGraph.BlackboardReference.SetVariableValue("Target (Player)", player);
        }

        if (isTut)
        {
            behaviorGraph.BlackboardReference.SetVariableValue("CanMove", false);
            Tutorial tut = GameObject.Find("Tutorial").GetComponent<Tutorial>();
            tut.enemySpawned.Add(enemySpawned);
            tut.nextTut = true;
            tut.currentTut = currentWave;
        }
        //spawnVFX = Instantiate(tempBruteSpawnerEffect, availableSpawns[0].position + Vector3.up * 2.5f, Quaternion.identity);

        //if (spawnVFX != null)
        //{
        //    StartCoroutine(VFXDestroyer(spawnVFX));
        //}

    }
}

[Serializable]
public struct WaveData
{
    public Transform waveSpawns;
    public int enemyCountTillSpawnNextWave;
    
    //public bool randomSpawns;
    //public int[] enemyTypeSpawnNumberForRandomOnly;
}
