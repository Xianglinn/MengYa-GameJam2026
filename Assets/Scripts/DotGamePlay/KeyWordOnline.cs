using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 剧情分支枚举
public enum PlotType
{
    None = 0,
    APlot = 1,
    BPlot = 2
}

public class KeyWordOnline : MonoBehaviour
{
    [SerializeField] private DotSolts solts;
    private bool isKeyWordActivated = false;
    private PlotType currentActivatedPlot = PlotType.None;

    [Header("剧情关键词配置")]
    [SerializeField] private GameObject aKeys;
    [SerializeField] private GameObject bKeys;
    [SerializeField] private float checkInterval = 0.1f; // 动态检测频率（可调整）
    private Coroutine checkCoroutine; // 协程引用

    void Start()
    {
        // 初始化DotSolts并校验
        if (solts == null)
        {
            solts = GameObject.Find("DotSolts")?.GetComponent<DotSolts>();
            if (solts == null)
            {
                Debug.LogError("场景中未找到DotSolts对象");
                return;
            }
        }

        // 校验关键词对象关联
        if (aKeys == null) Debug.LogError("A分支关键词（aKeys）未关联！");
        if (bKeys == null) Debug.LogError("B分支关键词（bKeys）未关联！");

        // 校验KeyWord组件
        if (aKeys != null && aKeys.GetComponent<KeyWord>() == null)
            Debug.LogError("aKeys对象未挂载KeyWord组件！");
        if (bKeys != null && bKeys.GetComponent<KeyWord>() == null)
            Debug.LogError("bKeys对象未挂载KeyWord组件！");

        // 启动动态检测协程
        StartDynamicCheck();
        Debug.Log("动态检测协程已启动");
    }

    /// <summary>
    /// 启动动态检测协程
    /// </summary>
    public void StartDynamicCheck()
    {
        if (checkCoroutine != null) StopCoroutine(checkCoroutine);
        checkCoroutine = StartCoroutine(CheckPlotConditionCoroutine());
    }

    /// <summary>
    /// 停止动态检测协程
    /// </summary>
    public void StopDynamicCheck()
    {
        if (checkCoroutine != null)
        {
            StopCoroutine(checkCoroutine);
            checkCoroutine = null;
            Debug.Log("KeyWordOnline：动态检测协程已停止");
        }
    }

    /// <summary>
    /// 定时检查剧情条件协程
    /// </summary>
    private IEnumerator CheckPlotConditionCoroutine()
    {
        while (true)
        {
            if (!isKeyWordActivated && solts != null && solts.slotOccupancy != null)
            {
                CheckAndAwakePlot();
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }

    /// <summary>
    /// 检查并激活剧情分支
    /// </summary>
    public PlotType CheckAndAwakePlot()
    {
        // 已激活则直接返回
        if (isKeyWordActivated)
        {
            return PlotType.None;
        }

        // 核心依赖为空则返回
        if (solts == null || solts.slotOccupancy == null) return PlotType.None;

        // 替换为实际格子名称（必须修改！）
        bool isAPlotValid = CheckSlotsOccupied(new List<string> { "DotObj (1)", "DotObj (2)", "DotObj (3)" });
        if (isAPlotValid)
        {
            aKeys.GetComponent<KeyWord>().SetRed();
            isKeyWordActivated = true;
            currentActivatedPlot = PlotType.APlot;
            Debug.Log("KeyWordOnline：A剧情已激活！关键词变红可拖拽");
            StopDynamicCheck();
            return PlotType.APlot;
        }

        // 替换为实际格子名称（必须修改！）
        bool isBPlotValid = CheckSlotsOccupied(new List<string> { "DotObj (1)", "DotObj (2)" });
        if (isBPlotValid)
        {
            bKeys.GetComponent<KeyWord>().SetRed();
            isKeyWordActivated = true;
            currentActivatedPlot = PlotType.BPlot;
            Debug.Log("KeyWordOnline：B剧情已激活！关键词变红可拖拽");
            StopDynamicCheck();
            return PlotType.BPlot;
        }

        return PlotType.None;
    }

    /// <summary>
    /// 检查指定传入格子是否全部被占用
    /// </summary>
    private bool CheckSlotsOccupied(List<string> slotNames)
    {
        foreach (string slotName in slotNames)
        {
            if (!solts.slotOccupancy.ContainsKey(slotName))
            {              
                return false;
            }
            if (solts.slotOccupancy[slotName] == null)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 重置关键词状态（剧情重新开始/切换时调用）
    /// </summary>
    public void ResetKeyWordState()
    {
        isKeyWordActivated = false;
        currentActivatedPlot = PlotType.None;

        // 重置A分支关键词
        if (aKeys != null)
        {
            KeyWord aKeyComp = aKeys.GetComponent<KeyWord>();
            if (aKeyComp != null)
            {
                aKeyComp.ResetKeyWord();
                Debug.Log("A分支关键词已重置");
            }
            else
            {
                Debug.LogWarning("aKeys对象未挂载KeyWord组件，无法重置！");
            }
        }

        // 重置B分支关键词
        if (bKeys != null)
        {
            KeyWord bKeyComp = bKeys.GetComponent<KeyWord>();
            if (bKeyComp != null)
            {
                bKeyComp.ResetKeyWord();
                Debug.Log("B分支关键词已重置");
            }
            else
            {
                Debug.LogWarning("bKeys对象未挂载KeyWord组件，无法重置！");
            }
        }

        // 重置后重启动态检测
        StartDynamicCheck();
    }
}