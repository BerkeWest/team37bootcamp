using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerInv : MonoBehaviour
{
    public InventoryObject inventory;


    public void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<ItemInv>();
        if (item != null)
        {
            inventory.AddItem(item.item, 1);
            Destroy(other.gameObject);
        }
    }
    public void OnApplicationQuit()
    {
        inventory.Container.Clear();
    }


}
