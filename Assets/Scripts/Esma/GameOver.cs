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
        Debug.Log("GameOver script started");
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
    
    void SetupButtonListeners()
    {
        // Ana menüye dön butonu
        if (menuButton != null)
            menuButton.onClick.AddListener(ReturnToMainMenu);
            
        // Yeni oyun butonu
        if (newGameButton != null)
            newGameButton.onClick.AddListener(StartNewGame);
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
        Debug.Log("Returning to main menu");
        
        // Zamanı normale döndür
        Time.timeScale = 1f;
        
        // Ana menüye geç
        SceneManager.LoadScene("menu");
    }
    
    // Yeni oyun başlat
    public void StartNewGame()
    {
        Debug.Log("Starting new game");
        
        // Zamanı normale döndür
        Time.timeScale = 1f;
        
        // Oyun sahnesine geç (yeni oyun)
        SceneManager.LoadScene("SampleScene");
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