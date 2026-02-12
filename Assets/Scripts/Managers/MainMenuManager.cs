using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button loadGameButton;  // 打开存档面板
    [SerializeField] private Button quitButton;
    
    [Header("Save Slot Settings")]
    [SerializeField] private int defaultSaveSlot = 0;

    private void Start()
    {
        // 检查是否有存档，如果没有则禁用继续按钮
        if (continueButton != null)
        {
            bool hasSave = SaveSystem.HasSave(defaultSaveSlot);
            continueButton.interactable = hasSave;
            
            if (!hasSave)
                Debug.Log("[MainMenu] No save file found, Continue button disabled");
        }
        
        // 确保 GameManager 存在
        if (GameManager.I == null)
        {
            GameObject gmObj = new GameObject("GameManager");
            gmObj.AddComponent<GameManager>();
            Debug.Log("[MainMenu] GameManager created");
        }
    }

    // 开始新游戏
    public void OnNewGame()
    {
        Debug.Log("[MainMenu] Starting new game...");
        
        if (GameManager.I == null)
        {
            Debug.LogError("[MainMenu] GameManager not found!");
            return;
        }
        
        // 创建新游戏状态
        GameManager.I.StartNewGame();
        
        // 跳转到游戏场景
        GameManager.I.GoToGameScene();
    }

    // 继续游戏（快速加载最新存档）
    public void OnContinue()
    {
        Debug.Log($"[MainMenu] Loading save from slot {defaultSaveSlot}...");
        
        if (GameManager.I == null)
        {
            Debug.LogError("[MainMenu] GameManager not found!");
            return;
        }
        
        // 尝试加载存档
        if (GameManager.I.TryLoad(defaultSaveSlot))
        {
            // 加载成功，跳转到对应场景
            string sceneName = GameManager.I.State.currentScene;
            GameManager.I.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("[MainMenu] Failed to load save file!");
        }
    }

    // 打开存档面板（选择存档加载）
    public void OnLoadGame()
    {
        Debug.Log("[MainMenu] Opening load game panel...");
        
        if (UIManager.I != null)
        {
            UIManager.I.OnLoad();
        }
        else
        {
            Debug.LogError("[MainMenu] UIManager not found!");
        }
    }

    // 打开结局图鉴
    public void OnOpenEndingGallery()
    {
        Debug.Log("[MainMenu] Opening ending gallery...");
        
        if (GameManager.I == null)
        {
            GameObject gmObj = new GameObject("GameManager");
            gmObj.AddComponent<GameManager>();
        }
        
        GameManager.I.LoadScene(Constants.Scenes.EndingGallery);
    }
    
    // 退出游戏
    public void OnQuit()
    {
        Debug.Log("[MainMenu] Quitting game...");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    // 额外功能：删除存档
    public void OnDeleteSave()
    {
        if (SaveSystem.HasSave(defaultSaveSlot))
        {
            SaveSystem.DeleteSave(defaultSaveSlot);
            Debug.Log($"[MainMenu] Deleted save in slot {defaultSaveSlot}");
            
            // 刷新继续按钮状态
            if (continueButton != null)
                continueButton.interactable = false;
        }
    }
    
    // ===== 测试快捷键（仅用于开发测试）=====
    // TODO: 正式发布前删除此方法
    private void Update()
    {
        // 确保 GameManager 存在
        if (GameManager.I == null)
            return;
        
        // T 键 - 坏结局1（Bad Ending - 出师未捷）
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("[MainMenu] [TEST] Jumping to Bad Ending (出师未捷)");
            GameManager.I.StartNewGame(); // 确保有游戏状态
            GameManager.I.GoToBadEndingFailed();
        }
        
        // Y 键 - 好结局3（Happy Ending - 玉祝福）
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("[MainMenu] [TEST] Jumping to Happy Ending (玉祝福)");
            GameManager.I.StartNewGame();
            GameManager.I.GoToHappyEndingJadeBlessed();
        }
        
        // U 键 - 普通结局2（Normal Ending - 漂流）
        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("[MainMenu] [TEST] Jumping to Normal Ending (漂流)");
            GameManager.I.StartNewGame();
            GameManager.I.GoToNormalEndingDrifting();
        }
        
        // I 键 - 真结局4（True Ending - 风）
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("[MainMenu] [TEST] Jumping to True Ending (风)");
            GameManager.I.StartNewGame();
            GameManager.I.GoToTrueEndingWind();
        }
    }
}
