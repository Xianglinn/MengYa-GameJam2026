using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubPanel : MonoBehaviour
{
    [SerializeField] private KeyWordOnline keyWordOnline;
    public Button subBtn;
    [Header("UI显示配置")]
    [SerializeField] private TextMeshProUGUI tmpText;
    [SerializeField] private string tipText = "请说出chinese";

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
    /// 仅激活对应剧情的关键词（不再触发剧情验证）
    /// </summary>
    public void ActivateKeyWords()
    {
        if (keyWordOnline == null)
        {
            Debug.LogError("SubPanel未找到KeyWordOnline组件，无法激活关键词");
            return;
        }

        // 启动动态检测（激活对应剧情的关键词）
        keyWordOnline.StartDynamicCheck();
        PlotType triggeredPlot = keyWordOnline.CheckAndAwakePlot();

        // 仅显示提示文本，不再验证剧情
        if (triggeredPlot != PlotType.None)
        {
            tmpText.gameObject.SetActive(false);
            Debug.Log($"已激活{triggeredPlot}剧情的关键词，请将所有关键词拖入剧情区");
        }
        else
        {
            tmpText.gameObject.SetActive(true);
            tmpText.text = tipText;
            Debug.Log("未触发任何剧情，显示提示文本");
        }
    }
}