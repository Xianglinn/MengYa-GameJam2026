using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ComicsPanel : MonoBehaviour
{
    [Header("UI Components")]
    public Image targetImage;

    public bool onlyRespondAfterImageSet = true;

    private bool isImageSet = false;
    private bool isWaitingForInput = true;

    public GameObject gameObj1;
    public GameObject gameObj2;
    public GameObject gameObj3;

    private PlotType currentActivePlot = PlotType.None;

    [SerializeField] private KeyWordOnline keyWordOnline;

    [Header("UI Panel")]
    public GameObject panelRoot;

    [Header("Animation Settings")]
    public float fadeInDuration = 1.5f;
    public bool enableSlideIn = true;
    public float slideOffsetY = 200f;

    private bool isAnimating = false;
    private Vector2 originalPos;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        if (targetImage == null)
        {
            Debug.LogError("Target Image not assigned in Inspector", this);
            isWaitingForInput = false;
        }

        if (panelRoot != null)
        {
            canvasGroup = panelRoot.GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = panelRoot.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            panelRoot.SetActive(true);

            if (enableSlideIn)
            {
                RectTransform panelRect = panelRoot.GetComponent<RectTransform>();
                originalPos = panelRect.anchoredPosition;
                panelRect.anchoredPosition = new Vector2(originalPos.x, originalPos.y - slideOffsetY);
            }
        }
        else if (targetImage != null)
        {
            Color imgColor = targetImage.color;
            imgColor.a = 0f;
            targetImage.color = imgColor;
            targetImage.gameObject.SetActive(true);
        }

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isWaitingForInput &&
            (!onlyRespondAfterImageSet || isImageSet))
        {
            if (Input.anyKeyDown)
            {
                SwitchToTargetScene(currentActivePlot);
            }
        }
    }

    public void SetDisplayImage(Sprite imageSprite)
    {
        gameObject.SetActive(true);
        gameObj1.gameObject.SetActive(false);
        gameObj2.gameObject.SetActive(false);
        gameObj3.gameObject.SetActive(false);

        currentActivePlot = keyWordOnline.CurrentActivatedPlot;

        if (targetImage == null || isAnimating) return;

        if (imageSprite == null)
        {
            Debug.LogWarning("Image sprite is null", this);
            return;
        }

        if (targetImage == null)
        {
            Debug.LogError("Target Image not assigned", this);
            return;
        }

        if (imageSprite == null)
        {
            Debug.LogWarning("Image sprite is null", this);
            return;
        }

        targetImage.sprite = imageSprite;
        targetImage.gameObject.SetActive(true);
        isImageSet = true;

        Debug.Log("Image set successfully", this);

        StartCoroutine(ShowPanelWithAnimation());
    }


    private IEnumerator ShowPanelWithAnimation()
    {
        isAnimating = true;
        float elapsedTime = 0f;

        while (elapsedTime < fadeInDuration)
        {
            float progress = elapsedTime / fadeInDuration;
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

            if (canvasGroup != null) canvasGroup.alpha = smoothProgress;
            else if (targetImage != null)
            {
                Color imgColor = targetImage.color;
                imgColor.a = smoothProgress;
                targetImage.color = imgColor;
            }

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
        Debug.Log("Animation completed", this);
    }

    private void SwitchToTargetScene(PlotType plotType)
    {
        isWaitingForInput = false;
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
