using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject pausePanel;
    
    [Header("Main Menu Buttons")]
    public Button playButton;
    public Button continueButton;
    public Button settingsButton;
    public Button creditsButton;
    public Button quitButton;
    
    [Header("Settings Menu")]
    public Button backToMainButton;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle fullscreenToggle;
    
    [Header("Pause Menu")]
    public Button resumeButton;
    public Button pauseSettingsButton;
    public Button pauseMainMenuButton;
    
    [Header("Camera Settings")]
    public Camera menuCamera;
    public Transform cameraTarget;
    public float cameraRotationSpeed = 10f;
    public float cameraDistance = 10f;
    
    [Header("Menu Navigation")]
    public int selectedButtonIndex = 0;
    public Button[] mainMenuButtons;
    
    private bool isPaused = false;
    private bool isInSettings = false;
    
    void Start()
    {
        Debug.Log("Menu script started");
        SetupMenu();
        SetupCamera();
        SetupButtonListeners();
        
        // Ana menüyü göster, diğerlerini gizle
        ShowMainMenu();
        
        PlayerPrefs.SetInt("SaveGame", 1);
        PlayerPrefs.Save();
    }
    
    void Update()
    {
        HandleCameraRotation();
        HandleKeyboardNavigation();
        HandlePauseInput();
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC basıldı, ana menüye dönülüyor");
            PlayerPrefs.SetInt("SaveGame", 1);
            PlayerPrefs.Save();
            SceneManager.LoadScene("menu");
        }
    }
    
    void SetupMenu()
    {
        // Menü panellerini başlangıçta gizle
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        
        // Continue butonunu sadece kayıtlı oyun varsa aktif et
        if (continueButton != null)
        {
            bool hasSaveGame = PlayerPrefs.HasKey("SaveGame");
            continueButton.interactable = hasSaveGame;
        }
    }
    
    void SetupCamera()
    {
        if (menuCamera == null)
        {
            menuCamera = Camera.main;
        }
        
        // Kamera pozisyonunu ayarla
        if (cameraTarget != null)
        {
            menuCamera.transform.position = cameraTarget.position + Vector3.back * cameraDistance;
            menuCamera.transform.LookAt(cameraTarget);
        }
    }
    
    void SetupButtonListeners()
    {
        Debug.Log("Button listeners set");
        // Ana menü butonları
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonClicked);
            
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueButtonClicked);
            
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            
        if (creditsButton != null)
            creditsButton.onClick.AddListener(OnCreditsButtonClicked);
            
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitButtonClicked);
        
        // Ayarlar menüsü
        if (backToMainButton != null)
            backToMainButton.onClick.AddListener(OnBackToMainButtonClicked);
        
        // Duraklat menüsü
        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResumeButtonClicked);
            
        if (pauseSettingsButton != null)
            pauseSettingsButton.onClick.AddListener(OnPauseSettingsButtonClicked);
            
        if (pauseMainMenuButton != null)
            pauseMainMenuButton.onClick.AddListener(OnPauseMainMenuButtonClicked);
    }
    
    void HandleCameraRotation()
    {
        if (menuCamera != null && cameraTarget != null)
        {
            // Kamerayı yavaşça döndür
            menuCamera.transform.RotateAround(cameraTarget.position, Vector3.up, cameraRotationSpeed * Time.deltaTime);
            menuCamera.transform.LookAt(cameraTarget);
        }
    }
    
    void HandleKeyboardNavigation()
    {
        if (isInSettings || isPaused) return;
        
        // Yukarı/aşağı ok tuşları ile navigasyon
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedButtonIndex = (selectedButtonIndex - 1 + mainMenuButtons.Length) % mainMenuButtons.Length;
            UpdateButtonSelection();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedButtonIndex = (selectedButtonIndex + 1) % mainMenuButtons.Length;
            UpdateButtonSelection();
        }
        
        // Enter tuşu ile seçim
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if (selectedButtonIndex < mainMenuButtons.Length && mainMenuButtons[selectedButtonIndex] != null)
            {
                mainMenuButtons[selectedButtonIndex].onClick.Invoke();
            }
        }
    }
    
    void HandlePauseInput()
    {
        // ESC tuşu ile duraklat/devam et
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                OnResumeButtonClicked();
            }
            else
            {
                OnPauseButtonClicked();
            }
        }
    }
    
    void UpdateButtonSelection()
    {
        for (int i = 0; i < mainMenuButtons.Length; i++)
        {
            if (mainMenuButtons[i] != null)
            {
                ColorBlock colors = mainMenuButtons[i].colors;
                if (i == selectedButtonIndex)
                {
                    colors.normalColor = Color.yellow;
                }
                else
                {
                    colors.normalColor = Color.white;
                }
                mainMenuButtons[i].colors = colors;
            }
        }
    }
    
    // Ana menü fonksiyonları
    void ShowMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        
        isInSettings = false;
        isPaused = false;
        
        // İlk butonu seç
        selectedButtonIndex = 0;
        UpdateButtonSelection();
    }
    
    public void OnPlayButtonClicked()
    {
        Debug.Log("Play button clicked - Starting new game");
        // Yeni oyun başlat
        SceneManager.LoadScene(1); // 1 numaralı sahne SampleScene
    }
    
    public void OnContinueButtonClicked()
    {
        Debug.Log("Continue button clicked - Loading saved game");
        SceneManager.LoadScene("SampleScene");
    }
    
    public void OnSettingsButtonClicked()
    {
        Debug.Log("Settings button clicked");
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
        
        isInSettings = true;
    }
    
    public void OnCreditsButtonClicked()
    {
        Debug.Log("Credits button clicked");
        // Krediler menüsünü göster
    }
    
    public void OnQuitButtonClicked()
    {
        Debug.Log("Quit button clicked");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    // Ayarlar menüsü fonksiyonları
    void OnBackToMainButtonClicked()
    {
        ShowMainMenu();
    }
    
    // Duraklat menüsü fonksiyonları
    void OnPauseButtonClicked()
    {
        if (pausePanel != null) pausePanel.SetActive(true);
        isPaused = true;
        Time.timeScale = 0f; // Oyunu duraklat
    }
    
    void OnResumeButtonClicked()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f; // Oyunu devam ettir
    }
    
    void OnPauseSettingsButtonClicked()
    {
        Debug.Log("Pause settings button clicked");
        // Duraklat menüsünden ayarlara git
    }
    
    void OnPauseMainMenuButtonClicked()
    {
        Debug.Log("Pause main menu button clicked");
        Time.timeScale = 1f; // Zamanı normale döndür
        // Ana menüye dön
        // SceneManager.LoadScene("MainMenu");
    }

    // Oyuncu bir ilerleme kaydettiğinde çağır
    void SaveGame()
    {
        PlayerPrefs.SetInt("SaveGame", 1); // Sadece varlığını kontrol etmek için
        // İstersen başka veriler de kaydedebilirsin
        // PlayerPrefs.SetInt("Level", currentLevel);
        // PlayerPrefs.SetFloat("PlayerX", player.transform.position.x);
        PlayerPrefs.Save();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SaveGame();
    }
}
