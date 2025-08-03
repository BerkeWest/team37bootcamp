using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshProUGUI i�in gerekli
using UnityEngine.UI; // UI elemanlar� i�in (Image, Panel vb.) gerekli

public class DisplayInventory : MonoBehaviour
{
    // Bu, Unity'deki Envanter Ekran�n�z�n ana paneli olacak.
    // Unity'de bu script'i att���n�z yerde, bu bo�lu�u doldurman�z gerekecek.
    public GameObject inventoryPanel;
    public InventoryObject inventory; // Envanter verilerini tutan ScriptableObject

    public TextMeshProUGUI goldText; // <<< YEN� EKLENEN SATIR: Alt�n miktar�n� g�sterecek TextMeshPro objesi <<<

    public int X_START; // �lk item'�n X ba�lang�� pozisyonu
    public int Y_START; // �lk item'�n Y ba�lang�� pozisyonu
    public int X_SPACE_BETWEEN_ITEM; // Item'lar aras� X bo�lu�u
    public int NUMBER_OF_COLUMN; // Bir sat�rdaki item s�tun say�s�
    public int Y_SPACE_BETWEEN_ITEMS; // Item'lar aras� Y bo�lu�u (sat�rlar aras�)

    // Envanterdeki slotlar� ve bunlara kar��l�k gelen UI GameObject'lerini tutar
    Dictionary<InventorySlot, GameObject> itemsDisplayed = new Dictionary<InventorySlot, GameObject>();

    // Envanterin o an a��k olup olmad���n� tutan bayrak
    private bool isInventoryOpen = false;

