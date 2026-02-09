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

    [SerializeField] private TextMeshProUGUI tmpText;

    private void Awake()
    {
        // 自动查找KeyWordOnline并校验
        if (keyWordOnline == null)
        {
            //未找到KeyWordOnline组件，剧情激活功能失效！
            keyWordOnline = FindObjectOfType<KeyWordOnline>();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        subBtn.onClick.AddListener(CheckPlot);
        tmpText.gameObject.SetActive(false);
    }

    /// <summary>
    /// 检查并激活剧情分支
    /// </summary>
    public void CheckPlot()
    {
        // 启动动态检测协程
        keyWordOnline.StartDynamicCheck();
        Debug.Log("动态检测协程已启动");

        // 检查是否触发剧情关键词
        if (keyWordOnline != null)
        {
            PlotType triggeredPlot = keyWordOnline.CheckAndAwakePlot();
            if (triggeredPlot != PlotType.None)
            {
                Debug.Log($"触发剧情类型：{triggeredPlot}，对应关键词已变红可拖动");
                //调用对话系统
                tmpText.gameObject.SetActive(false);
            }
            else
            {
                //显示你会说chinese吗，调用对话系统
                tmpText.gameObject.SetActive(true);
            }
        }

    }



}
