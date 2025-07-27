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
    public Slider soundSlider;
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

        int difficulty = PlayerPrefs.GetInt("Difficulty", 0);
        
        // Slider değerlerini PlayerPrefs'ten yükle
        LoadSliderValues();
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
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
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
        
        // Slider'ları ayarla
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
            Debug.Log("Music slider listener added");
        }
        else
        {
            Debug.LogWarning("Music slider is null!");
        }
            
        if (soundSlider != null)
        {
            soundSlider.onValueChanged.AddListener(OnSoundSliderChanged);
            Debug.Log("Sound slider listener added");
        }
        else
        {
            Debug.LogWarning("Sound slider is null!");
        }
        
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
    public void OnBackToMainButtonClicked()
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

    public void OnGraphicsDropdownChanged(int value)
    {
        // value: seçilen grafik kalitesi (0, 1, 2, ...)
        QualitySettings.SetQualityLevel(value);
        Debug.Log("Graphics quality changed: " + value);
    }

    public void OnDifficultyDropdownChanged(int value)
    {
        // value: seçilen zorluk seviyesi (0: Kolay, 1: Orta, 2: Zor gibi)
        PlayerPrefs.SetInt("Difficulty", value);
        PlayerPrefs.Save();
        Debug.Log("Difficulty changed: " + value);
    }

    public void OnMusicSliderChanged(float value)
    {
        // value: 0.0 - 1.0 arası müzik sesi
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
        Debug.Log("Music volume changed: " + value);
        // Burada müzik sesini ayarlayan AudioSource varsa onu da güncelleyebilirsin
    }

    public void OnSoundSliderChanged(float value)
    {
        // value: 0.0 - 1.0 arası ses seviyesi
        PlayerPrefs.SetFloat("SoundVolume", value);
        PlayerPrefs.Save();
        Debug.Log("Sound volume changed: " + value);
        // Burada ses efektini ayarlayan AudioSource varsa onu da güncelleyebilirsin
    }

    public void OnResolutionDropdownChanged(int value)
    {
        // Örnek: 0 = 1920x1080, 1 = 1280x720, 2 = 800x600
        switch (value)
        {
            case 0:
                Screen.SetResolution(1920, 1080, Screen.fullScreen);
                break;
            case 1:
                Screen.SetResolution(1280, 720, Screen.fullScreen);
                break;
            case 2:
                Screen.SetResolution(800, 600, Screen.fullScreen);
                break;
        }
        Debug.Log("Resolution changed: " + Screen.currentResolution);
    }
    
    void LoadSliderValues()
    {
        Debug.Log("Loading slider values...");
        
        // Müzik slider'ını yükle
        if (musicSlider != null)
        {
            float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
            musicSlider.value = musicVolume;
            Debug.Log("Music slider value set to: " + musicVolume);
        }
        else
        {
            Debug.LogWarning("Music slider is null in LoadSliderValues!");
        }
        
        // Ses slider'ını yükle
        if (soundSlider != null)
        {
            float soundVolume = PlayerPrefs.GetFloat("SoundVolume", 1.0f);
            soundSlider.value = soundVolume;
            Debug.Log("Sound slider value set to: " + soundVolume);
        }
        else
        {
            Debug.LogWarning("Sound slider is null in LoadSliderValues!");
        }
    }
}
