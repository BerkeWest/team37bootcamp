using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshProUGUI için gerekli
using UnityEngine.UI; // UI elemanlarý için (Image, Panel vb.) gerekli

public class DisplayInventory : MonoBehaviour
{
    // Bu, Unity'deki Envanter Ekranýnýzýn ana paneli olacak.
    // Unity'de bu script'i attýðýnýz yerde, bu boþluðu doldurmanýz gerekecek.
    public GameObject inventoryPanel;
    public InventoryObject inventory; // Envanter verilerini tutan ScriptableObject

    public TextMeshProUGUI goldText; // <<< YENÝ EKLENEN SATIR: Altýn miktarýný gösterecek TextMeshPro objesi <<<

    public int X_START; // Ýlk item'ýn X baþlangýç pozisyonu
    public int Y_START; // Ýlk item'ýn Y baþlangýç pozisyonu
    public int X_SPACE_BETWEEN_ITEM; // Item'lar arasý X boþluðu
    public int NUMBER_OF_COLUMN; // Bir satýrdaki item sütun sayýsý
    public int Y_SPACE_BETWEEN_ITEMS; // Item'lar arasý Y boþluðu (satýrlar arasý)

    // Envanterdeki slotlarý ve bunlara karþýlýk gelen UI GameObject'lerini tutar
    Dictionary<InventorySlot, GameObject> itemsDisplayed = new Dictionary<InventorySlot, GameObject>();

    // Envanterin o an açýk olup olmadýðýný tutan bayrak
    private bool isInventoryOpen = false;

