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
}