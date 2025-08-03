using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class YouWin : MonoBehaviour
{
    [Header("Buttons")]
    public Button menuButton; // Ana menüye dön butonu
    public Button newGameButton; // Yeni oyun butonu
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.None; // Fareyi serbest b�rak (UI ile etkile�im i�in)
        Cursor.visible = true; // Fareyi g�r�n�r yap
        Debug.Log("YouWin script started");
        
        // Event System kontrolü
        CheckEventSystem();
        
        SetupButtonListeners();
        
        // Oyunu duraklat (You Win ekranında)
        Time.timeScale = 0f;
        
        // You Win animasyonunu başlat
        StartYouWinAnimation();
        
        // You Win sesini çal
        PlayYouWinSound();
    }
    
    void CheckEventSystem()
    {
        // Event System var mı kontrol et
        UnityEngine.EventSystems.EventSystem eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("Event System not found! Creating one...");
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        else
        {
            Debug.Log("Event System found: " + eventSystem.name);
        }
        
        // Canvas kontrolü
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas not found! UI buttons won't work without a Canvas.");
        }
        else
        {
            Debug.Log("Canvas found: " + canvas.name);
            
            // Graphic Raycaster kontrolü
            UnityEngine.UI.GraphicRaycaster raycaster = canvas.GetComponent<UnityEngine.UI.GraphicRaycaster>();
            if (raycaster == null)
            {
                Debug.LogError("Graphic Raycaster not found on Canvas! Adding one...");
                canvas.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }
            else
            {
                Debug.Log("Graphic Raycaster found on Canvas");
            }
        }
    }
    
    void SetupButtonListeners()
    {
        Debug.Log("Setting up button listeners...");
        
        // Ana menüye dön butonu
        if (menuButton != null)
        {
            menuButton.onClick.AddListener(ReturnToMainMenu);
            Debug.Log("Menu button listener added successfully");
            
            // Button durumunu kontrol et
            Debug.Log("Menu button interactable: " + menuButton.interactable);
            Debug.Log("Menu button enabled: " + menuButton.enabled);
        }
        else
        {
            Debug.LogError("Menu button is null! Please assign it in the inspector.");
        }
            
        // Yeni oyun butonu
        if (newGameButton != null)
        {
            newGameButton.onClick.AddListener(StartNewGame);
            Debug.Log("New Game button listener added successfully");
            
            // Button durumunu kontrol et
            Debug.Log("New Game button interactable: " + newGameButton.interactable);
            Debug.Log("New Game button enabled: " + newGameButton.enabled);
        }
        else
        {
            Debug.LogError("New Game button is null! Please assign it in the inspector.");
        }
    }
    
    public void ReturnToMainMenu()
    {
        Debug.Log("=== MENU BUTTON CLICKED ===");
        Debug.Log("Returning to main menu from You Win");
        Time.timeScale = 1f; // Zamanı normale döndür
        SceneManager.LoadScene("MainMenu");
    }
    
    public void StartNewGame()
    {
        Debug.Log("=== NEW GAME BUTTON CLICKED ===");
        Debug.Log("Starting new game from You Win");
        Time.timeScale = 1f; // Zamanı normale döndür
        SceneManager.LoadScene("Levels"); // İlk oyun sahnesine git
    }
    
    void StartYouWinAnimation()
    {
        // You Win animasyonu için gerekirse buraya kod eklenebilir
        Debug.Log("You Win animation started");
    }
    
    void PlayYouWinSound()
    {
        // You Win sesi için gerekirse buraya kod eklenebilir
        Debug.Log("You Win sound played");
    }
    
    // Test için Context Menu
    [ContextMenu("Test You Win")]
    void TestYouWin()
    {
        Debug.Log("You Win test triggered");
    }
} 