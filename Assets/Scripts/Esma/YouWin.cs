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
        // Ana menüye dön butonu
        if (menuButton != null)
            menuButton.onClick.AddListener(ReturnToMainMenu);
        else
            Debug.LogWarning("Menu button is null! Please assign it in the inspector.");
            
        // Yeni oyun butonu
        if (newGameButton != null)
            newGameButton.onClick.AddListener(StartNewGame);
        else
            Debug.LogWarning("New Game button is null! Please assign it in the inspector.");
    }
    
    void ReturnToMainMenu()
    {
        Debug.Log("Returning to main menu from You Win");
        Time.timeScale = 1f; // Zamanı normale döndür
        SceneManager.LoadScene("MainMenu");
    }
    
    void StartNewGame()
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