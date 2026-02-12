using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 全局结局达成记录，与存档保存在同一目录 (Saves/achieved_endings.json)
/// 用于结局图鉴等 UI 显示 Resources/Backgrounds/end1~4（已达成）或 unlock（未达成）
/// </summary>
public static class EndingRegistry
{
    private static string RegistryPath => Path.Combine(Application.persistentDataPath, "Saves", Constants.EndingRegistryFileName);

    [System.Serializable]
    private class EndingRegistryData
    {
        public List<string> endings = new List<string>();
    }

    /// <summary>
    /// 记录已达成的结局
    /// </summary>
    /// <param name="endingId">结局 ID，对应 Resources/Backgrounds/ 下的资源名（如 end1, end2, end3, end4）</param>
    public static void Record(string endingId)
    {
        if (string.IsNullOrEmpty(endingId)) return;

        var data = Load();
        if (!data.endings.Contains(endingId))
        {
            data.endings.Add(endingId);
            Save(data);
            Debug.Log($"[EndingRegistry] Recorded ending: {endingId}");
        }
    }

    /// <summary>
    /// 检查是否已达成指定结局
    /// </summary>
    public static bool Has(string endingId)
    {
        if (string.IsNullOrEmpty(endingId)) return false;
        return Load().endings.Contains(endingId);
    }

    /// <summary>
    /// 获取所有已达成结局的 ID 列表
    /// </summary>
    public static List<string> GetAll()
    {
        return new List<string>(Load().endings);
    }

    /// <summary>
    /// 已达成结局数量
    /// </summary>
    public static int Count => Load().endings.Count;

    private static EndingRegistryData Load()
    {
        try
        {
            string dir = Path.GetDirectoryName(RegistryPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(RegistryPath))
                return new EndingRegistryData();

            string json = File.ReadAllText(RegistryPath);
            var data = JsonUtility.FromJson<EndingRegistryData>(json);
            return data ?? new EndingRegistryData();
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[EndingRegistry] Failed to load: {e.Message}");
            return new EndingRegistryData();
        }
    }

    private static void Save(EndingRegistryData data)
    {
        try
        {
            string dir = Path.GetDirectoryName(RegistryPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(RegistryPath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[EndingRegistry] Failed to save: {e.Message}");
        }
    }
}
