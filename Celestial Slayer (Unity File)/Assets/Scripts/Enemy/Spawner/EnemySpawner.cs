using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.Behavior;
using static GizmoSpawnLocations;

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
        GameObject spawnedEnemy = null;
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
            spawnedEnemy = RandomSpawn(availableSpawns, spawnedEnemy);
        else
            spawnedEnemy = RegularSpawn(availableSpawns, spawnedEnemy, spawnType);

        

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
                spawnedEnemy = Instantiate(enemiesTypes[i], randomSpawnLocation, Quaternion.identity);
                Enemy enemyScrp = spawnedEnemy.GetComponent<Enemy>();
                enemyScrp.spawner = this;
                availableSpawns.RemoveAt(randomIndex);
                currentEnemyCount++;
            }
        }
        return spawnedEnemy;
    }

    private GameObject RegularSpawn(List<Vector3> availableSpawns, GameObject spawnedEnemy, List<int> enemyType)
    {
        Debug.Log("InRegularSpawn");
        for (int i = 0; i < waveData[currentWave].enemyTypeSpawnNumber.Length; i++)
        {
            while(true)
            {
                //Find Spawner, if not spawner break to the next enemy
                int spawnIndex = -1;
                for (int k = 0; k < enemyType.Count; k++)
                {
                    Debug.Log("InIndex");
                    if(enemyType[k] == i)
                    {
                        spawnIndex = k; 
                        enemyType.RemoveAt(k);
                        Debug.Log(spawnIndex);
                        break;               
                    }
                }
                if (spawnIndex == -1)
                    break;
                Debug.Log("stillinSpawn");
                Vector3 spawnLocation = availableSpawns[spawnIndex];
                spawnedEnemy = Instantiate(enemiesTypes[i], spawnLocation, Quaternion.identity);
                Enemy enemyScrp = spawnedEnemy.GetComponent<Enemy>();
                enemyScrp.spawner = this;
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
    public bool randomSpawns;
    public int[] enemyTypeSpawnNumber;
    public Transform waveSpawns;
}
