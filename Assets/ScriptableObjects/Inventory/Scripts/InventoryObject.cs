using System.Collections;
using System.Collections.Generic;
// using System.IO.Enumeration; // Bu satýrý Unity'de hata veriyorsa silebilirsiniz, genellikle gerekli deðildir.
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject, ISerializationCallbackReceiver
{
    public string savePath;
    public ItemDatabaseObject database;
    public List<InventorySlot> Container = new List<InventorySlot>();

    public long gold; // <<< YENÝ EKLENEN SATIR: Oyuncunun altýn miktarý burda tutulacak! <<<

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

    // <<< YENÝ EKLENEN FONKSÝYON: Altýn eklemek için! <<<
    public void AddGold(long amount)
    {
        gold += amount; // Gelen miktarý mevcut altýna ekle
        Debug.Log($"Altýn eklendi: {amount}. Toplam altýn: {gold}"); // Konsola bilgi yaz
        // Eðer oyun içinde altýný gösteren bir UI text'iniz varsa, burada onu güncelleyebilirsiniz.
        // Örneðin: if (goldTextUI != null) goldTextUI.text = gold.ToString();
    }

    // <<< YENÝ EKLENEN FONKSÝYON: Item silmek için! <<<
    // Bu fonksiyon SellManager'da kullanýlýyor. Eðer sizde zaten varsa, mevcut olaný kullanýn.
    // Yoksa, bu fonksiyonu eklemeniz gerekiyor.
    public void RemoveItem(ItemObject _item, int _amount)
    {
        for (int i = 0; i < Container.Count; i++)
        {
            if (Container[i].item == _item)
            {
                Container[i].amount -= _amount;
                if (Container[i].amount <= 0)
                {
                    Container.RemoveAt(i); // Eðer miktar 0 veya altýna düþerse slotu tamamen sil
                }
                return;
            }
        }
    }


    public void Save()
    {
        string saveData = JsonUtility.ToJson(this, true);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
        bf.Serialize(file, saveData);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open);
            JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
            file.Close();
        }
    }

    public void OnAfterDeserialize()
    {
        for (int i = 0; i < Container.Count; i++)
        {
            Container[i].item = database.GetItem[Container[i].ID];
        }
    }

    public void OnBeforeSerialize()
    {
        // Genellikle buraya bir þey yazmaya gerek kalmaz, ama interface gereði boþ býrakýlmaz.
    }
}

[System.Serializable] // Unity'de Inspector'da görünmesi için gerekli
public class InventorySlot
{
    public int ID;
    public ItemObject item; // Envanter slotunda saklanan genel obje
    public int amount; // Envanter slotunda saklanan objelerin toplam miktarý

    public InventorySlot(int _id, ItemObject _item, int _amount)
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