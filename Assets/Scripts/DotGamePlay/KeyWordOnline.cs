using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlotType
{
    None = 0,
    APlot = 1,
    BPlot = 2,
    Cplot = 3
}

public class KeyWordOnline : MonoBehaviour
{
    [SerializeField] private DotSolts solts;
    private bool isKeyWordActivated = false;
    private PlotType currentActivatedPlot = PlotType.None;

    [Header("A剧情配置（多关键词）")]
    [SerializeField] private List<GameObject> aKeyList = new List<GameObject>();
    [SerializeField] private List<string> aPlotSlotNames = new List<string>(); // 可配置：A剧情触发的格子名列表

    [Header("B剧情配置（多关键词）")]
    [SerializeField] private List<GameObject> bKeyList = new List<GameObject>();
    [SerializeField] private List<string> bPlotSlotNames = new List<string>(); // 可配置：B剧情触发的格子名列表

    [Header("C剧情配置（扩展，多关键词）")]
    [SerializeField] private List<GameObject> cKeyList = new List<GameObject>();
    [SerializeField] private List<string> cPlotSlotNames = new List<string>(); // 可配置：C剧情触发的格子名列表

    // 对外暴露属性
    public PlotType CurrentActivatedPlot => currentActivatedPlot;
    public bool IsKeyWordActivated => isKeyWordActivated;
    public List<GameObject> AKeyList => aKeyList;
    public List<GameObject> BKeyList => bKeyList;
    public List<GameObject> CKeyList => cKeyList;

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

        // 校验关键词列表和卡槽配置
        ValidatePlotConfig(aKeyList, aPlotSlotNames, "A");
        ValidatePlotConfig(bKeyList, bPlotSlotNames, "B");
        ValidatePlotConfig(cKeyList, cPlotSlotNames, "C", true);