    void Start()
    {
        // Baþlangýçta envanter panelini gizle.
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false); // Paneli kapat (gizle)
        }

        // Oyun baþladýðýnda fare imlecini gizle ve hareketini kýsýtla.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Altýn miktarýný her oyun baþladýðýnda sýfýrla.
        if (inventory != null)
        {
            inventory.gold = 0; // <<< YENÝ EKLENEN SATIR: Altýný sýfýrla! <<<
        }

        // Envanterin baþlangýçtaki durumunu (varsa itemleri) ekrana yansýt.
        CreateDisplay();

        // Altýn textini baþlangýçta da güncelle (sýfýrlanmýþ deðeri göstersin).
        UpdateGoldText(); // <<< YENÝ EKLENEN ÇAÐRI <<<

        // DEBUG: Oyun baþladýðýnda konsola yaz
        Debug.Log("DisplayInventory script'i baþladý. Envanter kapalý.");
    }

    void Update()
    {
        // 'E' tuþuna basýldýðýnda
        if (Input.GetKeyDown(KeyCode.E))
        {
            // DEBUG: 'E' tuþuna basýldýðýný konsola yaz
            Debug.Log("E tuþuna basýldý!");
            ToggleInventory(); // Envanteri açma/kapama fonksiyonunu çaðýr
        }

        // Sadece envanter açýksa ve her karede UI'yý güncelle
        if (isInventoryOpen)
        {
            UpdateDisplay();
            // Altýn textini envanter açýkken sürekli güncelle (altýn kazandýkça deðiþimi gör)
            UpdateGoldText(); // <<< YENÝ EKLENEN ÇAÐRI <<<
        }
    }

    // Envanter panelini açýp kapatan ana fonksiyon.
    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen; // isInventoryOpen deðerini tersine çevir

        // Envanter panelinin aktifliðini (görünürlüðünü) ayarla.
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(isInventoryOpen);
            Debug.Log("Envanter panelinin durumu deðiþti: " + (isInventoryOpen ? "Açýk" : "Kapalý")); // DEBUG mesajý
        }
        else
        {
            // DEBUG: inventoryPanel'in atanmadýðýný konsola yaz (çok önemli!)
            Debug.LogError("Hata: inventoryPanel atanmamýþ! Lütfen InventoryScreen objesini Inspector'dan sürükleyip býrakýn.");
        }

        // Fare imlecinin davranýþýný ayarla.
        if (isInventoryOpen)
        {
            Cursor.lockState = CursorLockMode.None; // Fareyi serbest býrak (UI ile etkileþim için)
            Cursor.visible = true; // Fareyi görünür yap
            // DEBUG: Fare imleci durumunu konsola yaz
            Debug.Log("Fare imleci serbest ve görünür.");
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // Fareyi oyun penceresine kilitle
            Cursor.visible = false; // Fareyi gizle
            // DEBUG: Fare imleci durumunu konsola yaz
            Debug.Log("Fare imleci kilitli ve gizli.");
        }
    }

    // Envanterdeki item'larý ilk kez ekranda gösterir.
    public void CreateDisplay()
    {
        // Önceki gösterilen item'larý temizle (varsa)
        foreach (var obj in itemsDisplayed.Values)
        {
            Destroy(obj);
        }
        itemsDisplayed.Clear();

        // Envanterdeki her bir item için UI elemanýný oluþtur.
        for (int i = 0; i < inventory.Container.Count; i++)
        {
            InventorySlot currentSlot = inventory.Container[i];

            // Item'ýn prefab'inden yeni bir UI objesi oluþtur.
            // ÖNEMLÝ DEÐÝÞÝKLÝK BURADA: 'transform' yerine 'inventoryPanel.transform' kullanýyoruz.
            var obj = Instantiate(currentSlot.item.prefab, Vector3.zero, Quaternion.identity, inventoryPanel.transform);

            // Bu satýr, UI olaylarýný (mouse etkileþimi gibi) algýlamasýný saðlar.
            obj.AddComponent<CanvasGroup>().blocksRaycasts = true;

            // Item'ýn ekrandaki pozisyonunu ayarla.
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);

            // Item'ýn miktarýný gösteren TextMeshPro yazýsýný güncelle.
            // Emin olun ki item prefab'inizin içinde bir TextMeshProUGUI bileþeni var.
            TextMeshProUGUI amountText = obj.GetComponentInChildren<TextMeshProUGUI>();
            if (amountText != null)
            {
                amountText.text = currentSlot.amount.ToString("n0"); // "n0" formatý, binlik ayýrýcý olmadan sayý gösterir.
            }
            else
            {
                Debug.LogWarning($"Uyarý: {currentSlot.item.name} prefab'inde TextMeshProUGUI bulunamadý!");
            }

            // Gösterilen item'lar listesine ekle.
            itemsDisplayed.Add(currentSlot, obj);
        }
    }

    // Envanterdeki item'larý günceller (miktar deðiþince, yeni item eklenince vb.)
    public void UpdateDisplay()
    {
        // UI'dan kaldýrýlmasý gereken item'larý bul
        List<InventorySlot> slotsToRemove = new List<InventorySlot>();
        foreach (var entry in itemsDisplayed)
        {
            // Eðer bu slot artýk envanterde yoksa veya miktarý 0 ise (isteðe baðlý)
            if (!inventory.Container.Contains(entry.Key) || entry.Key.amount <= 0)
            {
                slotsToRemove.Add(entry.Key);
            }
        }

        // Bulunan item'larý kaldýr
        foreach (var slot in slotsToRemove)
        {
            if (itemsDisplayed.ContainsKey(slot) && itemsDisplayed[slot] != null) // Zaten silinmiþ olmamasý için kontrol
            {
                Destroy(itemsDisplayed[slot]); // GameObject'i sahneden sil
            }
            itemsDisplayed.Remove(slot); // Dictionary'den kaldýr
        }

        // Envanterdeki her bir item için UI'yý güncelle veya yeniden oluþtur
        for (int i = 0; i < inventory.Container.Count; i++)
        {
            InventorySlot currentSlot = inventory.Container[i];

            if (itemsDisplayed.ContainsKey(currentSlot)) // Eðer item zaten ekranda gösteriliyorsa
            {
                // Miktarýný güncelle
                TextMeshProUGUI amountText = itemsDisplayed[currentSlot].GetComponentInChildren<TextMeshProUGUI>();
                if (amountText != null)
                {
                    amountText.text = currentSlot.amount.ToString("n0");
                }
                // Pozisyonunu da güncelle, çünkü sýra deðiþmiþ olabilir.
                itemsDisplayed[currentSlot].GetComponent<RectTransform>().localPosition = GetPosition(i);
            }
            else // Item yeni eklendiyse veya daha önce gösterilmediyse
            {
                // Yeni UI objesi oluþtur
                // ÖNEMLÝ DEÐÝÞÝKLÝK BURADA: 'transform' yerine 'inventoryPanel.transform' kullanýyoruz.
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

    // <<< YENÝ EKLENEN FONKSÝYON: Altýn textini güncellemek için! <<<
    public void UpdateGoldText()
    {
        if (goldText != null && inventory != null)
        {
            goldText.text = inventory.gold.ToString(); // InventoryObject'teki 'gold' deðerini stringe çevirip text'e ata
        }
        else if (goldText == null)
        {
            Debug.LogWarning("Uyarý: goldText objesi DisplayInventory script'ine atanmamýþ!");
        }
        else if (inventory == null)
        {
            Debug.LogError("Hata: InventoryObject DisplayInventory script'ine atanmamýþ!");
        }
    }


    // Item'ýn envanterdeki pozisyonunu hesaplar (ýzgara mantýðý)
    public Vector3 GetPosition(int i)
    {
        // Formül: Baþlangýç X + (Item'ýn sütun numarasý * Sütunlar arasý boþluk)
        // Baþlangýç Y + (Item'ýn satýr numarasý * Satýrlar arasý boþluk)
        // Sýfýra bölme hatasýný önlemek için NUMBER_OF_COLUMN kontrolü
        if (NUMBER_OF_COLUMN == 0)
        {
            Debug.LogError("Hata: NUMBER_OF_COLUMN 0 olamaz! Lütfen DisplayInventory script'inde doðru bir deðer girin.");
            return Vector3.zero; // Hata durumunda varsayýlan bir deðer döndür
        }

        return new Vector3(X_START + (X_SPACE_BETWEEN_ITEM * (i % NUMBER_OF_COLUMN)),
                           Y_START + (-Y_SPACE_BETWEEN_ITEMS * (i / NUMBER_OF_COLUMN)),
                           0f);
    }
}