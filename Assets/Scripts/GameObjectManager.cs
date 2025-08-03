using System.Collections.Generic;
using UnityEngine;

public class GameObjectManager : MonoBehaviour
{
    public static GameObjectManager Instance;

    public List<ChestOpen> allChests = new();
    public List<KeyCollection> allKeys = new();
    public List<TrapDoor> allTraps = new();
    public List<GameObject> spawnedEnemies = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ResetAll()
    {
        foreach (ChestOpen chest in allChests)
            chest.ResetChest();

        foreach (KeyCollection key in allKeys)
            key.ResetKeyAndDoor();

        foreach (TrapDoor trap in allTraps)
            trap.TrapDoorReset();

        foreach (GameObject enemy in spawnedEnemies)
            Destroy(enemy);

        spawnedEnemies.Clear();
    }
}