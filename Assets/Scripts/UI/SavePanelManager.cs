using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 存档面板管理器
/// 处理存档的保存和加载
/// </summary>
public class SavePanelManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject savePanelRoot;
    [SerializeField] private Button closeButton;
    [SerializeField] private SaveSlotUI[] saveSlots;

    [Header("Mode Text")]
    [SerializeField] private TMPro.TMP_Text modeText;  // 显示"保存游戏"或"加载游戏"

    private SavePanelMode currentMode;

    public enum SavePanelMode
    {
        Save,    // 保存模式（游戏内）
        Load     // 加载模式（主菜单）
    }

    private void Awake()
    {
        // 初始化关闭按钮
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseSavePanel);

        // 初始化存档槽位
        for (int i = 0; i < saveSlots.Length; i++)
        {
            if (saveSlots[i] != null)
                saveSlots[i].Initialize(i, OnSlotClicked);
        }

        // 默认隐藏
        if (savePanelRoot != null)
            savePanelRoot.SetActive(false);
    }

    /// <summary>
    /// 打开存档面板
    /// </summary>
    /// <param name="mode">保存模式或加载模式</param>
    public void OpenSavePanel(SavePanelMode mode)
    {
        currentMode = mode;

        // 显示面板
        if (savePanelRoot != null)
            savePanelRoot.SetActive(true);

        // 更新模式文本
        if (modeText != null)
        {
            modeText.text = mode == SavePanelMode.Save ? "保存游戏" : "加载游戏";
        }

        // 刷新所有槽位显示
        RefreshAllSlots();

        Debug.Log($"[SavePanel] Opened in {mode} mode");
    }

    /// <summary>
    /// 关闭存档面板
    /// </summary>
    public void CloseSavePanel()
    {
        if (savePanelRoot != null)
            savePanelRoot.SetActive(false);

        Debug.Log("[SavePanel] Closed");
    }

    /// <summary>
    /// 刷新所有槽位显示
    /// </summary>
    private void RefreshAllSlots()
    {
        foreach (var slot in saveSlots)
        {
            if (slot != null)
                slot.RefreshDisplay();
        }
    }

    /// <summary>
    /// 槽位被点击
    /// </summary>
    private void OnSlotClicked(int slotIndex)
    {
        if (currentMode == SavePanelMode.Save)
        {
            // 保存模式
            OnSaveToSlot(slotIndex);
        }
        else
        {
            // 加载模式
            OnLoadFromSlot(slotIndex);
        }
    }

    /// <summary>
    /// 保存到指定槽位
    /// </summary>
    private void OnSaveToSlot(int slotIndex)
    {
        if (GameManager.I == null || GameManager.I.State == null)
        {
            Debug.LogError("[SavePanel] Cannot save: No active game state");
            return;
        }

        // 保存游戏
        GameManager.I.Save(slotIndex);

        // 刷新槽位显示
        if (slotIndex >= 0 && slotIndex < saveSlots.Length)
            saveSlots[slotIndex].RefreshDisplay();

        Debug.Log($"[SavePanel] Saved to slot {slotIndex}");

        // 可选：显示保存成功提示
        // ShowSaveSuccessMessage();

        // 可选：自动关闭面板
        // CloseSavePanel();
    }

    /// <summary>
    /// 从指定槽位加载
    /// </summary>
    private void OnLoadFromSlot(int slotIndex)
    {
        // 检查槽位是否为空
        if (!SaveSystem.HasSave(slotIndex))
        {
            Debug.LogWarning($"[SavePanel] Slot {slotIndex} is empty");
            return;
        }

        // 加载游戏
        if (GameManager.I.TryLoad(slotIndex))
        {
            Debug.Log($"[SavePanel] Loaded from slot {slotIndex}");

            // 关闭面板
            CloseSavePanel();

            // 跳转到游戏场景
            string sceneName = GameManager.I.State.currentScene;
            GameManager.I.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"[SavePanel] Failed to load from slot {slotIndex}");
        }
    }
}
