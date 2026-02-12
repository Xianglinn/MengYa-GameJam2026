using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonAutoMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                //动态创建 动态挂载
                //在场景上创建空物体
                GameObject obj = new GameObject();
                obj.name = typeof(T).ToString();
                //动态挂载对应的 单例模式脚本
                instance = obj.AddComponent<T>();
                //过场景时不移除
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

}

/// <summary>
/// 音效管理器
/// </summary>
public class SoundMgr : SingletonAutoMono<SoundMgr>
{

    // 鼠标点击音效资源
    [Tooltip("鼠标左键点击的音效文件")]
    public AudioClip clickClip;

    // 音频播放组件
    private AudioSource audioSource;

    // 音效音量（0-1）
    [Range(0f, 1f)]
    public float volume = 0.8f;

    private void Awake()
    {
        clickClip = Resources.Load<AudioClip>("Music/Click");

        // 添加AudioSource组件（如果物体上没有）
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 配置AudioSource参数
        audioSource.playOnAwake = false; // 不自动播放
        audioSource.loop = false;
        audioSource.volume = volume;
    }

    private void Update()
    {
        // 检测鼠标左键点击（全局检测）
        if (Input.GetMouseButtonDown(0))
        {
            PlayClickSound();
        }
    }

    /// <summary>
    /// 播放点击音效的公共方法（供其他脚本调用）
    /// </summary>
    public void PlayClickSound()
    {
        // 检查音效资源是否赋值
        if (clickClip == null)
        {
            Debug.LogWarning("点击音效资源未赋值！");
            return;
        }

        // 播放音效（PlayOneShot不会打断当前播放的音效）
        audioSource.PlayOneShot(clickClip, volume);
    }

}
