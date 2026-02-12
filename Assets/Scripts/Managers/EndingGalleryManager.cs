using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 结局图鉴管理器：显示已达成的结局图（end1~4）或未达成图（unlock）
/// </summary>
public class EndingGalleryManager : MonoBehaviour
{
    [Header("Ending Images")]
    [SerializeField] private Image ending1Image;  // 对应 end1
    [SerializeField] private Image ending2Image;  // 对应 end2
    [SerializeField] private Image ending3Image;  // 对应 end3
    [SerializeField] private Image ending4Image;  // 对应 end4

    [Header("Back Button")]
    [SerializeField] private Button backButton;

    private void Start()
    {
        // 绑定返回按钮
        if (backButton != null)
            backButton.onClick.AddListener(OnBackToMainMenu);

        // 加载并设置四个结局图
        RefreshEndingDisplay();
    }

    /// <summary>
    /// 刷新结局图显示：已达成显示 end1~4，未达成显示 unlock
    /// </summary>
    private void RefreshEndingDisplay()
    {
        SetEndingSprite(ending1Image, Constants.EndingSpriteNames.End1);
        SetEndingSprite(ending2Image, Constants.EndingSpriteNames.End2);
        SetEndingSprite(ending3Image, Constants.EndingSpriteNames.End3);
        SetEndingSprite(ending4Image, Constants.EndingSpriteNames.End4);

        // 可选：显示达成数量
        int achievedCount = EndingRegistry.Count;
        Debug.Log($"[EndingGallery] Player has achieved {achievedCount}/4 endings");
    }

    /// <summary>
    /// 根据是否达成设置 Image 的 Sprite
    /// </summary>
    private void SetEndingSprite(Image targetImage, string endingId)
    {
        if (targetImage == null) return;

        // 检查是否已达成
        bool isAchieved = EndingRegistry.Has(endingId);
        string spriteName = isAchieved ? endingId : Constants.EndingSpriteNames.Unlock;

        // 从 Resources/Backgrounds/ 加载 Sprite
        Sprite sprite = Resources.Load<Sprite>($"Backgrounds/{spriteName}");

        if (sprite != null)
        {
            targetImage.sprite = sprite;
            targetImage.color = Color.white;  // 确保可见
        }
        else
        {
            Debug.LogWarning($"[EndingGallery] Sprite not found: Backgrounds/{spriteName}");
        }
    }

    /// <summary>
    /// 返回主菜单
    /// </summary>
    private void OnBackToMainMenu()
    {
        Debug.Log("[EndingGallery] Returning to main menu...");
        
        if (GameManager.I != null)
        {
            GameManager.I.GoToMainMenu();
        }
        else
        {
            SceneManager.LoadScene(Constants.Scenes.MainMenu);
        }
    }
}
