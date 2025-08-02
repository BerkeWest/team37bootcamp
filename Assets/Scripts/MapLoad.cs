using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoad : MonoBehaviour
{
    [SerializeField] private Transform lobbySpawnPoint;
    [SerializeField] private Transform dungeonSpawnPoint;
    [SerializeField] private Transform labyrinthSpawnPoint;
    
    private bool inLobby = true;
    
    public GameObject enemyPrefab;
    public int dungeonEnemyCount = 12;
    public Transform[] dungeonEnemySpawnPoints;
    public int labyrinthEnemyCount = 12;
    public Transform[] labyrinthEnemySpawnPoints; 
    
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    
    private void OnTriggerEnter(Collider other)
    {
        PlayerControllerS player = other.GetComponent<PlayerControllerS>();

        if (inLobby)
        {
            int destination = Random.Range(0, 2);

            if (destination == 0)
            {
                player.GoToPosition(dungeonSpawnPoint);
                SpawnEnemies(dungeonEnemySpawnPoints, dungeonEnemyCount);
            }
            else
            {
                player.GoToPosition(labyrinthSpawnPoint);
                SpawnEnemies(labyrinthEnemySpawnPoints, labyrinthEnemyCount);
            }
            inLobby = false;
        }
        else
        {
            player.GoToPosition(lobbySpawnPoint);
            ClearEnemies();
            inLobby = true;
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

            Instantiate(enemyPrefab, enemySpawnPoints[spawnIndex].position, enemySpawnPoints[spawnIndex].rotation);
        }
    }
    
    private void ClearEnemies()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        spawnedEnemies.Clear();
    }
}