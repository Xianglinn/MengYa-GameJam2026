using System.Collections.Generic;
using UnityEngine;

public class DialogueBoot : MonoBehaviour
{
    public string startId = "pre_0001";
    private Dictionary<string, DialogueLine> db;

    void Start()
    {
        db = DialogueDatabaseLoader.LoadFromResources("Dialogues/Prologue");
        Debug.Log($"Loaded lines: {db.Count}");

        // demo: 打印起始行
        if (db.TryGetValue(startId, out var line))
        {
            Debug.Log($"[{line.type}] {line.DisplayName}: {line.content}");
        }
    }
}
