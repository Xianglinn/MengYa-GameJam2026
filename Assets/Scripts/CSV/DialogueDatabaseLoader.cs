using System;
using System.Collections.Generic;
using UnityEngine;

public static class DialogueDatabaseLoader
{
    public static Dictionary<string, DialogueLine> LoadFromResources(string resourcesPath)
    {
        TextAsset csv = Resources.Load<TextAsset>(resourcesPath);
        if (csv == null)
            throw new Exception($"CSV not found at Resources/{resourcesPath}.csv");

        var grid = CsvUtil.Parse(csv.text);
        if (grid.Count < 2)
            throw new Exception("CSV has no data rows.");

        // header -> column index
        var header = grid[0];
        var col = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < header.Count; i++)
            col[header[i].Trim()] = i;

        string Get(List<string> r, string name)
        {
            if (!col.TryGetValue(name, out int idx)) return "";
            if (idx < 0 || idx >= r.Count) return "";
            return r[idx]?.Trim() ?? "";
        }

        var dict = new Dictionary<string, DialogueLine>();

        for (int r = 1; r < grid.Count; r++)
        {
            var row = grid[r];
            var line = new DialogueLine
            {
                id = Get(row, "id"),
                next_id = Get(row, "next_id"),
                speaker = Get(row, "speaker"),
                scene = Get(row, "scene"),
                content = Get(row, "content"),
                type = Get(row, "type"),
                choices = Get(row, "choices"),
                target = Get(row, "target"),
                condition = Get(row, "condition"),
                expression = Get(row, "expression"),
                fx = Get(row, "fx"),
                bg = Get(row, "bg"),
                effect = Get(row, "effect"),
                nameOverride = Get(row, "nameOverride"),
                bgm = Get(row, "bgm"),
            };

            if (string.IsNullOrEmpty(line.id))
            {
                Debug.LogWarning($"CSV row {r + 1}: empty id, skipped.");
                continue;
            }

            if (dict.ContainsKey(line.id))
                throw new Exception($"Duplicate id: {line.id}");

            dict[line.id] = line;
        }

        // 基本引用校验
        foreach (var kv in dict)
        {
            var line = kv.Value;

            bool IsEnd(string x) => string.IsNullOrEmpty(x) || x.Equals("END", StringComparison.OrdinalIgnoreCase);

            if (!IsEnd(line.next_id) && !dict.ContainsKey(line.next_id))
                Debug.LogWarning($"[Dialogue] id={line.id} next_id not found: {line.next_id}");

            // choice/jump 用 target
            if (!string.IsNullOrEmpty(line.target))
            {
                foreach (var t in SplitPipe(line.target))
                {
                    if (!IsEnd(t) && !dict.ContainsKey(t))
                        Debug.LogWarning($"[Dialogue] id={line.id} target not found: {t}");
                }
            }

            // choice 数量匹配
            if (line.type.Equals("choice", StringComparison.OrdinalIgnoreCase))
            {
                var cs = SplitPipe(line.choices);
                var ts = SplitPipe(line.target);
                if (cs.Count != ts.Count)
                    Debug.LogWarning($"[Dialogue] id={line.id} choice count ({cs.Count}) != target count ({ts.Count})");
            }
        }

        return dict;
    }

    public static List<string> SplitPipe(string s)
    {
        var res = new List<string>();
        if (string.IsNullOrEmpty(s)) return res;
        foreach (var part in s.Split('|'))
        {
            var t = part.Trim();
            if (t.Length > 0) res.Add(t);
        }
        return res;
    }
}
