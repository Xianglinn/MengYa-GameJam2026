using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
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

    // 继续游戏（加载存档）
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
}
