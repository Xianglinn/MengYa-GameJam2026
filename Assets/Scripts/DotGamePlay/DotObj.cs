using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DotObj : MonoBehaviour
{
    [SerializeField] private List<Transform> correctTrans = new List<Transform>();
    [SerializeField] private DotSolts solts;
    [SerializeField] private KeyWordOnline keyWordOnline;
    private Vector2 startPos;
    [SerializeField] private float snapDistance = 0.2f;
    private string occupiedSlotName = null;
    private bool isMatched = false;

    void Start()
    {
        startPos = transform.position;

        // 自动查找DotSolts并校验
        if (solts == null)
        {
            solts = GameObject.Find("DotSolts")?.GetComponent<DotSolts>();
            //未找到DotSolts组件
            if (solts == null)
            {
                enabled = false;
                return;
            }
        }

        // 自动查找KeyWordOnline并校验
        if (keyWordOnline == null)
        {
            //未找到KeyWordOnline组件，剧情激活功能失效！
            keyWordOnline = FindObjectOfType<KeyWordOnline>();
        }

        // 初始化正确位置列表
        foreach (var slot in solts.solt)
        {
            correctTrans.Add(slot.transform);
        }
    }

    private void OnMouseDrag()
    {
        // 拖拽时释放之前占用的格子
        if (occupiedSlotName != null)
        {
            solts.ReleaseSlot(this);
            occupiedSlotName = null;
            isMatched = false;
        }

        // 更新拖拽位置
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        // 显示格子提示
        solts.SetAllChildSpritesAlpha(solts.gameObject.transform, 0.8f);
    }

    private void OnMouseUp()
    {
        // 隐藏格子提示
        solts.SetAllChildSpritesAlpha(solts.gameObject.transform, 0f);

        // 检查是否吸附到正确格子
        foreach (var targetTrans in correctTrans)
        {
            float distance = Vector2.Distance(transform.position, targetTrans.position);
            if (distance <= snapDistance && solts.IsSlotFree(targetTrans.name))
            {
                transform.position = targetTrans.position;
                occupiedSlotName = targetTrans.name;
                solts.OccupySlot(occupiedSlotName, this);
                isMatched = true;
                break;
            }
        }

        // 未匹配则回归初始位置
        if (!isMatched)
        {
            transform.position = startPos;
        }

        // 触发剧情检测
        if (keyWordOnline != null)
        {
            PlotType triggeredPlot = keyWordOnline.CheckAndAwakePlot();
            //未满足剧情条件，重置关键词状态
            if (triggeredPlot == PlotType.None)
            {
                keyWordOnline.ResetKeyWordState();
            }
        }
    }

    private void OnDestroy()
    {
        // 销毁时释放占用的格子
        if (occupiedSlotName != null)
        {
            solts.ReleaseSlot(this);
        }
    }
}