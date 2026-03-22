using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 盲盒系统入口：负责根据权重随机抽取奖励并结算。
/// SpecialCoin 掉入奖励区时由 DeadZone 调用 OpenOnce。
/// </summary>
public class BlindBoxManager : MonoBehaviour
{
    public static BlindBoxManager Instance { get; private set; }

    [Tooltip("CSV 相对 StreamingAssets 的路径，仅决定奖励类型及权重")]
    public string rewardsCsvPath = "Config/BlindBoxRewards.csv";

    [Header("配置数据库引用（用于按类型权重抽取具体奖励）")]
    public ModifierConfigDatabase modifierConfigDatabase;
    public SkinConfigDatabase skinConfigDatabase;
    public TokensConfigDatabase tokensConfigDatabase;

    private List<(BlindBoxRewardType type, float weight)> _typeWeights = new List<(BlindBoxRewardType, float)>();
    private int _pendingCount;

    /// <summary>
    /// 当前可开启的盲盒数量。SpecialCoin 掉入奖励区时 +1，玩家点击打开时 -1 并执行抽奖。
    /// </summary>
    public int PendingCount => _pendingCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (modifierConfigDatabase == null) modifierConfigDatabase = FindObjectOfType<ModifierConfigDatabase>();
        if (skinConfigDatabase == null) skinConfigDatabase = FindObjectOfType<SkinConfigDatabase>();
        if (tokensConfigDatabase == null) tokensConfigDatabase = FindObjectOfType<TokensConfigDatabase>();
        LoadRewardsFromCsv();
    }

    private void LoadRewardsFromCsv()
    {
        _typeWeights.Clear();
        string raw = CsvLoader.LoadFromStreamingAssets(rewardsCsvPath);
        if (string.IsNullOrEmpty(raw))
            return;

        var rows = CsvLoader.ParseCsv(raw);
        foreach (var row in rows)
        {
            string typeStr = CsvLoader.GetString(row, "type");
            if (string.IsNullOrEmpty(typeStr))
                continue;

            if (!System.Enum.TryParse(typeStr, true, out BlindBoxRewardType type))
            {
                Debug.LogWarning($"[BlindBoxManager] Unknown reward type: {typeStr}");
                continue;
            }

            float weight = CsvLoader.GetFloat(row, "weight", 1f);
            if (weight <= 0f) continue;
            _typeWeights.Add((type, weight));
        }
    }

    /// <summary>
    /// SpecialCoin 掉入奖励区时调用，增加一个可开启的盲盒。
    /// </summary>
    public void AddPendingBlindBox()
    {
        _pendingCount++;
    }

    /// <summary>
    /// 玩家点击「打开」时调用。先抽类型，再从对应表按权重抽取具体奖励并发放。
    /// </summary>
    public BlindBoxResult OpenOnce()
    {
        if (_pendingCount <= 0)
            return null;

        _pendingCount--;

        BlindBoxRewardType pickedType = PickRewardType();
        var result = BuildAndGrantReward(pickedType);
        if (result == null)
            Debug.LogWarning("[BlindBoxManager] No reward granted. Check type tables and config references.");
        return result;
    }

    /// <summary>
    /// 第一步：按 BlindBoxRewards 表的权重抽取奖励类型。
    /// </summary>
    private BlindBoxRewardType PickRewardType()
    {
        if (_typeWeights == null || _typeWeights.Count == 0)
            return BlindBoxRewardType.Tokens;

        float total = 0f;
        foreach (var (_, w) in _typeWeights)
            total += w;

        if (total <= 0f)
            return _typeWeights[0].type;

        float rand = Random.Range(0f, total);
        float acc = 0f;
        foreach (var (type, w) in _typeWeights)
        {
            acc += w;
            if (rand <= acc)
                return type;
        }
        return _typeWeights[_typeWeights.Count - 1].type;
    }

    /// <summary>
    /// 第二步：根据类型从对应表按权重抽取，并发放奖励。
    /// </summary>
    private BlindBoxResult BuildAndGrantReward(BlindBoxRewardType type)
    {
        var result = new BlindBoxResult { type = type };

        switch (type)
        {
            case BlindBoxRewardType.Tokens:
                int amount = tokensConfigDatabase != null ? tokensConfigDatabase.PickByWeight() : 1;
                amount = Mathf.Max(1, amount);
                result.amount = amount;
                if (WalletController.Instance != null)
                    WalletController.Instance.AddTokens(amount);
                else
                    Debug.LogWarning("[BlindBoxManager] WalletController.Instance is null.");
                break;

            case BlindBoxRewardType.Modifier:
                if (modifierConfigDatabase == null)
                {
                    Debug.LogWarning("[BlindBoxManager] ModifierConfigDatabase not set.");
                    return null;
                }
                var modData = modifierConfigDatabase.PickByWeight();
                if (modData == null)
                {
                    Debug.LogWarning("[BlindBoxManager] No modifier in config.");
                    return null;
                }
                result.payloadId = modData.id;
                result.gainedModifier = modData;
                if (InventoryManager.Instance != null)
                    InventoryManager.Instance.AddModifierById(modData.id, modData.level);
                else
                    Debug.LogWarning("[BlindBoxManager] InventoryManager.Instance is null.");
                break;

            case BlindBoxRewardType.Skin:
                if (skinConfigDatabase == null)
                {
                    Debug.LogWarning("[BlindBoxManager] SkinConfigDatabase not set.");
                    return null;
                }
                var skinData = skinConfigDatabase.PickByWeight();
                if (skinData == null)
                {
                    Debug.LogWarning("[BlindBoxManager] No skin in config.");
                    return null;
                }
                result.payloadId = skinData.id;
                result.gainedSkin = skinData;
                if (InventoryManager.Instance != null)
                    InventoryManager.Instance.AddSkinById(skinData.id);
                else
                    Debug.LogWarning("[BlindBoxManager] InventoryManager.Instance is null.");
                break;
        }

        return result;
    }
}

