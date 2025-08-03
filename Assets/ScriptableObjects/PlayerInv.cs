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
        if (item)
        {
            inventory.AddItem(item.item, 1);
            AudioManager.Instance.Play("AddItem", true);

            Destroy(other.gameObject);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            inventory.Save();
        }
         
        if(Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            inventory.Load ();
        }
    }

    public void OnApplicationQuit()
    {
        inventory.Container.Clear();
    }


}
