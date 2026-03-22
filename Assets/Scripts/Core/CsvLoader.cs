using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// 从 StreamingAssets 读取 CSV 文件并解析。
/// Excel 编辑后另存为 CSV（UTF-8），放入 Assets/StreamingAssets/Config/ 即可。
/// </summary>
public static class CsvLoader
{
    /// <summary>
    /// 从 StreamingAssets 读取文本文件。
    /// 路径为相对 StreamingAssets 的路径，如 "Config/Modifiers.csv"。
    /// </summary>
    public static string LoadFromStreamingAssets(string relativePath)
    {
        string path = Path.Combine(Application.streamingAssetsPath, relativePath);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"[CsvLoader] File not found: {path}");
            return null;
        }

        try
        {
            return File.ReadAllText(path, Encoding.UTF8);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[CsvLoader] Failed to read {path}: {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// 解析 CSV 文本，首行为表头，返回每行按列名映射的字典数组。
    /// 空行会被跳过。
    /// </summary>
    public static List<Dictionary<string, string>> ParseCsv(string rawText)
    {
        var result = new List<Dictionary<string, string>>();
        if (string.IsNullOrEmpty(rawText))
            return result;

        string[] lines = rawText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        if (lines.Length < 2)
            return result;

        string[] headers = ParseCsvLine(lines[0]);
        if (headers == null || headers.Length == 0)
            return result;

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] values = ParseCsvLine(line);
            if (values == null)
                continue;

            var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (int j = 0; j < headers.Length; j++)
            {
                string key = headers[j]?.Trim() ?? "";
                string value = j < values.Length ? (values[j]?.Trim() ?? "") : "";
                if (!string.IsNullOrEmpty(key))
                    row[key] = value;
            }
            result.Add(row);
        }

        return result;
    }

    /// <summary>
    /// 解析单行 CSV，支持双引号包裹的字段（内含逗号或换行）。
    /// </summary>
    private static string[] ParseCsvLine(string line)
    {
        var values = new List<string>();
        var current = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (inQuotes)
            {
                if (c == '"')
                {
                    if (i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    current.Append(c);
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
                    values.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }
        }

        values.Add(current.ToString());
        return values.ToArray();
    }

    /// <summary>
    /// 从行字典中安全获取字符串，缺省返回空字符串。
    /// </summary>
    public static string GetString(Dictionary<string, string> row, string key)
    {
        return row != null && row.TryGetValue(key, out var v) ? v : "";
    }

    /// <summary>
    /// 从行字典中解析整数，失败返回 defaultValue。
    /// </summary>
    public static int GetInt(Dictionary<string, string> row, string key, int defaultValue = 0)
    {
        string s = GetString(row, key);
        return int.TryParse(s, out int v) ? v : defaultValue;
    }

    /// <summary>
    /// 从行字典中解析浮点数，失败返回 defaultValue。
    /// </summary>
    public static float GetFloat(Dictionary<string, string> row, string key, float defaultValue = 0f)
    {
        string s = GetString(row, key);
        return float.TryParse(s, out float v) ? v : defaultValue;
    }
}
