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
                id = Get(row, Constants.CsvColumns.Id),
                next_id = Get(row, Constants.CsvColumns.NextId),
                speaker = Get(row, Constants.CsvColumns.Speaker),
                scene = Get(row, Constants.CsvColumns.Scene),
                content = Get(row, Constants.CsvColumns.Content),
                type = Get(row, Constants.CsvColumns.Type),
                choices = Get(row, Constants.CsvColumns.Choices),
                target = Get(row, Constants.CsvColumns.Target),
                condition = Get(row, Constants.CsvColumns.Condition),
                expression = Get(row, Constants.CsvColumns.Expression),
                fx = Get(row, Constants.CsvColumns.Fx),
                bg = Get(row, Constants.CsvColumns.Bg),
                effect = Get(row, Constants.CsvColumns.Effect),
                nameOverride = Get(row, Constants.CsvColumns.NameOverride),
                bgm = Get(row, Constants.CsvColumns.Bgm),
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

            bool IsEnd(string x) => string.IsNullOrEmpty(x) || x.Equals(Constants.EndMarker, StringComparison.OrdinalIgnoreCase);

            if (!IsEnd(line.next_id) && !dict.ContainsKey(line.next_id))
                Debug.LogWarning($"{Constants.DialogueTag} id={line.id} next_id not found: {line.next_id}");

            // choice/jump 用 target
            if (!string.IsNullOrEmpty(line.target))
            {
                foreach (var t in SplitPipe(line.target))
                {
                    if (!IsEnd(t) && !dict.ContainsKey(t))
                        Debug.LogWarning($"{Constants.DialogueTag} id={line.id} target not found: {t}");
                }
            }

            // choice 数量匹配
            if (line.type.Equals(Constants.DialogueType.Choice, StringComparison.OrdinalIgnoreCase))
            {
                var cs = SplitPipe(line.choices);
                var ts = SplitPipe(line.target);
                if (cs.Count != ts.Count)
                    Debug.LogWarning($"{Constants.DialogueTag} id={line.id} choice count ({cs.Count}) != target count ({ts.Count})");
            }
        }

        return dict;
    }

    public static List<string> SplitPipe(string s)
    {
        var res = new List<string>();
        if (string.IsNullOrEmpty(s)) return res;
        foreach (var part in s.Split(Constants.PipeSeparator[0]))
        {
            var t = part.Trim();
            if (t.Length > 0) res.Add(t);
        }
        return res;
    }
}
