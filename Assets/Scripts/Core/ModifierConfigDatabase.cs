using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 推币机装备（SlotModifier）的静态配置表。
/// 从 StreamingAssets/Config/Modifiers.csv 加载，Excel 另存为 CSV 后放入即可。
/// </summary>
public class ModifierConfigDatabase : MonoBehaviour
{
    [Tooltip("CSV 相对 StreamingAssets 的路径")]
    public string modifiersCsvPath = "Config/Modifiers.csv";

    private List<SlotModifierData> _modifiers = new List<SlotModifierData>();

    private void Awake()
    {
        LoadFromCsv();
    }

    private void LoadFromCsv()
    {
        _modifiers.Clear();
        string raw = CsvLoader.LoadFromStreamingAssets(modifiersCsvPath);
        if (string.IsNullOrEmpty(raw))
            return;

        var rows = CsvLoader.ParseCsv(raw);
        foreach (var row in rows)
        {
            string id = CsvLoader.GetString(row, "id");
            if (string.IsNullOrEmpty(id))
                continue;

            var data = new SlotModifierData
            {
                id = id,
                level = CsvLoader.GetInt(row, "level", 1),
                sideTiltOffset = CsvLoader.GetFloat(row, "sideTiltOffset"),
                pusherSpeedMultiplier = CsvLoader.GetFloat(row, "pusherSpeedMultiplier", 1f),
                dropTiltOffset = CsvLoader.GetFloat(row, "dropTiltOffset")
            };
            _modifiers.Add(data);
        }
    }

    /// <summary>
    /// 根据 ID 和等级查找装备配置。
    /// 如果找不到完全匹配的等级，则优先返回同 ID 的任意一条配置。
    /// </summary>
    public SlotModifierData GetById(string id, int level = -1)
    {
        if (string.IsNullOrEmpty(id) || _modifiers == null || _modifiers.Count == 0)
            return null;

        SlotModifierData fallback = null;
        foreach (var m in _modifiers)
        {
            if (m == null || m.id != id) continue;

            if (level < 0 || m.level == level)
            {
                return m;
            }

            // 记录一个同 ID 的备选
            if (fallback == null)
            {
                fallback = m;
            }
        }

        return fallback;
    }
}

