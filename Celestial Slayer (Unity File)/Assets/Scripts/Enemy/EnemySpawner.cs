using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.VisualScripting;

public class EnemySpawner : MonoBehaviour
{
    private List<Vector3> spawnLocations = new List<Vector3>();
    [SerializeField] private GameObject[] enemiesTypes;
    [SerializeField] private WaveData[] waveData;
    private int totalWaves;
    private int currentWave;
    public int currentEnemyCount;

    [Header("DeveloperTools")]
    public bool spawnEnemies;
    [SerializeField] private bool randomEnemySpawns;
    [SerializeField] private GameObject combatEventObj;
    private ICombatEvent combatEvent;

    
    void Start()
    {
        combatEvent = combatEventObj.GetComponent<ICombatEvent>();
        //Populate SpawnLocation Array
        Transform spawnLocationsParent = transform.Find("SpawnLocations");
        int spawnLocationsCount = spawnLocationsParent.childCount;
        for (int i = 0; i < spawnLocationsCount; i++)
        {
            spawnLocations.Add(spawnLocationsParent.GetChild(i).position);
        }
        totalWaves = waveData.Length;
    }

    void Update()
    {
        if (currentWave == totalWaves && currentEnemyCount == 0)
        {
            if(combatEvent != null) 
                combatEvent.PostCombatEvent();
            Destroy(gameObject);
            PlayerController.inCombat = false;

        }
        else if (spawnEnemies && currentEnemyCount == 0)
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
        GameObject spawnedEnemy = null;
        List<Vector3> availableSpawns = spawnLocations;

        if (randomEnemySpawns)
            spawnedEnemy = RandomSpawn(availableSpawns, spawnedEnemy);
        else
            spawnedEnemy = RegularSpawn(availableSpawns, spawnedEnemy);

        

        BehaviorGraphAgent behaviorGraph = spawnedEnemy.GetComponent<BehaviorGraphAgent>();
        GameObject player = GameObject.Find("Player");

        behaviorGraph.BlackboardReference.SetVariableValue("Target (Player)", player);
    }

    private GameObject RandomSpawn(List<Vector3> availableSpawns, GameObject spawnedEnemy)
    {
        for (int i = 0; i < waveData[currentWave].enemyTypeSpawnNumber.Length; i++)
        {
            for (int j = 0; j < waveData[currentWave].enemyTypeSpawnNumber[i]; j++)
            {
                //Get random location from set locations
                int randomIndex = UnityEngine.Random.Range(0, (spawnLocations.Count - 1));
                Vector3 randomSpawnLocation = availableSpawns[randomIndex];

                //SpawnEnemy at random Location and set its parent as the spawner
                spawnedEnemy = Instantiate(enemiesTypes[i], randomSpawnLocation, Quaternion.identity, transform);
                Enemy enemyScrp = spawnedEnemy.GetComponent<Enemy>();
                enemyScrp.spawner = this.GetComponent<EnemySpawner>();
                availableSpawns.RemoveAt(randomIndex);
                currentEnemyCount++;
            }
        }
        return spawnedEnemy;
    }

    private GameObject RegularSpawn(List<Vector3> availableSpawns, GameObject spawnedEnemy)
    {
        for (int i = 0; i < waveData[currentWave].enemyTypeSpawnNumber.Length; i++)
        {
            for (int j = 0; j < waveData[currentWave].enemyTypeSpawnNumber[i]; j++)
            {
                //SpawnEnemy at first Location and set its parent as the spawner remove spawn
                Vector3 SpawnLocation = availableSpawns[0];
                spawnedEnemy = Instantiate(enemiesTypes[i], SpawnLocation, Quaternion.identity);
                Enemy enemyScrp = spawnedEnemy.GetComponent<Enemy>();
                enemyScrp.spawner = this.GetComponent<EnemySpawner>();
                availableSpawns.RemoveAt(0);
                currentEnemyCount++;
            }
        }
        return spawnedEnemy;
    }
}

[Serializable]
public struct WaveData
{
    public int[] enemyTypeSpawnNumber;
}
