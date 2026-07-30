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
    private bool hasSpawnedEnemy;
    public int currentEnemyCount;

    public bool spawnEnemies;

    
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
        if (spawnEnemies && currentEnemyCount == 0)
        {
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

        for (int i = 0; i < waveData[currentWave].enemyTypeSpawnNumber.Length; i++)
        {
            for(int j = 0; j < waveData[currentWave].enemyTypeSpawnNumber[i]; j++)
            {
                //Get random location from set locations
                int randomIndex = UnityEngine.Random.Range(0, (spawnLocations.Count - 1));
                Debug.Log(randomIndex + "EnemyCount" + currentEnemyCount);
                Vector3 randomSpawnLocation = availableSpawns[randomIndex];
                availableSpawns.RemoveAt(randomIndex);

                //SpawnEnemy at random Location and set its parent as the spawner
                GameObject spawnedEnemy = Instantiate(enemiesTypes[i], randomSpawnLocation, Quaternion.identity, transform);

                //if(!hasSpawnedEnemy)
                //{
                //    Debug.Log("FirstEnemy");
                //    BehaviorGraphAgent behaviorGraph = spawnedEnemy.GetComponent<BehaviorGraphAgent>();
                //    GameObject player = GameObject.Find("Player");
                //    behaviorGraph.SetVariableValue("Target (Player)", player);

                //    hasSpawnedEnemy = true;
                //}

                currentEnemyCount++;
            }
        }
    }
}

[Serializable]
public struct WaveData
{
    public int[] enemyTypeSpawnNumber;
}
