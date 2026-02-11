using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DotSolts : MonoBehaviour
{
    [SerializeField] public List<GameObject> solt = new List<GameObject>();
    [SerializeField] public Dictionary<string, DotObj> slotOccupancy = new Dictionary<string, DotObj>();
    private void Awake()
    {
        InitializeSlots();
    }
    private void Start()
    {
        //初始化为隐藏状态，拖拽时显示
        SetAllChildSpritesAlpha(this.transform, 0f);
    }
    /// <summary>
    /// 初始化格子列表和占用字典（自动遍历子物体，新场景仅需添加子物体格子）
    /// </summary>
    private void InitializeSlots()
    {
        solt.Clear();
        slotOccupancy.Clear();
        foreach (Transform child in transform)
        {
            GameObject slot = child.gameObject;
            solt.Add(slot);
            slotOccupancy.Add(slot.name, null);
        }
    }
    /// <summary>
    /// 检查格子是否空闲
    /// </summary>
    public bool IsSlotFree(string slotName)
    {
        bool isFree = slotOccupancy.TryGetValue(slotName, out DotObj occupiedObj) && occupiedObj == null;
        return isFree;
    }
    /// <summary>
    /// 占用格子
    /// </summary>
    public void OccupySlot(string slotName, DotObj dot)
    {
        if (IsSlotFree(slotName))
        {
            slotOccupancy[slotName] = dot;
        }
    }
    /// <summary>
    /// 释放格子
    /// </summary>
    public void ReleaseSlot(DotObj dot)
    {
        foreach (var kvp in slotOccupancy)
        {
            if (kvp.Value == dot)
            {
                slotOccupancy[kvp.Key] = null;
                break;
            }
        }
    }
    /// <summary>
    /// 设置所有子物体Sprite透明度
    /// </summary>
    public void SetAllChildSpritesAlpha(Transform parent, float alpha)
    {
        foreach (Transform child in parent)
        {
            SpriteRenderer childSprite = child.GetComponent<SpriteRenderer>();
            if (childSprite != null)
            {
                Color color = childSprite.color;
                color.a = alpha;
                childSprite.color = color;
            }
        }
    }
}