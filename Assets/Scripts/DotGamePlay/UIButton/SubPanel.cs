using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SubPanel : MonoBehaviour
{
    //用于判断何时激活关键词
    [SerializeField] private KeyWordOnline keyWordOnline;
    public Button subBtn;
    [Header("UI提示配置")]
    [SerializeField] private TextMeshProUGUI tmpText;
    [SerializeField] private string tipText = "你会说chinese吗"; // 可配置：未触发剧情的提示文本
    private void Awake()
    {
        // 自动查找KeyWordOnline并校验
        if (keyWordOnline == null)
        {
            keyWordOnline = FindObjectOfType<KeyWordOnline>();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (subBtn != null)
        {
            subBtn.onClick.AddListener(CheckPlot);
        }
        else
        {
            Debug.LogError("SubPanel：触发按钮subBtn未关联！");
        }
        if (tmpText != null)
        {
            tmpText.gameObject.SetActive(false);
            tmpText.text = tipText; // 加载可配置的提示文本
        }
        else
        {
            Debug.LogError("SubPanel：提示文本tmpText未关联！");
        }
    }
    /// <summary>
    /// 检查并激活剧情分支
    /// </summary>
    public void CheckPlot()
    {
        if (keyWordOnline == null)
        {
            Debug.LogError("SubPanel：未找到KeyWordOnline组件，无法检测剧情！");
            return;
        }
        // 启动动态检测协程
        keyWordOnline.StartDynamicCheck();
        Debug.Log("动态检测协程已启动");
        // 检查是否触发剧情关键词
        PlotType triggeredPlot = keyWordOnline.CheckAndAwakePlot();
        if (triggeredPlot != PlotType.None)
        {
            Debug.Log($"触发剧情类型：{triggeredPlot}，对应关键词已变红可拖动");
            tmpText.gameObject.SetActive(false);
        }
        else
        {
            tmpText.gameObject.SetActive(true);
            Debug.Log("未触发任何剧情，显示提示文本");
        }
    }
}