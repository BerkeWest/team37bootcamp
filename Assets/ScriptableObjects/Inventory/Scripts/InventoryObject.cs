using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")] 
public class InventoryObject : ScriptableObject, ISerializationCallbackReceiver


{
    public ItemDatabaseObject database;
    public List<InventorySlot> Container = new List<InventorySlot>();
    public void AddItem(ItemObject _item, int _amount)
    {
        
        for (int i = 0; i < Container.Count; i++)
        {
            if (Container[i].item == _item)
            {
                Container[i].AddAmount(_amount);
                return;
            }
        }
        Container.Add(new InventorySlot(database.GetId[_item], _item, _amount));
        
    }

    public void OnAfterDeserialize()
    {
        for (int i = 0;i < Container.Count;i++)
        {
            Container[i].item = database.GetItem[Container[i].ID];
        }
    }

    public void OnBeforeSerialize()
    {
        
    }
}

[System.Serializable] // when we add it to a system within Unity, it will be serialized
public class InventorySlot
{
    public int ID;
    public ItemObject item; // general object stored in the inventory item
    public int amount; // total quantity of general objects stored in the inventory item
    public InventorySlot(int _id,  ItemObject _item, int _amount)
    {
        ID = _id;
        item = _item;
        amount = _amount;
    }
    public void AddAmount(int value)
    {
        amount += value;
    }
}