using System.Collections;
using System.Collections.Generic;
// using System.IO.Enumeration; // Bu sat�r� Unity'de hata veriyorsa silebilirsiniz, genellikle gerekli de�ildir.
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject, ISerializationCallbackReceiver
{
    public string savePath;
    public ItemDatabaseObject database;
    public List<InventorySlot> Container = new List<InventorySlot>();

    public long gold; // <<< YEN� EKLENEN SATIR: Oyuncunun alt�n miktar� burda tutulacak! <<<

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

    // <<< YEN� EKLENEN FONKS�YON: Alt�n eklemek i�in! <<<
    public void AddGold(long amount)
    {
        gold += amount; // Gelen miktar� mevcut alt�na ekle
        Debug.Log($"Alt�n eklendi: {amount}. Toplam alt�n: {gold}"); // Konsola bilgi yaz
        // E�er oyun i�inde alt�n� g�steren bir UI text'iniz varsa, burada onu g�ncelleyebilirsiniz.
        // �rne�in: if (goldTextUI != null) goldTextUI.text = gold.ToString();
        if (gold >= menu.playerDebt)
        {
            SceneManager.LoadScene("YouWin");
        }
    }

    // <<< YEN� EKLENEN FONKS�YON: Item silmek i�in! <<<
    // Bu fonksiyon SellManager'da kullan�l�yor. E�er sizde zaten varsa, mevcut olan� kullan�n.
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
                    Container.RemoveAt(i); // E�er miktar 0 veya alt�na d��erse slotu tamamen sil
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
        // Genellikle buraya bir �ey yazmaya gerek kalmaz, ama interface gere�i bo� b�rak�lmaz.
    }
}

[System.Serializable] // Unity'de Inspector'da g�r�nmesi i�in gerekli
public class InventorySlot
{
    public int ID;
    public ItemObject item; // Envanter slotunda saklanan genel obje
    public int amount; // Envanter slotunda saklanan objelerin toplam miktar�

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