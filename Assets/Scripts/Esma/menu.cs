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
    public Dropdown dropdownGraphics;
    public Dropdown dropdownDifficulty;
    
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
    
    [Header("Fire Animation")]
    public GameObject fireObject; // Fire animasyonu için
    
    private bool isPaused = false;
    private bool isInSettings = false;
    private bool settingsFromPause = false; // Settings menüsünün pause'tan mı geldiğini kontrol eder
    
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
        
        // Dropdown değerlerini yükle
        LoadDropdownValues();
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
        
        // Pause durumunu sıfırla
        isPaused = false;
        isInSettings = false;
        settingsFromPause = false;
        
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
        
        // Dropdown listeners
        if (dropdownGraphics != null)
        {
            dropdownGraphics.onValueChanged.AddListener(OnGraphicsDropdownChanged);
            Debug.Log("Graphics dropdown listener added");
        }
        else
        {
            Debug.LogWarning("Graphics dropdown is null!");
        }
        
        if (dropdownDifficulty != null)
        {
            dropdownDifficulty.onValueChanged.AddListener(OnDifficultyDropdownChanged);
            Debug.Log("Difficulty dropdown listener added");
        }
        else
        {
            Debug.LogWarning("Difficulty dropdown is null!");
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
        // Tüm panelleri sıfırla
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        
        // Durumları sıfırla
        isInSettings = false;
        isPaused = false;
        settingsFromPause = false; // Settings flag'ini de sıfırla
        
        // Fire animasyonunu başlat
        StartFireAnimation();
        
        // İlk butonu seç
        selectedButtonIndex = 0;
        UpdateButtonSelection();
    }
    
    void StartFireAnimation()
    {
        if (fireObject != null)
        {
            fireObject.SetActive(true);
            Animator anim = fireObject.GetComponent<Animator>();
            if (anim != null)
            {
                anim.Play("FireAnimation"); // Animasyon adını buraya yazın
                Debug.Log("Fire animation started");
            }
        }
    }
    
    void StopFireAnimation()
    {
        if (fireObject != null)
        {
            fireObject.SetActive(false);
            Debug.Log("Fire animation stopped");
        }
    }
    
    public void OnPlayButtonClicked()
    {
        Debug.Log("Play button clicked - Starting new game");
        // Yeni oyun başlat
        SceneManager.LoadScene("SampleScene"); // SampleScene sahnesine git
    }
    
    public void OnContinueButtonClicked()
    {
        Debug.Log("Continue button clicked - Loading saved game");
        SceneManager.LoadScene("SampleScene"); // SampleScene sahnesine git
    }
    
    public void OnSettingsButtonClicked()
    {
        Debug.Log("Settings button clicked from main menu");
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);

        isInSettings = true;
        settingsFromPause = false; // Main menu'den geldiğini işaretle
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
        if (settingsFromPause)
        {
            // Pause menu'den geldiyse pause menu'ye dön
            Debug.Log("Returning to pause menu from settings");
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (pausePanel != null) pausePanel.SetActive(true);
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false); // Main menu'yu gizle
            isInSettings = false;
            isPaused = true; // Pause durumunu koru
            // Oyun hala duraklatılmış durumda kalmalı (Time.timeScale = 0)
        }
        else
        {
            // Main menu'den geldiyse main menu'ye dön
            Debug.Log("Returning to main menu from settings");
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (pausePanel != null) pausePanel.SetActive(false); // Pause menu'yu gizle
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
            isInSettings = false;
            isPaused = false;
            
            // İlk butonu seç
            selectedButtonIndex = 0;
            UpdateButtonSelection();
        }
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
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
        
        isInSettings = true;
        settingsFromPause = true; // Pause menu'den geldiğini işaretle
        // Oyun hala duraklatılmış durumda kalmalı (Time.timeScale = 0)
    }
    
    void OnPauseMainMenuButtonClicked()
    {
        Debug.Log("Pause main menu button clicked");
        Time.timeScale = 1f; // Zamanı normale döndür
        isPaused = false;
        
        // Ana menüye dön
        SceneManager.LoadScene("menu"); // Ana menü sahnesine dön
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
        // value: 0 = Low, 1 = Medium, 2 = High
        switch (value)
        {
            case 0: // Low
                QualitySettings.SetQualityLevel(0);
                Debug.Log("Graphics quality set to: Low");
                break;
            case 1: // Medium
                QualitySettings.SetQualityLevel(1);
                Debug.Log("Graphics quality set to: Medium");
                break;
            case 2: // High
                QualitySettings.SetQualityLevel(2);
                Debug.Log("Graphics quality set to: High");
                break;
            default:
                QualitySettings.SetQualityLevel(1); // Default to Medium
                Debug.Log("Graphics quality set to: Medium (default)");
                break;
        }
        
        // Ayarı kaydet
        PlayerPrefs.SetInt("GraphicsQuality", value);
        PlayerPrefs.Save();
    }

    public void OnDifficultyDropdownChanged(int value)
    {
        // value: 0 = Easy, 1 = Normal, 2 = Hard
        switch (value)
        {
            case 0: // Easy
                PlayerPrefs.SetInt("Difficulty", 0);
                Debug.Log("Difficulty set to: Easy");
                break;
            case 1: // Normal
                PlayerPrefs.SetInt("Difficulty", 1);
                Debug.Log("Difficulty set to: Normal");
                break;
            case 2: // Hard
                PlayerPrefs.SetInt("Difficulty", 2);
                Debug.Log("Difficulty set to: Hard");
                break;
            default:
                PlayerPrefs.SetInt("Difficulty", 1); // Default to Normal
                Debug.Log("Difficulty set to: Normal (default)");
                break;
        }
        
        PlayerPrefs.Save();
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
    
    void LoadDropdownValues()
    {
        Debug.Log("Loading dropdown values...");
        
        // Graphics dropdown değerini yükle
        if (dropdownGraphics != null)
        {
            int graphicsQuality = PlayerPrefs.GetInt("GraphicsQuality", 1); // Default: Medium
            dropdownGraphics.value = graphicsQuality;
            Debug.Log("Graphics dropdown value set to: " + graphicsQuality);
        }
        else
        {
            // Otomatik olarak Graphics dropdown'u bul
            dropdownGraphics = FindDropdownByName("Graphics");
            if (dropdownGraphics != null)
            {
                int graphicsQuality = PlayerPrefs.GetInt("GraphicsQuality", 1);
                dropdownGraphics.value = graphicsQuality;
                Debug.Log("Graphics dropdown found and value set to: " + graphicsQuality);
            }
            else
            {
                Debug.LogWarning("Graphics dropdown not found! Please assign it in the inspector.");
            }
        }
        
        // Difficulty dropdown değerini yükle
        if (dropdownDifficulty != null)
        {
            int difficulty = PlayerPrefs.GetInt("Difficulty", 1); // Default: Normal
            dropdownDifficulty.value = difficulty;
            Debug.Log("Difficulty dropdown value set to: " + difficulty);
        }
        else
        {
            // Otomatik olarak Difficulty dropdown'u bul
            dropdownDifficulty = FindDropdownByName("Difficulty");
            if (dropdownDifficulty != null)
            {
                int difficulty = PlayerPrefs.GetInt("Difficulty", 1);
                dropdownDifficulty.value = difficulty;
                Debug.Log("Difficulty dropdown found and value set to: " + difficulty);
            }
            else
            {
                Debug.LogWarning("Difficulty dropdown not found! Please assign it in the inspector.");
            }
        }
    }
    
    // Dropdown'u ismine göre bulan yardımcı fonksiyon
    Dropdown FindDropdownByName(string name)
    {
        Dropdown[] allDropdowns = FindObjectsOfType<Dropdown>();
        foreach (Dropdown dropdown in allDropdowns)
        {
            if (dropdown.name.Contains(name))
            {
                return dropdown;
            }
        }
        return null;
    }
}
