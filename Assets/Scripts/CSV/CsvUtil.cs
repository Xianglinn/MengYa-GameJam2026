using System;
using System.Collections.Generic;
using System.Text;

public static class CsvUtil
{
    // 解析整份CSV为“行->列”的二维数组
    public static List<List<string>> Parse(string csvText)
    {
        var rows = new List<List<string>>();
        var row = new List<string>();
        var field = new StringBuilder();

        bool inQuotes = false;

        for (int i = 0; i < csvText.Length; i++)
        {
            char c = csvText[i];

            if (inQuotes)
            {
                if (c == '"')
                {
                    // 双引号转义 "" -> "
                    if (i + 1 < csvText.Length && csvText[i + 1] == '"')
                    {
                        field.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    field.Append(c);
                }
            }
            else
            {
                if (c == '"')
                {
                    inQuotes = true;
                }
                else if (c == ',')
                {
                    row.Add(field.ToString());
                    field.Clear();
                }
                else if (c == '\n')
                {
                    // 处理 \r\n
                    if (field.Length > 0 && field[field.Length - 1] == '\r')
                        field.Length -= 1;

                    row.Add(field.ToString());
                    field.Clear();

                    // 忽略空行（可选）
                    if (!(row.Count == 1 && string.IsNullOrEmpty(row[0])))
                        rows.Add(row);

                    row = new List<string>();
                }
                else
                {
                    field.Append(c);
                }
            }
        }

        // last field
        if (field.Length > 0 || inQuotes || row.Count > 0)
        {
            // 末尾可能有 \r
            if (field.Length > 0 && field[field.Length - 1] == '\r')
                field.Length -= 1;

            row.Add(field.ToString());
            rows.Add(row);
        }

        return rows;
    }
}
