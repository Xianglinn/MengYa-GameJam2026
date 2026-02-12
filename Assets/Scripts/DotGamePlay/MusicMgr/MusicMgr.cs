using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Reflection;

/// <summary>
/// 单例模式基类
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseManager<T> where T : class//,new()
{
    private static T instance;

    //属性的方式
    public static T Instance
    {
        get
        {
            if (instance == null)
            {      
                Type type = typeof(T);
                ConstructorInfo info = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
                                                            null,
                                                            Type.EmptyTypes,
                                                            null);
                if (info != null)
                    instance = info.Invoke(null) as T;
                else
                    Debug.LogError("没有得到对应的无参构造函数");
            }

            return instance;
        }
    }
}


/// <summary>
/// 音乐管理器
/// </summary>
public class MusicMgr : BaseManager<MusicMgr>
{
    //背景音乐播放组件
    private AudioSource bkMusic = null;

    //背景音乐大小
    private float bkMusicValue = 0.1f;

    private MusicMgr() { }

    //播放背景音乐
    public void PlayBKMusic(string name)
    {
        //动态创建播放背景音乐 不会过场景移除 
        if (bkMusic == null)
        {
            GameObject obj = new GameObject();
            obj.name = "BKMusic";
            GameObject.DontDestroyOnLoad(obj);
            bkMusic = obj.AddComponent<AudioSource>();
        }

        AudioClip Clip = Resources.Load<AudioClip>(name);
        bkMusic.clip = Clip;
        bkMusic.loop = true;
        bkMusic.volume = bkMusicValue;
        bkMusic.Play();
    }

    //停止背景音乐
    public void StopBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Stop();
    }

    //暂停背景音乐
    public void PauseBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Pause();
    }

    //设置背景音乐大小
    public void ChangeBKMusicValue(float v)
    {
        bkMusicValue = v;
        if (bkMusic == null)
            return;
        bkMusic.volume = bkMusicValue;
    }

}
