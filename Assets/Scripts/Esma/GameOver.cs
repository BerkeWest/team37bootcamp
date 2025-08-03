using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [Header("Game Over UI")]
    public Text gameOverText;
    
    [Header("Buttons")]
    public Button menuButton; // Ana menüye dön butonu
    public Button newGameButton; // Yeni oyun butonu
    
    [Header("Animation")]
    public GameObject gameOverAnimation; // Game over animasyonu için
    
    [Header("Audio")]
    public AudioSource gameOverSound;
    

    
    void Start()
    {
        Cursor.lockState = CursorLockMode.None; // Fareyi serbest b�rak (UI ile etkile�im i�in)
        Cursor.visible = true; // Fareyi g�r�n�r yap
        Debug.Log("GameOver script started");
        
        // Event System kontrolü
        CheckEventSystem();
        
        SetupGameOver();
        SetupButtonListeners();
        
        // Game over animasyonunu başlat
        StartGameOverAnimation();
        
        // Game over sesini çal
        PlayGameOverSound();
    }
    
    void Update()
    {
        // ESC tuşu GameOver sahnesinde devre dışı
        // Kullanıcı sadece butonlarla seçim yapabilir
    }
    
    void SetupGameOver()
    {
        // Game over sahnesi başlangıç ayarları
        Debug.Log("Game Over scene setup completed");
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
    
    // Bu fonksiyon kahraman öldüğünde çağrılacak
    public void ShowGameOver()
    {
        Debug.Log("Game Over triggered!");
        
        // Game over sahnesine geç
        SceneManager.LoadScene("GameOver");
    }
    
    void StartGameOverAnimation()
    {
        if (gameOverAnimation != null)
        {
            gameOverAnimation.SetActive(true);
            Animator anim = gameOverAnimation.GetComponent<Animator>();
            if (anim != null)
            {
                anim.Play("GameOverAnimation");
                Debug.Log("Game over animation started");
            }
        }
    }
    
    void PlayGameOverSound()
    {
        if (gameOverSound != null)
        {
            gameOverSound.Play();
            Debug.Log("Game over sound played");
        }
    }
    
    // Ana menüye dön
    public void ReturnToMainMenu()
    {
        Debug.Log("=== MENU BUTTON CLICKED ===");
        Debug.Log("Returning to main menu");
        
        // Zamanı normale döndür
        Time.timeScale = 1f;
        
        // Ana menüye geç
        SceneManager.LoadScene("MainMenu");
    }
    
    // Yeni oyun başlat
    public void StartNewGame()
    {
        Debug.Log("=== NEW GAME BUTTON CLICKED ===");
        Debug.Log("Starting new game");
        
        // Zamanı normale döndür
        Time.timeScale = 1f;
        
        // Oyun sahnesine geç (yeni oyun)
        SceneManager.LoadScene("Levels");
    }
    
    // Bu fonksiyon kahraman öldüğünde çağrılabilir
    public void OnPlayerDeath()
    {
        ShowGameOver();
    }
    
    // Test için (sadece geliştirme aşamasında kullan)
    [ContextMenu("Test Game Over")]
    void TestGameOver()
    {
        ShowGameOver();
    }
} 