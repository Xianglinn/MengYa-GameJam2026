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