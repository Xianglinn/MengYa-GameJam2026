using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class DialogueLine
{
    public string id;
    public string next_id;
    public string speaker;
    public string scene;
    public string content;
    public string type;          // line / narration / choice / jump / command ...
    public string choices;       // "A|B|C"
    public string target;        // "id1|id2|id3"
    public string condition;     // "trust>=2" etc
    public string expression;
    public string fx;
    public string bg;
    public string effect;        // "trust+=1;flag=true"
    public string nameOverride;
    public string bgm;

    public string DisplayName => string.IsNullOrEmpty(nameOverride) ? speaker : nameOverride;
}


public class VNManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text contentText;
    
    [Header("Story Settings")]
    [SerializeField] private string storyFilePath = Constants.DefaultStoryPath;
    [SerializeField] private string startId = Constants.DefaultStartId;
    
    [Header("Visual Elements")]
    [SerializeField] private Image backgroundImage;     // 背景图
    [SerializeField] private Image avatarImage;         // 角色立绘（左下角）
    
    [Header("Avatar Settings")]
    [SerializeField] private Color activeAvatarColor = Color.white;        // 说话时的颜色
    [SerializeField] private Color inactiveAvatarColor = new Color(0.7f, 0.7f, 0.7f, 1f);  // 不说话时变暗（可选）
    
    [Header("Ending UI")]
    [SerializeField] private GameObject dialoguePanel;           // 对话框面板
    [SerializeField] private GameObject endingAchievementPanel;  // 结局达成面板
    [SerializeField] private TMP_Text endingNameText;            // 结局名字文本

    private Dictionary<string, DialogueLine> lineDict;
    private DialogueLine currentLine;
    private bool waitingForInput = false;
    private bool isShowingEnding = false;  // 是否正在显示结局画面

    void Start()
    {
        // 根据 GameState 决定加载哪个故事文件
        if (GameManager.I?.State != null && !string.IsNullOrEmpty(GameManager.I.State.currentStoryFile))
        {
            storyFilePath = GameManager.I.State.currentStoryFile;
            Debug.Log($"{Constants.VNManagerTag} Loading story file from GameState: {storyFilePath}");
        }
        
        LoadStoryFromFile();
        
        // 如果有保存的游戏状态，从保存的位置继续
        if (GameManager.I?.State != null && !string.IsNullOrEmpty(GameManager.I.State.currentDialogueId))
        {
            string savedId = GameManager.I.State.currentDialogueId;
            
            // 检查保存的对话ID是否存在
            if (lineDict != null && lineDict.TryGetValue(savedId, out DialogueLine savedLine))
            {
                currentLine = savedLine;
                Debug.Log($"{Constants.VNManagerTag} Resuming from saved dialogue: {savedId}");
                
                // 显示保存的对话（不是下一行）
                if (currentLine.type.Equals(Constants.DialogueType.Narration, System.StringComparison.OrdinalIgnoreCase))
                {
                    if (speakerText != null) speakerText.text = "";
                    if (contentText != null) contentText.text = currentLine.content;
                    
                    // 旁白时隐藏角色
                    if (avatarImage != null)
                        avatarImage.gameObject.SetActive(false);
                }
                else
                {
                    if (speakerText != null) speakerText.text = currentLine.DisplayName;
                    if (contentText != null) contentText.text = currentLine.content;
                    
                    // 显示角色立绘
                    ShowAvatar(currentLine.speaker, currentLine.expression);
                }
                
                // 加载背景图
                if (!string.IsNullOrEmpty(currentLine.bg))
                {
                    LoadBackground(currentLine.bg);
                }
                
                waitingForInput = true;
                return;
            }
        }
        
        // 没有保存的状态，或者保存的ID无效，从头开始
        DisplayNextLine();
    }

    void Update()
    {
        if (waitingForInput)
        {
            // 如果正在显示结局达成画面
            if (isShowingEnding)
            {
                // 检测任意输入返回主菜单
                if (Input.GetKeyDown(Constants.InputKeys.Advance1) || 
                    Input.GetKeyDown(Constants.InputKeys.Advance2) ||
                    Input.GetMouseButtonDown(Constants.InputKeys.MouseButton))
                {
                    ReturnToMainMenu();
                    return;
                }
            }
            else
            {
                // 正常对话推进逻辑
                // 检测按键输入（不受 UI 影响）
                if (Input.GetKeyDown(Constants.InputKeys.Advance1) || 
                    Input.GetKeyDown(Constants.InputKeys.Advance2))
                {
                    DisplayNextLine();
                    return;
                }
                
                // 检测鼠标点击（需要排除 UI）
                if (Input.GetMouseButtonDown(Constants.InputKeys.MouseButton))
                {
                    // 如果点击在 UI 上，不触发对话推进
                    if (IsPointerOverUI())
                        return;
                    
                    DisplayNextLine();
                }
            }
        }
    }
    
    // 检测鼠标是否在 UI 上
    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
            return false;
        
        return EventSystem.current.IsPointerOverGameObject();
    }

    void LoadStoryFromFile()
    {
        try
        {
            lineDict = DialogueDatabaseLoader.LoadFromResources(storyFilePath);
            Debug.Log($"{Constants.VNManagerTag} Loaded {lineDict.Count} dialogue lines from {storyFilePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"{Constants.VNManagerTag} Failed to load story: {e.Message}");
        }
    }

    void DisplayNextLine()
    {
        // 第一次调用：从起始ID开始
        if (currentLine == null)
        {
            if (!lineDict.TryGetValue(startId, out currentLine))
            {
                Debug.LogError($"{Constants.VNManagerTag} Start ID not found: {startId}");
                return;
            }
        }
        else
        {
            // 后续：根据 next_id 获取下一行
            if (string.IsNullOrEmpty(currentLine.next_id) || 
                currentLine.next_id.Equals(Constants.EndMarker, System.StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log($"{Constants.VNManagerTag} Story ended.");
                HandleStoryEnd();  // 根据故事类型处理结束逻辑
                return;
            }

            if (!lineDict.TryGetValue(currentLine.next_id, out currentLine))
            {
                Debug.LogError($"{Constants.VNManagerTag} Next line not found: {currentLine.next_id}");
                waitingForInput = false;
                return;
            }
        }

        // 显示当前行
        if (currentLine.type.Equals(Constants.DialogueType.Narration, System.StringComparison.OrdinalIgnoreCase))
        {
            // 旁白：隐藏说话人
            if (speakerText != null) speakerText.text = "";
            if (contentText != null) contentText.text = currentLine.content;
            
            // 旁白时隐藏角色立绘
            if (avatarImage != null)
                avatarImage.gameObject.SetActive(false);
        }
        else
        {
            // 对话：显示说话人和内容
            if (speakerText != null) speakerText.text = currentLine.DisplayName;
            if (contentText != null) contentText.text = currentLine.content;
            
            // 显示角色立绘
            ShowAvatar(currentLine.speaker, currentLine.expression);
        }
        
        // 加载背景图
        if (!string.IsNullOrEmpty(currentLine.bg))
        {
            LoadBackground(currentLine.bg);
        }

        // 更新游戏状态（保存当前进度）
        if (GameManager.I?.State != null)
        {
            GameManager.I.State.currentDialogueId = currentLine.id;
            Debug.Log($"{Constants.VNManagerTag} Updated progress: {currentLine.id}");
        }

        waitingForInput = true;
    }
    
    /// <summary>
    /// 加载并显示背景图
    /// </summary>
    private void LoadBackground(string bgName)
    {
        if (backgroundImage == null || string.IsNullOrEmpty(bgName))
            return;
        
        // 从 Resources/Backgrounds/ 加载
        Sprite bgSprite = Resources.Load<Sprite>($"Backgrounds/{bgName}");
        
        if (bgSprite != null)
        {
            backgroundImage.sprite = bgSprite;
            backgroundImage.color = Color.white;  // 确保可见
            Debug.Log($"{Constants.VNManagerTag} Background loaded: {bgName}");
        }
        else
        {
            Debug.LogWarning($"{Constants.VNManagerTag} Background not found: Backgrounds/{bgName}");
        }
    }
    
    /// <summary>
    /// 显示角色立绘
    /// </summary>
    private void ShowAvatar(string speaker, string expression)
    {
        if (avatarImage == null)
            return;
        
        // 如果没有说话人，隐藏立绘
        if (string.IsNullOrEmpty(speaker))
        {
            avatarImage.gameObject.SetActive(false);
            return;
        }
        
        // 映射 CSV 中的角色名到实际的资源文件名
        string avatarFileName = GetAvatarFileName(speaker);
        
        // 构建资源路径：Characters/角色名
        string avatarPath = $"Characters/{avatarFileName}";
        
        Sprite avatarSprite = Resources.Load<Sprite>(avatarPath);
        
        if (avatarSprite != null)
        {
            avatarImage.sprite = avatarSprite;
            avatarImage.color = activeAvatarColor;  // 使用设定的颜色
            avatarImage.gameObject.SetActive(true);
            
            Debug.Log($"{Constants.VNManagerTag} Avatar shown: {avatarPath}");
        }
        else
        {
            Debug.LogWarning($"{Constants.VNManagerTag} Avatar not found: {avatarPath}");
            // 如果找不到，尝试保持当前显示（不隐藏）
            // avatarImage.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// 将 CSV 中的角色名映射到资源文件名
    /// </summary>
    private string GetAvatarFileName(string speaker)
    {
        // 根据 CSV 中的说话人名字，返回对应的资源文件名
        switch (speaker)
        {
            case "林子月":
                return "林子月";
            
            case "唐沁":
                return "唐沁";
            
            case "师弟":
            case "师妹":
            case "同学":
            case "师弟师妹":      // 集体称呼
            case "师弟师妹们":    // 集体称呼（带"们"）
                return "npc";  // 师弟师妹等都使用 npc 立绘
            
            case "导师":
            case "唐某":
                return "教授";  // 导师使用教授立绘
            
            case "警察":
            case "狱警":        // 真结局中的狱警
                return "警察";
            
            default:
                // 如果没有匹配，返回原名字（可能找不到）
                Debug.LogWarning($"{Constants.VNManagerTag} Unknown speaker: {speaker}, using default 'npc'");
                return "npc";  // 默认使用 npc
        }
    }
    
    /// <summary>
    /// 处理故事结束 - 根据故事类型决定后续行为
    /// </summary>
    private void HandleStoryEnd()
    {
        // 检查是否是序章结束（需要跳转到 DotGamePlay）
        if (storyFilePath.Equals(Constants.StoryFiles.Prologue, System.StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log($"{Constants.VNManagerTag} Prologue ended. Going to DotGamePlay scene.");
            GoToDotGamePlay();
        }
        else
        {
            // 结局 CSV 结束，显示结局达成画面
            Debug.Log($"{Constants.VNManagerTag} Ending story ended. Showing ending achievement.");
            ShowEndingAchievement();
        }
    }
    
    /// <summary>
    /// 跳转到 DotGamePlay 场景
    /// </summary>
    private void GoToDotGamePlay()
    {
        waitingForInput = false;
        
        if (GameManager.I != null)
        {
            GameManager.I.LoadScene(Constants.Scenes.DotGamePlayDome1);
        }
        else
        {
            // Fallback：直接加载场景
            UnityEngine.SceneManagement.SceneManager.LoadScene(Constants.Scenes.DotGamePlayDome1);
        }
    }
    
    /// <summary>
    /// 显示结局达成画面
    /// </summary>
    private void ShowEndingAchievement()
    {
        // 1. 隐藏对话框
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        
        // 2. 显示结局达成面板
        if (endingAchievementPanel != null)
        {
            endingAchievementPanel.SetActive(true);
            
            // 3. 设置结局名称（从 currentLine.scene 字段获取）
            if (endingNameText != null && currentLine != null)
            {
                endingNameText.text = currentLine.scene;
                Debug.Log($"{Constants.VNManagerTag} Ending achievement shown: {currentLine.scene}");
            }
        }
        
        // 4. 标记为显示结局状态
        isShowingEnding = true;
        waitingForInput = true;  // 等待玩家点击
    }
    
    /// <summary>
    /// 返回主菜单
    /// </summary>
    private void ReturnToMainMenu()
    {
        Debug.Log($"{Constants.VNManagerTag} Returning to main menu...");
        
        if (GameManager.I != null)
        {
            GameManager.I.GoToMainMenu();
        }
        else
        {
            // Fallback：直接加载场景
            UnityEngine.SceneManagement.SceneManager.LoadScene(Constants.Scenes.MainMenu);
        }
    }
}
