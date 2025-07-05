using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment Object", menuName = "Inventory System/Items/Equipment")]

public class EquipmentObject : ItemObject

{
    public float atkBonus; // attack bonus
    public float defenceBonus; // defence bonus
    public void Awake()
    {
        type = ItemType.Equipment;
    }
}
