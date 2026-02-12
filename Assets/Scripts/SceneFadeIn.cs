using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneFadeIn : MonoBehaviour
{
    [Tooltip("淡入持续时间（秒）")]
    public float fadeDuration = 1.5f;
    [Tooltip("全屏黑色面板")]
    public Image fadePanel;

    private void Start()
    {
        // 确保面板初始是全黑的
        if (fadePanel != null)
        {
            Color panelColor = fadePanel.color;
            panelColor.a = 1f;
            fadePanel.color = panelColor;

            // 开始淡入协程
            StartCoroutine(FadeInCoroutine());
        }
        else
        {
            Debug.LogError("没有FadePanel");
        }
    }

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
        //淡入完成后禁用面板
        fadePanel.gameObject.SetActive(false);
    }
}