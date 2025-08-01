using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro kullanmak için bu satýr gerekli
using UnityEngine.UI; // UI elemanlarý için (Image, Panel vb.) bu satýr gerekli

public class DisplayInventory : MonoBehaviour
{
    private InputManager inputManager;
    public GameObject inventoryPanel;
    private bool isInventoryOpen = false;

    void Start()
    {
        inputManager = InputManager.Instance;

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // DEBUG: Oyun baþladýðýnda konsola yaz
        Debug.Log("DisplayInventory script'i baþladý. Envanter kapalý.");
    }


    void Update()
    {
        // 'E' tuþuna basýldýðýnda
        if (inputManager.GetInventoryInput())
        {
            // DEBUG: 'E' tuþuna basýldýðýný konsola yaz
            Debug.Log("E tuþuna basýldý!");
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(isInventoryOpen);
            // DEBUG: Envanter panelinin yeni durumunu konsola yaz
            Debug.Log("Envanter panelinin durumu deðiþti: " + (isInventoryOpen ? "Açýk" : "Kapalý"));
        }
        else
        {
            // DEBUG: inventoryPanel'in atanmadýðýný konsola yaz (çok önemli!)
            Debug.LogError("Hata: inventoryPanel atanmamýþ! Lütfen InventoryScreen objesini Inspector'dan sürükleyip býrakýn.");
        }

        if (isInventoryOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            // DEBUG: Fare imleci durumunu konsola yaz
            Debug.Log("Fare imleci serbest ve görünür.");
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            // DEBUG: Fare imleci durumunu konsola yaz
            Debug.Log("Fare imleci kilitli ve gizli.");
        }
    }
}