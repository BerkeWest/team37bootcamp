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
        SetupButtonListeners();
        
        // Oyunu duraklat (You Win ekranında)
        Time.timeScale = 0f;
        
        // You Win animasyonunu başlat
        StartYouWinAnimation();
        
        // You Win sesini çal
        PlayYouWinSound();
    }
    
    void SetupButtonListeners()
    {
        Debug.Log("Setting up button listeners...");
        
        // Ana menüye dön butonu
        if (menuButton != null)
        {
            menuButton.onClick.AddListener(ReturnToMainMenu);
            Debug.Log("Menu button listener added successfully");
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
        }
        else
        {
            Debug.LogError("New Game button is null! Please assign it in the inspector.");
        }
    }
    
    public void ReturnToMainMenu()
    {
        Debug.Log("Returning to main menu from You Win");
        Time.timeScale = 1f; // Zamanı normale döndür
        SceneManager.LoadScene("MainMenu");
    }
    
    public void StartNewGame()
    {
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