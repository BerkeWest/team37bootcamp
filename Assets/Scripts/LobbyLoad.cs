using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyLoad : MonoBehaviour
{
    [SerializeField] private Transform lobbySpawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerControllerS>().GoToPosition(lobbySpawnPoint);
            Debug.Log("Teleporting to Lobby");
        }
    }
}
