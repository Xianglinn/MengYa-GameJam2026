using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// 单个存档槽位的 UI 控制
/// </summary>
public class SaveSlotUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text slotNumberText;     // 槽位编号 "槽位 1"
    [SerializeField] private TMP_Text chapterText;        // 章节名称
    [SerializeField] private TMP_Text moneyText;          // 金钱
    [SerializeField] private TMP_Text saveTimeText;       // 保存时间
    [SerializeField] private GameObject emptySlotPanel;   // 空槽位提示
    [SerializeField] private GameObject dataPanel;        // 有数据时显示的面板
    [SerializeField] private Button slotButton;           // 槽位按钮

    private int slotIndex;
    private bool isEmpty;
    private System.Action<int> onSlotClicked;

    public void Initialize(int index, System.Action<int> callback)
    {
        slotIndex = index;
        onSlotClicked = callback;

        if (slotButton != null)
            slotButton.onClick.AddListener(OnButtonClick);

        if (slotNumberText != null)
            slotNumberText.text = $"槽位 {index + 1}";

        RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        SaveInfo info = SaveSystem.GetSaveInfo(slotIndex);

        if (info == null)
        {
            // 空槽位
            isEmpty = true;
            if (emptySlotPanel) emptySlotPanel.SetActive(true);
            if (dataPanel) dataPanel.SetActive(false);
        }
        else
        {
            // 有存档数据
            isEmpty = false;
            if (emptySlotPanel) emptySlotPanel.SetActive(false);
            if (dataPanel) dataPanel.SetActive(true);

            // 显示章节名称（场景名转换为友好名称）
            if (chapterText != null)
                chapterText.text = GetChapterName(info.currentScene);

            // 显示金钱
            if (moneyText != null)
            {
                // 需要从 SaveSystem 加载完整的 GameState 来获取金钱
                GameState state = SaveSystem.Load(slotIndex);
                moneyText.text = state != null ? $"💰 {state.money}" : "💰 0";
            }

            // 显示保存时间
            if (saveTimeText != null)
                saveTimeText.text = FormatSaveTime(info.saveTime);
        }
    }

    private void OnButtonClick()
    {
        onSlotClicked?.Invoke(slotIndex);
    }

    private string GetChapterName(string sceneName)
    {
        // 将场景名转换为友好的章节名称
        switch (sceneName)
        {
            case "MainMenu": return "主菜单";
            case "Prologue": return "序章";
            case "Chapter1": return "第一章";
            case "Chapter2": return "第二章";
            default: return sceneName;
        }
    }

    private string FormatSaveTime(DateTime time)
    {
        TimeSpan diff = DateTime.Now - time;

        if (diff.TotalMinutes < 1)
            return "刚刚";
        else if (diff.TotalHours < 1)
            return $"{(int)diff.TotalMinutes} 分钟前";
        else if (diff.TotalDays < 1)
            return $"{(int)diff.TotalHours} 小时前";
        else if (diff.TotalDays < 7)
            return $"{(int)diff.TotalDays} 天前";
        else
            return time.ToString("yyyy/MM/dd HH:mm");
    }

    public bool IsEmpty => isEmpty;
}
