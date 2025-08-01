using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoad : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    
    public GameObject enemyPrefab;             
    public Transform[] spawnPoints;           
    public int numberOfEnemiesToSpawn = 5;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerControllerS>().GoToPosition(spawnPoint);
            SpawnEnemies();
        }
    }
    
    private void SpawnEnemies()
    {
        System.Collections.Generic.List<int> availableIndices = new System.Collections.Generic.List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
            availableIndices.Add(i);
        
        for (int i = 0; i < numberOfEnemiesToSpawn && availableIndices.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availableIndices.Count);
            int spawnIndex = availableIndices[randomIndex];
            availableIndices.RemoveAt(randomIndex);

            Instantiate(enemyPrefab, spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation);
        }
    }
}