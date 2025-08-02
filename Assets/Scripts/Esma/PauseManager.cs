using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Pause UI")]
    public GameObject pausePanel;
    
    [Header("Pause Buttons")]
    public Button menuButton; // Ana menüye dön butonu
    public Button quitButton; // Oyundan çıkış butonu
    
    private bool isPaused = false;
    
    void Start()
    {
        Debug.Log("PauseManager started");
        SetupPauseMenu();
        SetupButtonListeners();
    }
    
    void Update()
    {
        // ESC tuşu ile pause menüsünü aç/kapat
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    void SetupPauseMenu()
    {
        // Pause panelini başlangıçta gizle
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }
    
    void SetupButtonListeners()
    {
        // Ana menüye dön butonu
        if (menuButton != null)
            menuButton.onClick.AddListener(ReturnToMainMenu);
            
        // Oyundan çıkış butonu
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }
    
    void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    
    void PauseGame()
    {
        Debug.Log("Game Paused");
        
        // Pause panelini göster
        if (pausePanel != null)
            pausePanel.SetActive(true);
            
        // Oyunu duraklat
        isPaused = true;
        Time.timeScale = 0f;
    }
    
    public void ResumeGame()
    {
        Debug.Log("Game Resumed");
        
        // Pause panelini gizle
        if (pausePanel != null)
            pausePanel.SetActive(false);
            
        // Oyunu devam ettir
        isPaused = false;
        Time.timeScale = 1f;
    }
    
    void QuitGame()
    {
        Debug.Log("Quitting game from pause menu");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    void ReturnToMainMenu()
    {
        Debug.Log("Returning to main menu");
        
        // Zamanı normale döndür
        Time.timeScale = 1f;
        isPaused = false;
        
        // Ana menüye dön
        SceneManager.LoadScene("menu");
    }
    
    // Test için
    [ContextMenu("Test Pause")]
    void TestPause()
    {
        PauseGame();
    }
} 