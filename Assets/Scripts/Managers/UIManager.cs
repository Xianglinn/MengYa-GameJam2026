using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager I { get; private set; }

    [Header("Quick Access UI")]
    [SerializeField] private GameObject quickAccessPanel;  // 右上角容器
    [SerializeField] private Button saveButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button muteButton;

    [Header("Mute Button Icons")]
    [SerializeField] private Sprite soundOnIcon;   // 音量开启图标
    [SerializeField] private Sprite soundOffIcon;  // 静音图标

    [Header("Save Panel")]
    [SerializeField] private SavePanelManager savePanelManager;

    private bool isMuted = false;

    private void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        // 绑定按钮事件
        if (saveButton != null)
            saveButton.onClick.AddListener(OnSave);
        
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnReturnToMainMenu);
        
        if (muteButton != null)
            muteButton.onClick.AddListener(OnToggleMute);
        
        // 加载静音状态
        LoadMuteState();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        
        // 清理按钮事件
        if (saveButton != null)
            saveButton.onClick.RemoveListener(OnSave);
        
        if (mainMenuButton != null)
            mainMenuButton.onClick.RemoveListener(OnReturnToMainMenu);
        
        if (muteButton != null)
            muteButton.onClick.RemoveListener(OnToggleMute);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 主菜单和结局图鉴隐藏快捷按钮
        bool shouldHidePanel = scene.name == Constants.Scenes.MainMenu || 
                               scene.name == Constants.Scenes.EndingGallery;
        if (quickAccessPanel)
            quickAccessPanel.SetActive(!shouldHidePanel);
    }

    // === 按钮功能 ===
    public void OnSave()
    {
        if (GameManager.I == null)
        {
            Debug.LogError("[UIManager] GameManager not found! Cannot save.");
            return;
        }
        
        // 打开存档面板（保存模式）
        if (savePanelManager != null)
        {
            savePanelManager.OpenSavePanel(SavePanelManager.SavePanelMode.Save);
        }
        else
        {
            // 如果没有存档面板，使用快速保存
            GameManager.I.QuickSave();
            Debug.Log("[UIManager] Game saved to slot 0");
        }
    }
    
    // 打开加载面板（主菜单使用）
    public void OnLoad()
    {
        if (savePanelManager != null)
        {
            savePanelManager.OpenSavePanel(SavePanelManager.SavePanelMode.Load);
        }
        else
        {
            Debug.LogError("[UIManager] SavePanelManager not found!");
        }
    }

    public void OnReturnToMainMenu()
    {
        Debug.Log("[UIManager] Return to Main Menu button clicked");
        
        if (GameManager.I == null)
        {
            Debug.LogError("[UIManager] GameManager not found! Cannot return to main menu.");
            return;
        }
        
        Debug.Log($"[UIManager] Loading scene: {Constants.Scenes.MainMenu}");
        GameManager.I.GoToMainMenu();
    }

    public void OnToggleMute()
    {
        isMuted = !isMuted;
        AudioListener.volume = isMuted ? 0f : 1f;

        // 更新按钮图标
        if (muteButton != null)
        {
            var image = muteButton.GetComponent<Image>();
            if (image != null)
                image.sprite = isMuted ? soundOffIcon : soundOnIcon;
        }

        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadMuteState()
    {
        isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;
        AudioListener.volume = isMuted ? 0f : 1f;

        // 初始化按钮图标
        if (muteButton != null)
        {
            var image = muteButton.GetComponent<Image>();
            if (image != null)
                image.sprite = isMuted ? soundOffIcon : soundOnIcon;
        }
    }
}