using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 代币奖励配置表。
/// 从 StreamingAssets/Config/Tokens.csv 加载，表头：amount,weight。
/// </summary>
public class TokensConfigDatabase : MonoBehaviour
{
    [Tooltip("CSV 相对 StreamingAssets 的路径")]
    public string tokensCsvPath = "Config/Tokens.csv";

    private List<(int amount, float weight)> _entries = new List<(int, float)>();

    private void Awake()
    {
        LoadFromCsv();
    }

    private void LoadFromCsv()
    {
        _entries.Clear();
        string raw = CsvLoader.LoadFromStreamingAssets(tokensCsvPath);
        if (string.IsNullOrEmpty(raw))
            return;

        var rows = CsvLoader.ParseCsv(raw);
        foreach (var row in rows)
        {
            int amount = CsvLoader.GetInt(row, "amount", 1);
            float weight = CsvLoader.GetFloat(row, "weight", 1f);
            if (weight <= 0f) continue;
            _entries.Add((amount, weight));
        }
    }

    /// <summary>
    /// 按权重随机抽取一个代币数量。
    /// </summary>
    public int PickByWeight()
    {
        if (_entries == null || _entries.Count == 0)
            return 1;

        float total = 0f;
        foreach (var (_, w) in _entries)
            total += w;

        if (total <= 0f)
            return _entries[0].amount;

        float rand = Random.Range(0f, total);
        float acc = 0f;
        foreach (var (amount, w) in _entries)
        {
            acc += w;
            if (rand <= acc)
                return amount;
        }
        return _entries[_entries.Count - 1].amount;
    }
}
