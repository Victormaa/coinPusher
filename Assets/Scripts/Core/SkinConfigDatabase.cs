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
    private List<float> _weights = new List<float>();

    private void Awake()
    {
        LoadFromCsv();
    }

    private void LoadFromCsv()
    {
        _skins.Clear();
        _weights.Clear();
        string raw = CsvLoader.LoadFromStreamingAssets(skinsCsvPath);
        if (string.IsNullOrEmpty(raw))
            return;

        var rows = CsvLoader.ParseCsv(raw);
        foreach (var row in rows)
        {
            string id = CsvLoader.GetString(row, "id");
            if (string.IsNullOrEmpty(id))
                continue;

            float weight = CsvLoader.GetFloat(row, "weight", 1f);
            _skins.Add(new SkinData
            {
                id = id,
                displayName = CsvLoader.GetString(row, "displayName")
            });
            _weights.Add(weight <= 0f ? 1f : weight);
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

    /// <summary>
    /// 按权重随机抽取一个皮肤配置。
    /// </summary>
    public SkinData PickByWeight()
    {
        if (_skins == null || _skins.Count == 0)
            return null;

        float total = 0f;
        for (int i = 0; i < _skins.Count; i++)
        {
            float w = i < _weights.Count ? _weights[i] : 1f;
            if (w <= 0f) continue;
            total += w;
        }

        if (total <= 0f)
            return _skins[0];

        float rand = Random.Range(0f, total);
        float acc = 0f;
        for (int i = 0; i < _skins.Count; i++)
        {
            float w = i < _weights.Count ? _weights[i] : 1f;
            if (w <= 0f) continue;
            acc += w;
            if (rand <= acc)
                return _skins[i];
        }
        return _skins[_skins.Count - 1];
    }
}

