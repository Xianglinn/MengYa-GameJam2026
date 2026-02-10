using System;

[System.Serializable]
public class GameState
{
    // 当前对话进度
    public string currentDialogueId;
    public string currentScene;
    
    // 玩家金钱
    public int money = 0;
    
    // 存档信息 (DateTime 不能被 JsonUtility 序列化，用字符串代替)
    public string saveTime;  // ISO 8601 格式: "2026-02-03T17:30:00"
    public string saveName;
    
    // 构造函数
    public GameState()
    {
        currentDialogueId = Constants.DefaultStartId;
        currentScene = Constants.Scenes.Prologue;
        saveTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
        saveName = "New Game";
        money = 0;
    }
    
    // 获取 DateTime 格式的保存时间
    public DateTime GetSaveDateTime()
    {
        if (DateTime.TryParse(saveTime, out DateTime result))
            return result;
        return DateTime.MinValue;
    }
}
