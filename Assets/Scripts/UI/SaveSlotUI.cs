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
    [SerializeField] private TMP_Text chapterText;        // 章节名称（显示 CSV 的 scene）
    [SerializeField] private TMP_Text contentPreviewText; // 对话预览（前 7 个字 + "......"）
    [SerializeField] private TMP_Text saveTimeText;       // 保存时间
    [SerializeField] private GameObject emptySlotPanel;   // 空槽位提示
    [SerializeField] private GameObject dataPanel;        // 有数据时显示的面板
    [SerializeField] private Button slotButton;           // 槽位按钮
    
    [Header("Mode Sprites")]
    [SerializeField] private Sprite saveSprite;           // 保存模式图标
    [SerializeField] private Sprite loadSprite;           // 加载模式图标
    [SerializeField] private Image buttonImage;           // 按钮的 Image 组件（用于切换图标）

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
        
        // 如果没有手动指定 buttonImage，尝试自动获取
        if (buttonImage == null)
            buttonImage = GetComponent<Image>();

        RefreshDisplay();
    }
    
    /// <summary>
    /// 根据面板模式设置按钮图标
    /// </summary>
    public void SetMode(SavePanelManager.SavePanelMode mode)
    {
        if (buttonImage == null)
        {
            Debug.LogWarning($"[SaveSlotUI] Button Image not assigned on slot {slotIndex}");
            return;
        }
        
        // 根据模式切换图标
        if (mode == SavePanelManager.SavePanelMode.Save)
        {
            if (saveSprite != null)
            {
                buttonImage.sprite = saveSprite;
                Debug.Log($"[SaveSlotUI] Slot {slotIndex} switched to Save sprite");
            }
        }
        else
        {
            if (loadSprite != null)
            {
                buttonImage.sprite = loadSprite;
                Debug.Log($"[SaveSlotUI] Slot {slotIndex} switched to Load sprite");
            }
        }
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

            // 显示章节名称（直接显示 CSV 的 scene 字段）
            if (chapterText != null)
                chapterText.text = info.sceneName;

            // 显示对话内容预览（前 7 个字 + "......"）
            if (contentPreviewText != null)
                contentPreviewText.text = info.contentPreview;

            // 显示保存时间
            if (saveTimeText != null)
                saveTimeText.text = FormatSaveTime(info.GetSaveDateTime());
        }
    }

    private void OnButtonClick()
    {
        onSlotClicked?.Invoke(slotIndex);
    }

    private string FormatSaveTime(DateTime time)
    {
        // 直接显示绝对时间
        return time.ToString("yyyy/MM/dd HH:mm");
    }

    public bool IsEmpty => isEmpty;
}
