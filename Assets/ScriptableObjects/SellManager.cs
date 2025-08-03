using UnityEngine;
using UnityEngine.UI; // UI elemanlarý (Button, Panel) için gerekli
using TMPro; // TextMeshProUGUI için gerekli (Eðer altýn miktarýný gösterecek bir Text kullanacaksan)

public class SellManager : MonoBehaviour
{
    // === Unity Inspector'dan Baðlayacaðýmýz Yerler ===
    public GameObject sellButtonPanel; // Unity'de oluþturduðumuz "SellButtonPanel" objesi
    public Button sellAllButton; // Unity'de oluþturduðumuz "SellButtonPanel" içindeki "SellAllButton" objesi
    public InventoryObject playerInventory; // Oyuncunun envanteri (Project penceresindeki ScriptableObject)

    // Eðer oyununda oyuncunun altýn miktarýný gösteren bir Text varsa, onu buraya baðlayabilirsin.
    // Yoksa bu satýrý þimdilik kapalý býrakabilirsin (baþýndaki // iþaretlerini silme).
    // public TextMeshProUGUI playerGoldText; 

    // === Bu deðiþkenler script'in kendi içinde kullanýlýr ===
    private bool isInSellZone = false; // Oyuncunun satýþ bölgesinde olup olmadýðýný takip eder

    // Oyun baþladýðýnda bir kere çalýþýr
    void Start()
    {
        // Oyun baþladýðýnda satýþ panelini gizle (görünmez yap)
        if (sellButtonPanel != null)
        {
            sellButtonPanel.SetActive(false);
        }

        // "Hepsini Sat" butonuna týklanýnca ne olacaðýný söyleyen kýsým
        if (sellAllButton != null)
        {
            sellAllButton.onClick.AddListener(SellAllItems); // Butona basýldýðýnda SellAllItems fonksiyonunu çaðýr
        }
    }

    // Her oyun karesinde sürekli kontrol eder
    void Update()
    {
        // Eðer oyuncu satýþ bölgesindeyse (isInSellZone doðruysa) VE 'F' tuþuna basýldýysa
        if (isInSellZone && Input.GetKeyDown(KeyCode.F))
        {
            // Satýþ panelini göster/gizle (aktifse kapat, pasifse aç)
            if (sellButtonPanel != null)
            {
                sellButtonPanel.SetActive(!sellButtonPanel.activeSelf); // Panelin mevcut aktifliðini tersine çevir
                Debug.Log("F tuþuna basýldý! Satýþ paneli açýldý/kapandý."); // Konsola mesaj yaz
            }
        }
    }

    // Oyuncu, "Is Trigger" olarak iþaretlenmiþ bir nesneye (burada SellZone'a) girdiðinde çalýþýr
    void OnTriggerEnter(Collider other)
    {
        // Eðer giren obje "Player" etiketine sahipse (oyuncu olduðundan emin olmak için)
        if (other.CompareTag("SellZone"))
        {
            isInSellZone = true; // Oyuncu satýþ bölgesine girdi olarak iþaretle
            Debug.Log("Oyuncu satýþ bölgesine girdi."); // Konsola mesaj yaz
            // Ýsteðe baðlý: Burada ekranda "F'ye basarak satýþ yap" gibi bir ipucu gösterebiliriz.
        }
    }

    // Oyuncu, "Is Trigger" olarak iþaretlenmiþ bir nesneden (burada SellZone'dan) çýktýðýnda çalýþýr
    void OnTriggerExit(Collider other)
    {
        // Çýkan obje "Player" etiketine sahipse
        if (other.CompareTag("SellZone"))
        {
            isInSellZone = false; // Oyuncu satýþ bölgesinden çýktý olarak iþaretle
            Debug.Log("Oyuncu satýþ bölgesinden çýktý."); // Konsola mesaj yaz
            // Oyuncu bölgeden çýkýnca satýþ panelini gizle
            if (sellButtonPanel != null)
            {
                sellButtonPanel.SetActive(false);
            }
        }
    }

    // "Hepsini Sat" butonuna týklandýðýnda çalýþacak ana fonksiyon
    public void SellAllItems()
    {
        if (playerInventory == null) // Envanter atanmamýþsa hata ver ve dur
        {
            Debug.LogError("Hata: Player Inventory atanmamýþ! Lütfen Sell Manager script'indeki boþluðu doldurun.");
            return;
        }

        long totalGoldEarned = 0; // Kazanýlan toplam altýný tutacak deðiþken (uzun sayýlar için 'long' kullandýk)

        // Envanterdeki her bir item'ý tersten gezerek satýþ yap
        // Tersten gezmek önemli, çünkü itemleri sildikçe liste boyutu deðiþebilir.
        for (int i = playerInventory.Container.Count - 1; i >= 0; i--)
        {
            InventorySlot slot = playerInventory.Container[i]; // Mevcut item slotunu al
            if (slot.item != null) // Slot boþ deðilse (item varsa)
            {
                // Item'ýn satýþ deðerini ve miktarýný kullanarak kazanýlan altýný hesapla
                totalGoldEarned += (long)slot.item.salePrice * slot.amount;
                AudioManager.Instance.Play("SellItem", true);

                Debug.Log($"{slot.item.name} x {slot.amount} satýldý. Kazanýlan: {slot.item.salePrice * slot.amount} altýn."); // Konsola bilgi yaz
            }
            // Item'ý envanterden tamamen sil (InventoryObject'teki RemoveItem fonksiyonunu çaðýrýr)
            playerInventory.RemoveItem(slot.item, slot.amount);
        }

        // Toplam kazanýlan altýný oyuncunun envanterine ekle
        playerInventory.AddGold(totalGoldEarned); // Bu fonksiyonu bir sonraki adýmda InventoryObject'e ekleyeceðiz

        // Tüm itemler satýldýktan sonra satýþ panelini tekrar gizle (daha derli toplu durur)
        if (sellButtonPanel != null)
        {
            sellButtonPanel.SetActive(false);
        }

        Debug.Log($"Tüm itemler satýldý. Toplam kazanýlan altýn: {totalGoldEarned}"); // Konsola toplam altýný yaz
    }
}