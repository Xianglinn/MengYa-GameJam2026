using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 游戏启动图片播放面板控制器
/// 功能：依次播放两张启动图片（各3秒），播放完毕后隐藏图片面板，显示主页面
/// </summary>
public class StartImagePanel : MonoBehaviour
{
    [Header("图片配置")]
    [Tooltip("第一张启动图片")]
    public Sprite firstImage;
    [Tooltip("第二张启动图片")]
    public Sprite secondImage;
    [Tooltip("用于显示图片的Image组件")]
    public Image displayImage;

    [Header("面板配置")]
    [Tooltip("启动图片的父面板（播放完毕后失活）")]
    public GameObject startImagePanel;
    [Tooltip("游戏主页面面板（播放完毕后激活）")]
    public GameObject mainGamePanel;

    [Header("时间配置")]
    [Tooltip("每张图片显示时长（秒）")]
    public float imageShowDuration = 3f;

    private void Start()
    {
        // 初始化面板状态：显示图片面板，隐藏主页面
        if (startImagePanel != null) startImagePanel.SetActive(true);
        if (mainGamePanel != null) mainGamePanel.SetActive(false);

        // 检查必要组件是否赋值
        if (displayImage == null || firstImage == null || secondImage == null)
        {
            Debug.LogError("请在Inspector面板中为启动图片控制器赋值必要的图片/组件！");
            return;
        }

        // 启动协程播放图片序列
        StartCoroutine(PlayStartImageSequence());
    }

    /// <summary>
    /// 协程：依次播放两张启动图片，控制时长和面板切换
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayStartImageSequence()
    {
        // 第一步：显示第一张图片，等待指定时长
        displayImage.sprite = firstImage;
        yield return new WaitForSeconds(imageShowDuration);

        // 第二步：显示第二张图片，等待指定时长
        displayImage.sprite = secondImage;
        yield return new WaitForSeconds(imageShowDuration);

        // 确保面板初始是全黑的
        if (fadePanel != null)
        {
            Color panelColor = fadePanel.color;
            panelColor.a = 1f;
            fadePanel.color = panelColor;

            // 开始淡入协程
            StartCoroutine(FadeInCoroutine());
            StopCoroutine(PlayStartImageSequence());
        }
        else
        {
            Debug.LogError("没有FadePanel");
        }

    }


    [Tooltip("全屏黑色面板")]
    public Image fadePanel;
    [Tooltip("淡入持续时间（秒）")]
    public float fadeDuration = 1.5f;
    /// <summary>
    /// 淡入协程：控制面板透明度从1降到0
    /// </summary>
    private IEnumerator FadeInCoroutine()
    {
        float elapsedTime = 0f;
        Color startColor = fadePanel.color;

        while (elapsedTime < fadeDuration)
        {

            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            startColor.a = alpha;
            fadePanel.color = startColor;


            elapsedTime += Time.deltaTime;
            yield return null; // 等待下一帧
        }

        //确保最终透明度为0
        startColor.a = 0f;
        fadePanel.color = startColor;
        // 第三步：播放完毕，失活图片面板，激活主页面
        if (startImagePanel != null) startImagePanel.SetActive(false);
        if (mainGamePanel != null) mainGamePanel.SetActive(true);

        Destroy(this);

    }

}