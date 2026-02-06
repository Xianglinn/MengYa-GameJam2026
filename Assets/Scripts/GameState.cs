using System;
using System.Collections.Generic;

[System.Serializable]
public class GameState
{
    // 当前对话进度
    public string currentDialogueId;
    public string currentScene;
    
    // 游戏变量（用于条件判断和效果）
    public Dictionary<string, int> intVars = new Dictionary<string, int>();
    public Dictionary<string, bool> boolVars = new Dictionary<string, bool>();
    public Dictionary<string, string> stringVars = new Dictionary<string, string>();
    
    // 已读对话记录（用于跳过已读内容）
    public HashSet<string> readDialogues = new HashSet<string>();
    
    // 存档信息
    public DateTime saveTime;
    public string saveName;
    
    // 构造函数
    public GameState()
    {
        currentDialogueId = Constants.DefaultStartId;
        currentScene = Constants.Scenes.Prologue;
        saveTime = DateTime.Now;
        saveName = "New Game";
    }
    
    // 获取/设置变量的便捷方法
    public int GetInt(string key, int defaultValue = 0)
    {
        return intVars.TryGetValue(key, out int value) ? value : defaultValue;
    }
    
    public void SetInt(string key, int value)
    {
        intVars[key] = value;
    }
    
    public bool GetBool(string key, bool defaultValue = false)
    {
        return boolVars.TryGetValue(key, out bool value) ? value : defaultValue;
    }
    
    public void SetBool(string key, bool value)
    {
        boolVars[key] = value;
    }
    
    public string GetString(string key, string defaultValue = "")
    {
        return stringVars.TryGetValue(key, out string value) ? value : defaultValue;
    }
    
    public void SetString(string key, string value)
    {
        stringVars[key] = value;
    }
    
    // 标记对话为已读
    public void MarkAsRead(string dialogueId)
    {
        if (!string.IsNullOrEmpty(dialogueId))
            readDialogues.Add(dialogueId);
    }
    
    public bool HasRead(string dialogueId)
    {
        return readDialogues.Contains(dialogueId);
    }
}
