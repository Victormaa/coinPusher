using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 推币机皮肤的静态配置表。
/// </summary>
public class SkinConfigDatabase : MonoBehaviour
{
    [Tooltip("所有可用皮肤配置列表。")]
    public List<SkinData> skins = new List<SkinData>();

    public SkinData GetById(string id)
    {
        if (string.IsNullOrEmpty(id) || skins == null || skins.Count == 0)
            return null;

        foreach (var s in skins)
        {
            if (s != null && s.id == id)
                return s;
        }

        return null;
    }
}

