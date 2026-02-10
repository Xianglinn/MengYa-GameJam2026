using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }
    public GameState State { get; private set; }

    private void Awake()
    {
        if (I != null) 
        { 
            Destroy(gameObject); 
            return; 
        }
        I = this;
        DontDestroyOnLoad(gameObject);
        
        // 监听场景加载，自动更新当前场景
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // 更新当前场景（如果有游戏状态）
        if (State != null)
        {
            State.currentScene = scene.name;
            Debug.Log($"[GameManager] Scene loaded: {scene.name}, State updated");
        }
    }

    // 开始新游戏
    public void StartNewGame(string startNodeId = null)
    {
        if (string.IsNullOrEmpty(startNodeId))
            startNodeId = Constants.DefaultStartId;
        
        State = SaveSystem.NewGame(startNodeId);
        Debug.Log($"[GameManager] Starting new game from {startNodeId}");
    }

    // 加载存档
    public bool TryLoad(int slot)
    {
        var loaded = SaveSystem.Load(slot);
        if (loaded == null) return false;
        State = loaded;
        Debug.Log($"[GameManager] Loaded game from slot {slot}");
        return true;
    }

    // 保存游戏
    public void Save(int slot)
    {
        if (State == null)
        {
            Debug.LogWarning("[GameManager] Cannot save: no active game state");
            return;
        }
        
        // 确保保存当前场景
        State.currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"[GameManager] Saving to slot {slot}, Scene: {State.currentScene}, Money: {State.money}");
        
        SaveSystem.Save(slot, State);
    }

    // 快速保存（默认槽位0）
    public void QuickSave()
    {
        Save(0);
    }

    // 场景切换
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(Constants.Scenes.MainMenu);
    }

    public void GoToGameScene()
    {
        SceneManager.LoadScene(Constants.Scenes.Prologue);
    }
    
    // ===== 结局跳转方法 =====
    
    /// <summary>
    /// 跳转到结局场景，加载指定的结局 CSV 文件
    /// </summary>
    /// <param name="endingStoryFile">结局故事文件路径（例如 Constants.StoryFiles.EndingBadFailed）</param>
    /// <param name="startDialogueId">结局的起始对话 ID（默认从第一行开始）</param>
    public void GoToEnding(string endingStoryFile, string startDialogueId = null)
    {
        if (State == null)
        {
            Debug.LogError("[GameManager] Cannot go to ending: no active game state");
            return;
        }
        
        // 设置要加载的故事文件
        State.currentStoryFile = endingStoryFile;
        
        // 设置起始对话 ID（如果提供）
        if (!string.IsNullOrEmpty(startDialogueId))
        {
            State.currentDialogueId = startDialogueId;
        }
        else
        {
            // 默认从头开始（VNManager 会使用 CSV 的第一行）
            State.currentDialogueId = "";
        }
        
        Debug.Log($"[GameManager] Going to ending: {endingStoryFile}, Start ID: {startDialogueId ?? "auto"}");
        
        // 加载结局场景
        SceneManager.LoadScene(Constants.Scenes.Endings);
    }
    
    // 便捷方法 - 跳转到各个结局
    public void GoToBadEndingFailed() => GoToEnding(Constants.StoryFiles.EndingBadFailed, "BE_0001");
    public void GoToNormalEndingDrifting() => GoToEnding(Constants.StoryFiles.EndingNormalDrifting, "NE_0001");
    public void GoToTrueEndingWind() => GoToEnding(Constants.StoryFiles.EndingTrueWind, "TE_0001");
    public void GoToHappyEndingJadeBlessed() => GoToEnding(Constants.StoryFiles.EndingHappyJadeBlessed, "HE_0001");
}