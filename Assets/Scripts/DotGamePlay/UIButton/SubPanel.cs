using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubPanel : MonoBehaviour
{
    [SerializeField] private KeyWordOnline keyWordOnline;
    public Button subBtn;
    public Button jailBtn;

    [Header("UI显示配置")]
    [SerializeField] private TextMeshProUGUI tmpText;
    [SerializeField] private string tipText = "请说出chinese";

    // 嘲讽文案数组（整合所有文案）
    private string[] mockTexts = new string[]
    {
        "此路不通，逗号施工",
        "哎捧u你会说Chinese？",
        "断句如断粮，瞅给你饿的",
        "从识字开始重开还来得及",
        "666话都让你断哭了",
        "阁下断句，有如盲人摸象",
        "点击提交失去你的所有文化",
        "古人托梦：不是这么读",
        "歧义解析失败：用户全责",
        "？策划说就放个问号然后什么都不用说",
        "好醒目，好刺眼，好悲哀！"
    };

    private void Awake()
    {
        if (keyWordOnline == null)
        {
            keyWordOnline = FindObjectOfType<KeyWordOnline>();
        }
    }

    private void Start()
    {
        if (subBtn != null)
        {
            // 提交按钮用于激活关键词
            subBtn.onClick.AddListener(ActivateKeyWords);
        }
        else
        {
            Debug.LogError("SubPanel的提交按钮subBtn未赋值");
        }
        if(jailBtn != null)
        {
            subBtn.onClick.AddListener(() =>
            {
                GameManager.I.GoToTrueEndingWind();
            });
        }

        if (tmpText != null)
        {
            tmpText.gameObject.SetActive(false);
            tmpText.text = tipText;
        }
        else
        {
            Debug.LogError("SubPanel的显示文本tmpText未赋值");
        }
    }

    /// <summary>
    /// 仅激活对应剧情的关键词
    /// </summary>
    public void ActivateKeyWords()
    {
        if (keyWordOnline == null)
        {
            Debug.LogError("SubPanel未找到KeyWordOnline组件，无法激活关键词");
            return;
        }

        PlotType triggeredPlot = keyWordOnline.CheckAndAwakePlot();

        // 仅显示提示文本，不再验证剧情
        if (triggeredPlot != PlotType.None)
        {
            tmpText.text = "请将所有关键词拖入笔录";
            Debug.Log($"已激活{triggeredPlot}剧情的关键词，请将所有关键词拖入剧情区");
        }
        else
        {
            // 随机获取嘲讽文案并显示
            tmpText.text = GetRandomMockText();
            tmpText.gameObject.SetActive(true);
            Debug.Log("未触发任何剧情，显示提示");
        }
    }


    private string GetRandomMockText()
    {
        int randomIndex = Random.Range(0, mockTexts.Length);
        return mockTexts[randomIndex];
    }
}