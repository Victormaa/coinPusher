using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 推币机皮肤的静态配置表。
/// 从 StreamingAssets/Config/Skins.csv 加载，Excel 另存为 CSV 后放入即可。
/// </summary>
public class SkinConfigDatabase : MonoBehaviour
{
    [Tooltip("CSV 相对 StreamingAssets 的路径")]
    public string skinsCsvPath = "Config/Skins.csv";

    private List<SkinData> _skins = new List<SkinData>();

    private void Awake()
    {
        LoadFromCsv();
    }

    private void LoadFromCsv()
    {
        _skins.Clear();
        string raw = CsvLoader.LoadFromStreamingAssets(skinsCsvPath);
        if (string.IsNullOrEmpty(raw))
            return;

        var rows = CsvLoader.ParseCsv(raw);
        foreach (var row in rows)
        {
            string id = CsvLoader.GetString(row, "id");
            if (string.IsNullOrEmpty(id))
                continue;

            _skins.Add(new SkinData
            {
                id = id,
                displayName = CsvLoader.GetString(row, "displayName")
            });
        }
    }

    public SkinData GetById(string id)
    {
        if (string.IsNullOrEmpty(id) || _skins == null || _skins.Count == 0)
            return null;

        foreach (var s in _skins)
        {
            if (s != null && s.id == id)
                return s;
        }

        return null;
    }
}

