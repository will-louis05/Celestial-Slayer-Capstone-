using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.Behavior;

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
    [SerializeField] private GameObject combatEventObj;

    
    void Start()
    {

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
            ICombatEvent combatEvent = combatEventObj.GetComponent<ICombatEvent>();
            if (combatEvent != null) 
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
        List<Vector3> availableSpawns = spawnLocations;
        List <int> spawnType = new List<int>();
        if (waveData[currentWave].waveSpawns != null)
        {
            int spawnLocationsCount = waveData[currentWave].waveSpawns.childCount;
            for (int i = 0; i < spawnLocationsCount; i++)
            {
                availableSpawns.Add(waveData[currentWave].waveSpawns.GetChild(i).position);
                spawnType.Add(waveData[currentWave].waveSpawns.GetChild(i).GetComponent<GizmoSpawnLocations>().enemyTypeInt);
            }
        }
        bool randomEnemySpawns = waveData[currentWave].randomSpawns;
        if (randomEnemySpawns)
            RandomSpawn(availableSpawns);
        else
            RegularSpawn(availableSpawns, spawnType);
    }

    private void RandomSpawn(List<Vector3> availableSpawns)
    {
        for (int i = 0; i < waveData[currentWave].enemyTypeSpawnNumber.Length; i++)
        {
            for (int j = 0; j < waveData[currentWave].enemyTypeSpawnNumber[i]; j++)
            {
                //Get random location from set locations
                int randomIndex = UnityEngine.Random.Range(0, (spawnLocations.Count - 1));
                Vector3 randomSpawnLocation = availableSpawns[randomIndex];

                //SpawnEnemy at random Location and set its parent as the spawner
                GameObject spawnedEnemy = Instantiate(enemiesTypes[i], randomSpawnLocation, Quaternion.identity);
                Enemy enemyScrp = spawnedEnemy.GetComponent<Enemy>();
                enemyScrp.spawner = this;
                availableSpawns.RemoveAt(randomIndex);
                currentEnemyCount++;
            }
            Debug.Log("eType = " + enemiesTypes[i]);
            BehaviorGraphAgent behaviorGraph = enemiesTypes[i].GetComponent<BehaviorGraphAgent>();

            if (behaviorGraph != null)
            {
                GameObject player = GameObject.Find("Player");
                behaviorGraph.BlackboardReference.SetVariableValue("Target (Player)", player);
            }
        }
    }

    private void RegularSpawn(List<Vector3> availableSpawns, List<int> enemyType)
    {
        for (int i = 0; i < waveData[currentWave].enemyTypeSpawnNumber.Length; i++)
        {
            while(true)
            {
                //Find Spawner, if not spawner break to the next enemy
                int spawnIndex = -1;
                for (int k = 0; k < enemyType.Count; k++)
                {
                    if(enemyType[k] == i)
                    {
                        spawnIndex = k; 
                        enemyType.RemoveAt(k);
                        break;               
                    }
                }
                if (spawnIndex == -1)
                    break;
                Vector3 spawnLocation = availableSpawns[spawnIndex];
                GameObject spawnedEnemy = Instantiate(enemiesTypes[i], spawnLocation, Quaternion.identity);
                Enemy enemyScrp = spawnedEnemy.GetComponent<Enemy>();
                enemyScrp.spawner = this;
                availableSpawns.RemoveAt(0);
                currentEnemyCount++;
            }

            BehaviorGraphAgent behaviorGraph = enemiesTypes[i].GetComponent<BehaviorGraphAgent>();
            if(behaviorGraph != null)
            {
                GameObject player = GameObject.Find("Player");
                behaviorGraph.BlackboardReference.SetVariableValue("Target (Player)", player);
            }
        }
    }
}

[Serializable]
public struct WaveData
{
    public bool randomSpawns;
    public int[] enemyTypeSpawnNumber;
    public Transform waveSpawns;
}
