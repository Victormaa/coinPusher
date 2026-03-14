using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 推币机装备（SlotModifier）的静态配置表。
/// 通过 ID + 等级返回对应的数值配置。
/// </summary>
public class ModifierConfigDatabase : MonoBehaviour
{
    [Tooltip("所有可掉落/可获得的装备配置列表。")]
    public List<SlotModifierData> modifiers = new List<SlotModifierData>();

    /// <summary>
    /// 根据 ID 和等级查找装备配置。
    /// 如果找不到完全匹配的等级，则优先返回同 ID 的任意一条配置。
    /// </summary>
    public SlotModifierData GetById(string id, int level = -1)
    {
        if (string.IsNullOrEmpty(id) || modifiers == null || modifiers.Count == 0)
            return null;

        SlotModifierData fallback = null;
        foreach (var m in modifiers)
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

