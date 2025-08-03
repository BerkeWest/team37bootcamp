using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoad1 : MonoBehaviour
{
    [SerializeField] private Transform dungeonSpawnPoint;
    [SerializeField] private Transform labyrinthSpawnPoint;
    
    public GameObject enemyPrefab;
    public int dungeonEnemyCount = 12;
    [SerializeField] private Transform[] dungeonEnemySpawnPoints;
    public int labyrinthEnemyCount = 12;
    [SerializeField] private Transform[] labyrinthEnemySpawnPoints; 
    
    private void OnTriggerEnter(Collider other)
    {
        PlayerControllerS player = other.GetComponent<PlayerControllerS>();
        if (other.CompareTag("Player"))
        {
            int destination = Random.Range(0, 2);

            if (destination == 0)
            {
                player.GoToPosition(dungeonSpawnPoint);
                Debug.Log("Teleporting to Dungeon");
                SpawnEnemies(dungeonEnemySpawnPoints, dungeonEnemyCount);
            }
            else
            {
                player.GoToPosition(labyrinthSpawnPoint);
                Debug.Log("Teleporting to Labyrinth");
                SpawnEnemies(labyrinthEnemySpawnPoints, labyrinthEnemyCount);
            }
        }
    }
    
    private void SpawnEnemies(Transform[] enemySpawnPoints, int enemyCount)
    {
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < enemySpawnPoints.Length; i++)
        {
            availableIndices.Add(i);
        }
        
        for (int i = 0; i < enemyCount && availableIndices.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availableIndices.Count);
            int spawnIndex = availableIndices[randomIndex];
            availableIndices.RemoveAt(randomIndex);
            
            GameObject newEnemy = Instantiate(enemyPrefab, enemySpawnPoints[spawnIndex].position, enemySpawnPoints[spawnIndex].rotation);
        }
    }
}