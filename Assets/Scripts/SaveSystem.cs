using System;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string SaveDirectory => Path.Combine(Application.persistentDataPath, "Saves");
    
    // 确保存档目录存在
    static SaveSystem()
    {
        if (!Directory.Exists(SaveDirectory))
            Directory.CreateDirectory(SaveDirectory);
    }
    
    // 获取存档文件路径
    private static string GetSavePath(int slot)
    {
        return Path.Combine(SaveDirectory, $"{Constants.SaveFilePrefix}{slot}{Constants.SaveFileExtension}");
    }
    
    // 创建新游戏
    public static GameState NewGame(string startNodeId)
    {
        var state = new GameState
        {
            currentDialogueId = startNodeId,
            currentScene = Constants.Scenes.Prologue,
            saveTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
            saveName = "New Game"
        };
        return state;
    }
    
    // 保存游戏
    public static void Save(int slot, GameState state)
    {
        if (state == null)
        {
            Debug.LogError($"[SaveSystem] Cannot save null state to slot {slot}");
            return;
        }
        
        if (slot < 0 || slot >= Constants.MaxSaveSlots)
        {
            Debug.LogError($"[SaveSystem] Invalid save slot: {slot}. Must be between 0 and {Constants.MaxSaveSlots - 1}");
            return;
        }
        
        try
        {
            state.saveTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            string json = JsonUtility.ToJson(state, true);
            string path = GetSavePath(slot);
            File.WriteAllText(path, json);
            Debug.Log($"[SaveSystem] Game saved to slot {slot} at {path}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] Failed to save to slot {slot}: {e.Message}");
        }
    }
    
    // 加载游戏
    public static GameState Load(int slot)
    {
        if (slot < 0 || slot >= Constants.MaxSaveSlots)
        {
            Debug.LogError($"[SaveSystem] Invalid save slot: {slot}");
            return null;
        }
        
        string path = GetSavePath(slot);
        
        if (!File.Exists(path))
        {
            Debug.LogWarning($"[SaveSystem] No save file found in slot {slot}");
            return null;
        }
        
        try
        {
            string json = File.ReadAllText(path);
            GameState state = JsonUtility.FromJson<GameState>(json);
            Debug.Log($"[SaveSystem] Game loaded from slot {slot}");
            return state;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] Failed to load from slot {slot}: {e.Message}");
            return null;
        }
    }
    
    // 检查存档是否存在
    public static bool HasSave(int slot)
    {
        if (slot < 0 || slot >= Constants.MaxSaveSlots)
            return false;
        
        return File.Exists(GetSavePath(slot));
    }
    
    // 删除存档
    public static void DeleteSave(int slot)
    {
        if (slot < 0 || slot >= Constants.MaxSaveSlots)
        {
            Debug.LogError($"[SaveSystem] Invalid save slot: {slot}");
            return;
        }
        
        string path = GetSavePath(slot);
        
        if (File.Exists(path))
        {
            try
            {
                File.Delete(path);
                Debug.Log($"[SaveSystem] Deleted save in slot {slot}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Failed to delete save in slot {slot}: {e.Message}");
            }
        }
    }
    
    // 获取存档信息（用于显示存档列表）
    public static SaveInfo GetSaveInfo(int slot)
    {
        if (!HasSave(slot))
            return null;
        
        try
        {
            string json = File.ReadAllText(GetSavePath(slot));
            GameState state = JsonUtility.FromJson<GameState>(json);
            
            return new SaveInfo
            {
                slot = slot,
                saveName = state.saveName,
                saveTime = state.saveTime,
                currentScene = state.currentScene,
                money = state.money  // 包含金钱
            };
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] Failed to get save info from slot {slot}: {e.Message}");
            return null;
        }
    }
}

// 存档信息结构（用于显示）
[System.Serializable]
public class SaveInfo
{
    public int slot;
    public string saveName;
    public string saveTime;  // 字符串格式的时间
    public string currentScene;
    public int money;
    
    // 获取 DateTime 格式的保存时间
    public DateTime GetSaveDateTime()
    {
        if (DateTime.TryParse(saveTime, out DateTime result))
            return result;
        return DateTime.MinValue;
    }
}
