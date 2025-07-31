using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestOpen : MonoBehaviour
{
    [SerializeField] private GameObject chestlid;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerSword"))
        {
            chestlid.transform.Rotate(Vector3.left, 90f);
        }
    }
}
