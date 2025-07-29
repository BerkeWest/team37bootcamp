using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoad : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = spawnPoint.position;
            other.transform.rotation = spawnPoint.rotation;
        }
    }
}