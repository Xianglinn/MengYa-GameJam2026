using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyWord : MonoBehaviour
{
    private bool isMatched = false; // 是否匹配剧情槽位
    private bool isRed = false; // 是否变红（激活）
    private Vector2 startPos; // 初始位置

    [SerializeField] private PlotArea plotArea; // 关联剧情区
    SpriteRenderer Sprite;

    // 对外暴露状态
    public bool IsRed => isRed;
    public bool IsMatched => isMatched;

    private void Start()
    {
        startPos = transform.position;
        Sprite = GetComponent<SpriteRenderer>();

        // 自动查找剧情区
        if (plotArea == null)
        {
            plotArea = FindObjectOfType<PlotArea>();
        }
    }

    private void OnMouseDrag()
    {
        // 只有激活变红且未匹配的关键词才能拖拽
        if (!isMatched && isRed)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }
    }

    private void OnMouseUp()
    {
        // 未匹配则返回初始位置
        if (!isMatched && isRed)
        {
            // 检测是否拖入剧情区
            bool isInPlotArea = plotArea?.CheckKeywordInArea(this) ?? false;
            if (!isInPlotArea)
            {
                transform.position = startPos; // 未拖入剧情区则返回初始位置
            }
        }
    }

    /// <summary>
    /// 设置关键词变红（激活）
    /// </summary>
    public void SetRed()
    {
        Sprite.color = Color.green;
        isRed = true;
    }

    /// <summary>
    /// 标记关键词已匹配剧情区
    /// </summary>
    public void SetMatched(bool matched)
    {
        isMatched = matched;
    }

    /// <summary>
    /// 重置关键词状态
    /// </summary>
    public void ResetKeyWord()
    {
        Sprite.color = Color.black;
        isRed = false;
        isMatched = false;
        transform.position = startPos;
    }
}