    void Start()
    {
        // Ba�lang��ta envanter panelini gizle.
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false); // Paneli kapat (gizle)
        }

        // Oyun ba�lad���nda fare imlecini gizle ve hareketini k�s�tla.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Alt�n miktar�n� her oyun ba�lad���nda s�f�rla.
        if (inventory != null)
        {
            inventory.gold = 0; // <<< YEN� EKLENEN SATIR: Alt�n� s�f�rla! <<<
        }

        // Envanterin ba�lang��taki durumunu (varsa itemleri) ekrana yans�t.
        CreateDisplay();

        // Alt�n textini ba�lang��ta da g�ncelle (s�f�rlanm�� de�eri g�stersin).
        UpdateGoldText(); // <<< YEN� EKLENEN �A�RI <<<

        // DEBUG: Oyun ba�lad���nda konsola yaz
        Debug.Log("DisplayInventory script'i ba�lad�. Envanter kapal�.");
    }

    void Update()
    {
        // 'E' tu�una bas�ld���nda
        if (Input.GetKeyDown(KeyCode.E))
        {
            // DEBUG: 'E' tu�una bas�ld���n� konsola yaz
            Debug.Log("E tu�una bas�ld�!");
            ToggleInventory(); // Envanteri a�ma/kapama fonksiyonunu �a��r
        }

        // Sadece envanter a��ksa ve her karede UI'y� g�ncelle
        if (isInventoryOpen)
        {
            UpdateDisplay();
            // Alt�n textini envanter a��kken s�rekli g�ncelle (alt�n kazand�k�a de�i�imi g�r)
            UpdateGoldText(); // <<< YEN� EKLENEN �A�RI <<<
        }
    }

    // Envanter panelini a��p kapatan ana fonksiyon.
    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen; // isInventoryOpen de�erini tersine �evir

        // Envanter panelinin aktifli�ini (g�r�n�rl���n�) ayarla.
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(isInventoryOpen);
            Debug.Log("Envanter panelinin durumu de�i�ti: " + (isInventoryOpen ? "A��k" : "Kapal�")); // DEBUG mesaj�
        }
        else
        {
            // DEBUG: inventoryPanel'in atanmad���n� konsola yaz (�ok �nemli!)
            Debug.LogError("Hata: inventoryPanel atanmam��! L�tfen InventoryScreen objesini Inspector'dan s�r�kleyip b�rak�n.");
        }

        // Fare imlecinin davran���n� ayarla.
        if (isInventoryOpen)
        {
            Cursor.lockState = CursorLockMode.None; // Fareyi serbest b�rak (UI ile etkile�im i�in)
            Cursor.visible = true; // Fareyi g�r�n�r yap
            // DEBUG: Fare imleci durumunu konsola yaz
            Debug.Log("Fare imleci serbest ve g�r�n�r.");
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // Fareyi oyun penceresine kilitle
            Cursor.visible = false; // Fareyi gizle
            // DEBUG: Fare imleci durumunu konsola yaz
            Debug.Log("Fare imleci kilitli ve gizli.");
        }
    }

    // Envanterdeki item'lar� ilk kez ekranda g�sterir.
    public void CreateDisplay()
    {
        // �nceki g�sterilen item'lar� temizle (varsa)
        foreach (var obj in itemsDisplayed.Values)
        {
            Destroy(obj);
        }
        itemsDisplayed.Clear();

        // Envanterdeki her bir item i�in UI eleman�n� olu�tur.
        for (int i = 0; i < inventory.Container.Count; i++)
        {
            InventorySlot currentSlot = inventory.Container[i];

            // Item'�n prefab'inden yeni bir UI objesi olu�tur.
            // �NEML� DE����KL�K BURADA: 'transform' yerine 'inventoryPanel.transform' kullan�yoruz.
            var obj = Instantiate(currentSlot.item.prefab, Vector3.zero, Quaternion.identity, inventoryPanel.transform);

            // Bu sat�r, UI olaylar�n� (mouse etkile�imi gibi) alg�lamas�n� sa�lar.
            obj.AddComponent<CanvasGroup>().blocksRaycasts = true;

            // Item'�n ekrandaki pozisyonunu ayarla.
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);

            // Item'�n miktar�n� g�steren TextMeshPro yaz�s�n� g�ncelle.
            // Emin olun ki item prefab'inizin i�inde bir TextMeshProUGUI bile�eni var.
            TextMeshProUGUI amountText = obj.GetComponentInChildren<TextMeshProUGUI>();
            if (amountText != null)
            {
                amountText.text = currentSlot.amount.ToString("n0"); // "n0" format�, binlik ay�r�c� olmadan say� g�sterir.
            }
            else
            {
                Debug.LogWarning($"Uyar�: {currentSlot.item.name} prefab'inde TextMeshProUGUI bulunamad�!");
            }

            // G�sterilen item'lar listesine ekle.
            itemsDisplayed.Add(currentSlot, obj);
        }
    }

    // Envanterdeki item'lar� g�nceller (miktar de�i�ince, yeni item eklenince vb.)
    public void UpdateDisplay()
    {
        // UI'dan kald�r�lmas� gereken item'lar� bul
        List<InventorySlot> slotsToRemove = new List<InventorySlot>();
        foreach (var entry in itemsDisplayed)
        {
            // E�er bu slot art�k envanterde yoksa veya miktar� 0 ise (iste�e ba�l�)
            if (!inventory.Container.Contains(entry.Key) || entry.Key.amount <= 0)
            {
                slotsToRemove.Add(entry.Key);
            }
        }

        // Bulunan item'lar� kald�r
        foreach (var slot in slotsToRemove)
        {
            if (itemsDisplayed.ContainsKey(slot) && itemsDisplayed[slot] != null) // Zaten silinmi� olmamas� i�in kontrol
            {
                Destroy(itemsDisplayed[slot]); // GameObject'i sahneden sil
            }
            itemsDisplayed.Remove(slot); // Dictionary'den kald�r
        }

        // Envanterdeki her bir item i�in UI'y� g�ncelle veya yeniden olu�tur
        for (int i = 0; i < inventory.Container.Count; i++)
        {
            InventorySlot currentSlot = inventory.Container[i];

            if (itemsDisplayed.ContainsKey(currentSlot)) // E�er item zaten ekranda g�steriliyorsa
            {
                // Miktar�n� g�ncelle
                TextMeshProUGUI amountText = itemsDisplayed[currentSlot].GetComponentInChildren<TextMeshProUGUI>();
                if (amountText != null)
                {
                    amountText.text = currentSlot.amount.ToString("n0");
                }
                // Pozisyonunu da g�ncelle, ��nk� s�ra de�i�mi� olabilir.
                itemsDisplayed[currentSlot].GetComponent<RectTransform>().localPosition = GetPosition(i);
            }
            else // Item yeni eklendiyse veya daha �nce g�sterilmediyse
            {
                // Yeni UI objesi olu�tur
                // �NEML� DE����KL�K BURADA: 'transform' yerine 'inventoryPanel.transform' kullan�yoruz.
                var obj = Instantiate(currentSlot.item.prefab, Vector3.zero, Quaternion.identity, inventoryPanel.transform);
                obj.AddComponent<CanvasGroup>().blocksRaycasts = true;
                obj.GetComponent<RectTransform>().localPosition = GetPosition(i);

                TextMeshProUGUI amountText = obj.GetComponentInChildren<TextMeshProUGUI>();
                if (amountText != null)
                {
                    amountText.text = currentSlot.amount.ToString("n0");
                }
                itemsDisplayed.Add(currentSlot, obj);
            }
        }
    }

    // <<< YEN� EKLENEN FONKS�YON: Alt�n textini g�ncellemek i�in! <<<
    public void UpdateGoldText()
    {
        if (goldText != null && inventory != null)
        {
            goldText.text = inventory.gold.ToString(); // InventoryObject'teki 'gold' de�erini stringe �evirip text'e ata
        }
        else if (goldText == null)
        {
            Debug.LogWarning("Uyar�: goldText objesi DisplayInventory script'ine atanmam��!");
        }
        else if (inventory == null)
        {
            Debug.LogError("Hata: InventoryObject DisplayInventory script'ine atanmam��!");
        }
    }


    // Item'�n envanterdeki pozisyonunu hesaplar (�zgara mant���)
    public Vector3 GetPosition(int i)
    {
        // Form�l: Ba�lang�� X + (Item'�n s�tun numaras� * S�tunlar aras� bo�luk)
        // Ba�lang�� Y + (Item'�n sat�r numaras� * Sat�rlar aras� bo�luk)
        // S�f�ra b�lme hatas�n� �nlemek i�in NUMBER_OF_COLUMN kontrol�
        if (NUMBER_OF_COLUMN == 0)
        {
            Debug.LogError("Hata: NUMBER_OF_COLUMN 0 olamaz! L�tfen DisplayInventory script'inde do�ru bir de�er girin.");
            return Vector3.zero; // Hata durumunda varsay�lan bir de�er d�nd�r
        }

        return new Vector3(X_START + (X_SPACE_BETWEEN_ITEM * (i % NUMBER_OF_COLUMN)),
                           Y_START + (-Y_SPACE_BETWEEN_ITEMS * (i / NUMBER_OF_COLUMN)),
                           0f);
    }
}