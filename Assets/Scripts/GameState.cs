using System;
using System.Collections.Generic;

[System.Serializable]
public class GameState
{
    // 当前对话进度
    public string currentDialogueId;
    public string currentScene;
    
    // 玩家金钱
    public int money = 0;
    
    // 已读对话记录（用于跳过已读内容）
    public List<string> readDialogues = new List<string>();
    
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
        money = 0;
    }
    
    // 标记对话为已读
    public void MarkAsRead(string dialogueId)
    {
        if (!string.IsNullOrEmpty(dialogueId) && !readDialogues.Contains(dialogueId))
            readDialogues.Add(dialogueId);
    }
    
    public bool HasRead(string dialogueId)
    {
        return readDialogues.Contains(dialogueId);
    }
}
