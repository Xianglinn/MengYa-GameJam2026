using System.Collections.Generic;
using UnityEngine;

public class PlotArea : MonoBehaviour
{
    [Header("剧情区配置")]
    [SerializeField] private KeyWordOnline keyWordOnline; // 关联关键词管理器
    [SerializeField] private float snapDistance = 0.3f; // 关键词吸附到剧情区的距离
    [SerializeField] private bool autoTriggerPlot = true; // 是否自动触发剧情

    // 已拖入剧情区的关键词
    private List<KeyWord> draggedKeywords = new List<KeyWord>();
    // 当前激活的剧情类型（从KeyWordOnline获取）
    private PlotType currentActivePlot = PlotType.None;

    /// <summary>
    /// 检测关键词是否拖入剧情区（在KeyWord拖拽结束时调用）
    /// </summary>
    public bool CheckKeywordInArea(KeyWord keyword)
    {
        // 只有激活的关键词才能拖入
        if (!keyword.IsRed) return false;

        // 计算关键词与剧情区的距离
        float distance = Vector2.Distance(keyword.transform.position, transform.position);
        if (distance <= snapDistance)
        {
            // 吸附到剧情区中心
            keyword.transform.position = transform.position;
            // 添加到已拖入列表（去重）
            if (!draggedKeywords.Contains(keyword))
            {
                draggedKeywords.Add(keyword);
                keyword.SetMatched(true); // 标记关键词已匹配剧情区
                Debug.Log($"关键词[{keyword.gameObject.name}]已拖入{currentActivePlot}剧情区");

                // 自动验证并触发剧情
                if (autoTriggerPlot)
                {
                    CheckAndTriggerPlotAuto();
                }
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// 自动检测是否满足剧情触发条件并执行
    /// </summary>
    private void CheckAndTriggerPlotAuto()
    {
        // 获取当前激活的剧情类型
        currentActivePlot = keyWordOnline.CurrentActivatedPlot;
        if (currentActivePlot == PlotType.None)
        {
            Debug.LogWarning("暂无激活的剧情类型，无需验证关键词");
            return;
        }

        // 获取当前激活剧情对应的所有关键词
        List<GameObject> targetKeyList = GetTargetKeyListByPlotType(currentActivePlot);
        if (targetKeyList == null || targetKeyList.Count == 0)
        {
            Debug.LogError($"剧情类型[{currentActivePlot}]未配置关键词列表");
            return;
        }

        // 验证是否所有目标关键词都已拖入
        int matchedCount = 0;
        foreach (var keyObj in targetKeyList)
        {
            KeyWord keyComp = keyObj.GetComponent<KeyWord>();
            if (keyComp != null && draggedKeywords.Contains(keyComp))
            {
                matchedCount++;
            }
        }

        bool isAllMatched = matchedCount == targetKeyList.Count;
        if (isAllMatched)
        {
            Debug.Log($"{currentActivePlot}剧情的所有关键词已全部拖入剧情区（共{matchedCount}/{targetKeyList.Count}）");
            // 自动触发剧情逻辑
            TriggerPlotLogic(currentActivePlot);
        }
        else
        {
            Debug.LogWarning($"{currentActivePlot}剧情仍有未拖入的关键词（已拖入{matchedCount}/{targetKeyList.Count}）");
        }
    }

    /// <summary>
    /// 根据剧情类型获取对应的关键词列表
    /// </summary>
    private List<GameObject> GetTargetKeyListByPlotType(PlotType plotType)
    {
        switch (plotType)
        {
            case PlotType.APlot:
                return keyWordOnline.AKeyList;
            case PlotType.BPlot:
                return keyWordOnline.BKeyList;
            case PlotType.Cplot:
                return keyWordOnline.CKeyList;
            default:
                return null;
        }
    }

    /// <summary>
    /// 触发对应剧情的业务逻辑
    /// </summary>
    private void TriggerPlotLogic(PlotType plotType)
    {
        if (GameManager.I == null)
        {
            Debug.LogError("GameManager未找到，无法触发剧情逻辑");
            return;
        }
        string currentSceneName = gameObject.scene.name;
        switch (plotType)
        {
            case PlotType.APlot:
                Debug.Log("触发A剧情逻辑");
                if(currentSceneName == "DotGamePlayDome 1")
                {
                    GameManager.I.GoToBadEndingFailed();
                }
                else if(currentSceneName == "DotGamePlayDome 2")
                {
                    GameManager.I.GoToNormalEndingDrifting();
                }
                else
                {
                    GameManager.I.GoToBadEndingFailed();
                }
                //GameManager.I.GoToBadEndingFailed();
                break;

            case PlotType.BPlot:
                Debug.Log("触发B剧情逻辑");
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
                //GameManager.I.GoToNormalEndingDrifting();
                break;

            case PlotType.Cplot:
                Debug.Log("触发C剧情逻辑");
                break;
        }

        // 触发剧情后重置状态
        ResetPlotArea();
        keyWordOnline.ResetKeyWordState();

        // 保存游戏进度
        //GameManager.I.QuickSave();
    }

    /// <summary>
    /// 重置剧情区状态（用于重新开始验证）
    /// </summary>
    public void ResetPlotArea()
    {
        foreach (var keyword in draggedKeywords)
        {
            keyword.ResetKeyWord(); // 重置关键词状态
        }
        draggedKeywords.Clear();
        currentActivePlot = PlotType.None;
        Debug.Log("剧情区已重置，清空所有拖入的关键词");
    }
}