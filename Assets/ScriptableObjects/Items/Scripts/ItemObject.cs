using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType // Bu kýsým item tiplerini (yemek, ekipman vb.) belirtir.
{
    Food,
    Equipment,
    Default
}
public abstract class ItemObject : ScriptableObject // Her item'ýn temel özelliklerini tanýmlayan yer.

{
    public GameObject prefab; // Bu item'ýn oyun içindeki görüntüsü (3D modeli veya UI prefab'i)
    public ItemType type; // Bu item'ýn ne tür bir item olduðunu gösterir (yemek mi, ekipman mý?).
    [TextArea(15, 20)] // Bu, Unity Editor'da açýklama yazmak için büyük bir kutu yapar.
    public string description; // Item'ýn açýklamasý.
    public int salePrice; // <<< YENÝ EKLENEN SATIR: Bu item'ýn satýþ fiyatý! <<<
}