        // 检测卡槽条件交集（调试提示）
        CheckSlotIntersection();
    }

    #region 初始化校验
    /// <summary>
    /// 统一校验剧情的关键词和卡槽配置
    /// </summary>
    private void ValidatePlotConfig(List<GameObject> keyList, List<string> slotList, string plotTag, bool isExtend = false)
    {
        // 扩展剧情（如C）允许空配置，非扩展剧情强制校验
        if (slotList.Count == 0 && !isExtend)
        {
            Debug.LogError($"{plotTag}剧情未配置卡槽列表，无法触发！");
            return;
        }
        if (keyList.Count == 0 && !isExtend)
        {
            Debug.LogError($"{plotTag}剧情未配置关键词列表，无法激活！");
            return;
        }
        // 校验关键词组件
        foreach (var keyObj in keyList)
        {
            if (keyObj == null)
            {
                Debug.LogError($"{plotTag}剧情关键词列表存在空对象！");
                continue;
            }
            if (keyObj.GetComponent<KeyWord>() == null)
            {
                Debug.LogError($"{plotTag}剧情对象【{keyObj.name}】未挂载KeyWord组件！");
            }
        }
    }

    /// <summary>
    /// 检测卡槽条件交集（仅调试提示）
    /// </summary>
    private void CheckSlotIntersection()
    {
        if (HasIntersection(aPlotSlotNames, bPlotSlotNames))
            Debug.LogWarning($"A/B剧情卡槽存在交集，冲突场景将触发排他规则");
        if (HasIntersection(aPlotSlotNames, cPlotSlotNames))
            Debug.LogWarning($"A/C剧情卡槽存在交集，冲突场景将触发排他规则");
        if (HasIntersection(bPlotSlotNames, cPlotSlotNames))
            Debug.LogWarning($"B/C剧情卡槽存在交集，冲突场景将触发排他规则");
    }

    /// <summary>
    /// 判断两个列表是否有交集
    /// </summary>
    private bool HasIntersection(List<string> list1, List<string> list2)
    {
        if (list1.Count == 0 || list2.Count == 0) return false;
        foreach (var item in list1)
        {
            if (list2.Contains(item)) return true;
        }
        return false;
    }
    #endregion



    #region 核心检测逻辑
    /// <summary>
    /// 检查并激活剧情分支（核心：兼容单条件，排他多条件）
    /// </summary>
    public PlotType CheckAndAwakePlot()
    {
        // 基础依赖校验
        if (solts == null || solts.slotOccupancy == null) return PlotType.None;

        if (cKeyList.Count > 0 && cPlotSlotNames.Count > 0 && CheckSlotsOccupied(cPlotSlotNames))
        {
            ActivatePlotKey(cKeyList, PlotType.Cplot);
            return PlotType.Cplot;
        }

        // 中优先级
        if (bKeyList.Count > 0 && bPlotSlotNames.Count > 0 && CheckSlotsOccupied(bPlotSlotNames))
        {
            ActivatePlotKey(bKeyList, PlotType.BPlot);
            return PlotType.BPlot;
        }

        // 最低优先级
        if (aKeyList.Count > 0 && aPlotSlotNames.Count > 0 && CheckSlotsOccupied(aPlotSlotNames))
        {
            ActivatePlotKey(aKeyList, PlotType.APlot);
            return PlotType.APlot;
        }

        return PlotType.None;
    }


    /// <summary>
    /// 检查指定传入格子是否全部被占用
    /// </summary>
    private bool CheckSlotsOccupied(List<string> slotNames)
    {
        // 第一步：校验目标卡槽是否全部存在且被占用
        foreach (string slotName in slotNames)
        {
            if (!solts.slotOccupancy.ContainsKey(slotName))
            {
                Debug.LogWarning($"格子{slotName}不存在于DotSolts中");
                return false;
            }
            if (solts.slotOccupancy[slotName] == null)
            {
                return false;
            }
        }

        //校验非目标卡槽是否全部空闲
        foreach (var kvp in solts.slotOccupancy)
        {
            string currentSlotName = kvp.Key;
            DotObj occupiedDot = kvp.Value;

            // 如果当前卡槽不是目标卡槽，但被占用 → 条件不满足
            if (!slotNames.Contains(currentSlotName) && occupiedDot != null)
            {
                Debug.LogWarning($"非目标卡槽[{currentSlotName}]被占用，拒绝触发剧情");
                return false;
            }
        }

        // 两步都满足 → 条件成立
        return true;
    }

    /// <summary>
    /// 激活指定剧情的所有关键词
    /// </summary>
    private void ActivatePlotKey(List<GameObject> keyList, PlotType plotType)
    {
        foreach (var keyObj in keyList)
        {
            if (keyObj == null) continue;
            KeyWord keyComp = keyObj.GetComponent<KeyWord>();
            if (keyComp != null) keyComp.SetRed();
            else Debug.LogWarning($"关键词【{keyObj.name}】无KeyWord组件，跳过激活");
        }
        isKeyWordActivated = true;
        currentActivatedPlot = plotType;

        Debug.Log($"触发剧情类型：{plotType}，对应关键词已变红可拖动");
    }
    #endregion

    /// <summary>
    /// 重置关键词状态（剧情重新开始/切换时调用）
    /// </summary>
    public void ResetKeyWordState()
    {
        isKeyWordActivated = false;
        currentActivatedPlot = PlotType.None;

        ResetPlotKey(aKeyList, "A");
        ResetPlotKey(bKeyList, "B");
        ResetPlotKey(cKeyList, "C");
        Debug.Log("所有剧情关键词状态已重置");
    }

    /// <summary>
    /// 重置指定剧情的所有关键词
    /// </summary>
    private void ResetPlotKey(List<GameObject> keyList, string plotTag)
    {
        foreach (var keyObj in keyList)
        {
            if (keyObj == null) continue;
            KeyWord keyComp = keyObj.GetComponent<KeyWord>();
            if (keyComp != null) keyComp.ResetKeyWord();
            else Debug.LogWarning($"{plotTag}剧情关键词【{keyObj.name}】无KeyWord组件，跳过重置");
        }
    }
}