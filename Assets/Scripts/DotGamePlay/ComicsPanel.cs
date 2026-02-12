using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static UnityEditor.MaterialProperty;
using System.Collections;

/// <summary>
/// 显示外部传入的图片，并支持按键跳转场景
/// 挂载到包含Image组件的UI面板上
/// </summary>
public class ComicsPanel : MonoBehaviour
{
    [Header("UI组件关联")]
    [Tooltip("用于显示图片的Image组件")]
    public Image targetImage; // 关联UI面板中的Image

    [Tooltip("是否仅在图片显示后响应按键")]
    public bool onlyRespondAfterImageSet = true;

    private bool isImageSet = false; // 标记是否已传入并显示图片
    private bool isWaitingForInput = true; // 标记是否等待按键输入

    public GameObject gameObj1;
    public GameObject gameObj2;
    public GameObject gameObj3;

    // 当前激活的剧情类型（从KeyWordOnline获取）
    private PlotType currentActivePlot = PlotType.None;

    [SerializeField] private KeyWordOnline keyWordOnline; // 关联关键词管理器

    [Header("UI组件关联")]
    public GameObject panelRoot; // 新增：面板根物体（控制整个面板动画）

    [Header("缓慢出现动画配置")]
    public float fadeInDuration = 1.5f; // 新增：淡入动画时长
    public bool enableSlideIn = true; // 新增：是否启用滑入动画
    public float slideOffsetY = 200f; // 新增：滑入初始偏移量

    private bool isAnimating = false; // 新增：标记是否正在播放动画
    private Vector2 originalPos; // 新增：面板原始位置（滑入动画用）
    private CanvasGroup canvasGroup; // 新增：控制整体透明度的组件

    private void Start()
    {
        // 初始化检查：确保Image组件已关联
        if (targetImage == null)
        {
            Debug.LogError("未关联Target Image组件！请在Inspector面板中绑定UI上的Image", this);
            isWaitingForInput = false; // 未配置则不响应按键
        }

        // 【新增】初始化CanvasGroup和动画初始状态
        if (panelRoot != null)
        {
            canvasGroup = panelRoot.GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = panelRoot.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f; // 初始透明
            panelRoot.SetActive(true);

            // 【新增】滑入动画：记录原始位置，设置初始偏移
            if (enableSlideIn)
            {
                RectTransform panelRect = panelRoot.GetComponent<RectTransform>();
                originalPos = panelRect.anchoredPosition;
                panelRect.anchoredPosition = new Vector2(originalPos.x, originalPos.y - slideOffsetY);
            }
        }
        else if (targetImage != null)
        {
            // 【新增】无面板时，直接隐藏Image透明度
            Color imgColor = targetImage.color;
            imgColor.a = 0f;
            targetImage.color = imgColor;
            targetImage.gameObject.SetActive(true);
        }

        gameObject.SetActive(false);
        
    }

    private void Update()
    {
        // 仅在允许输入、图片已设置（可选）、有目标场景名时响应按键
        if (isWaitingForInput &&
            (!onlyRespondAfterImageSet || isImageSet))
        {
            // 检测任意按键（包括鼠标键）
            if (Input.anyKeyDown)
            {
                SwitchToTargetScene(currentActivePlot);
            }
        }
    }

    /// <summary>
    /// 供外部脚本调用：设置要显示的图片
    /// </summary>
    /// <param name="imageSprite">要显示的Sprite图片数据</param>
    public void SetDisplayImage(Sprite imageSprite)
    {
        gameObject.SetActive(true);
        gameObj1.gameObject.SetActive(false);
        gameObj2.gameObject.SetActive(false);
        gameObj3.gameObject.SetActive(false);

        // 获取当前激活的剧情类型
        currentActivePlot = keyWordOnline.CurrentActivatedPlot;

        // 【新增】增加动画状态校验：动画中不重复触发
        if (targetImage == null || isAnimating) return;

        if (imageSprite == null)
        {
            Debug.LogWarning("传入的图片数据为空", this);
            return;
        }

        if (targetImage == null)
        {
            Debug.LogError("Target Image未绑定，无法显示图片", this);
            return;
        }

        if (imageSprite == null)
        {
            Debug.LogWarning("传入的图片数据为空", this);
            return;
        }

        // 显示图片并标记状态
        targetImage.sprite = imageSprite;
        targetImage.gameObject.SetActive(true);
        isImageSet = true;

        Debug.Log("图片已成功显示", this);

        // 【新增】启动缓慢出现动画
        StartCoroutine(ShowPanelWithAnimation());
    }


    private IEnumerator ShowPanelWithAnimation()
    {
        isAnimating = true;
        float elapsedTime = 0f;

        while (elapsedTime < fadeInDuration)
        {
            float progress = elapsedTime / fadeInDuration;
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress); // 顺滑插值

            // 【新增】调整透明度
            if (canvasGroup != null) canvasGroup.alpha = smoothProgress;
            else if (targetImage != null)
            {
                Color imgColor = targetImage.color;
                imgColor.a = smoothProgress;
                targetImage.color = imgColor;
            }

            // 【新增】调整位置（滑入动画）
            if (enableSlideIn && panelRoot != null)
            {
                RectTransform panelRect = panelRoot.GetComponent<RectTransform>();
                panelRect.anchoredPosition = Vector2.Lerp(
                    new Vector2(originalPos.x, originalPos.y - slideOffsetY),
                    originalPos,
                    smoothProgress
                );
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 【新增】动画结束：确保完全显示
        if (canvasGroup != null) canvasGroup.alpha = 1f;
        else if (targetImage != null)
        {
            Color imgColor = targetImage.color;
            imgColor.a = 1f;
            targetImage.color = imgColor;
        }
        if (enableSlideIn && panelRoot != null)
        {
            panelRoot.GetComponent<RectTransform>().anchoredPosition = originalPos;
        }

        isAnimating = false;
        Debug.Log("面板已缓慢显示完成", this);
    }

    /// <summary>
    /// 跳转到目标场景
    /// </summary>
    private void SwitchToTargetScene(PlotType plotType)
    {
        isWaitingForInput = false; // 防止重复触发跳转
        string currentSceneName = gameObject.scene.name;

        switch (plotType)
        {
            case PlotType.APlot:
                if (currentSceneName == "DotGamePlayDome 1")
                {
                    GameManager.I.GoToBadEndingFailed();
                }
                else if (currentSceneName == "DotGamePlayDome 2")
                {
                    GameManager.I.GoToNormalEndingDrifting();
                }
                else
                {
                    GameManager.I.GoToBadEndingFailed();
                }
                break;

            case PlotType.BPlot:
                if (currentSceneName == "DotGamePlayDome 1")
                {
                    GameManager.I.LoadScene("DotGamePlayDome 2");
                }
                else if (currentSceneName == "DotGamePlayDome 2")
                {
                    GameManager.I.LoadScene("DotGamePlayDome 3");
                }
                else
                {
                    GameManager.I.GoToHappyEndingJadeBlessed();
                }
                break;
        }

    }
}