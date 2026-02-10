using UnityEngine;

public static class Constants
{
    // ===== Dialogue System =====
    
    // 对话类型
    public static class DialogueType
    {
        public const string Line = "line";
        public const string Narration = "narration";
        public const string Choice = "choice";
        public const string Jump = "jump";
        public const string Command = "command";
    }
    
    // 特殊标记
    public const string EndMarker = "END";
    public const string PipeSeparator = "|";
    
    // CSV 列名
    public static class CsvColumns
    {
        public const string Id = "id";
        public const string NextId = "next_id";
        public const string Speaker = "speaker";
        public const string Scene = "scene";
        public const string Content = "content";
        public const string Type = "type";
        public const string Choices = "choices";
        public const string Target = "target";
        public const string Condition = "condition";
        public const string Expression = "expression";
        public const string Fx = "fx";
        public const string Bg = "bg";
        public const string Effect = "effect";
        public const string NameOverride = "nameOverride";
        public const string Bgm = "bgm";
    }
    
    // ===== Input =====
    public static class InputKeys
    {
        public const KeyCode Advance1 = KeyCode.Space;
        public const KeyCode Advance2 = KeyCode.Return;
        public const int MouseButton = 0;
    }
    
    // ===== Debug Log Tags =====
    public const string VNManagerTag = "[VNManager]";
    public const string DialogueTag = "[Dialogue]";
    
    // ===== Default Values =====
    public const string DefaultStoryPath = "Story/prologue";
    public const string DefaultStartId = "pre_0001";
    
    // ===== Scene Names =====
    public static class Scenes
    {
        public const string MainMenu = "MainMenu";
        public const string Prologue = "Prologue";
        public const string Game = "Game";
        public const string Endings = "Endings";  // 通用结局场景
    }
    
    // ===== Story File Paths =====
    public static class StoryFiles
    {
        public const string Prologue = "Story/prologue";
        
        // 结局 CSV 文件路径
        public const string EndingBadFailed = "Story/END_BE_failed";      // 坏结局 - 出师未捷
        public const string EndingNormalDrifting = "Story/END_NE_drifting";  // 普通结局 - 漂流
        public const string EndingTrueWind = "Story/END_TE_wind";         // 真结局 - 风
        public const string EndingHappyJadeBlessed = "Story/END_HE_jade_blessed"; // 好结局 - 玉祝福
    }
    
    // ===== Save System =====
    public const string SaveFilePrefix = "save_";
    public const string SaveFileExtension = ".json";
    public const int MaxSaveSlots = 3;
}