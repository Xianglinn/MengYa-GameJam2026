using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class KeyWord : MonoBehaviour
{
    private bool isMatched = false; // 标记是否找到匹配位置
    private bool isRed = false; // 标记是否激活
    private Vector2 startPos; // 存储初始位置

    SpriteRenderer Sprite;
    private void Start()
    {
        startPos = transform.position;
        Sprite = GetComponent<SpriteRenderer>();
    }
    private void OnMouseDrag()
    {
        // 仅激活状态下可拖拽
        if (!isMatched && isRed)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }
    }
    private void OnMouseUp()
    {
        // 未匹配则回归初始位置
        if (!isMatched && isRed)
        {
            transform.position = startPos;
        }
    }
    /// <summary>
    /// 激活关键词（变红，可拖拽）
    /// </summary>
    public void SetRed()
    {

        Sprite.color = Color.red;

        isRed = true;
    }
    /// <summary>
    /// 重置关键词状态（恢复白色，禁用拖拽）
    /// </summary>
    public void ResetKeyWord()
    {
        Sprite.color = Color.white;
        isRed = false;
        isMatched = false;
        transform.position = startPos;
    }
}