using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text contentText;
    [SerializeField] private string storyFilePath = Constants.DefaultStoryPath;
    [SerializeField] private string startId = Constants.DefaultStartId;

    private Dictionary<string, DialogueLine> lineDict;
    private DialogueLine currentLine;
    private bool waitingForInput = false;

    void Start()
    {
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
                }
                else
                {
                    if (speakerText != null) speakerText.text = currentLine.DisplayName;
                    if (contentText != null) contentText.text = currentLine.content;
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
                waitingForInput = false;
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
        }
        else
        {
            // 对话：显示说话人和内容
            if (speakerText != null) speakerText.text = currentLine.DisplayName;
            if (contentText != null) contentText.text = currentLine.content;
        }

        // 更新游戏状态（保存当前进度）
        if (GameManager.I?.State != null)
        {
            GameManager.I.State.currentDialogueId = currentLine.id;
            Debug.Log($"{Constants.VNManagerTag} Updated progress: {currentLine.id}");
        }

        waitingForInput = true;
    }
}
