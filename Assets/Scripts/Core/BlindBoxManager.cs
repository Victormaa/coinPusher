using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 盲盒系统入口：负责根据权重随机抽取奖励并结算。
/// SpecialCoin 掉入奖励区时由 DeadZone 调用 OpenOnce。
/// </summary>
public class BlindBoxManager : MonoBehaviour
{
    public static BlindBoxManager Instance { get; private set; }

    [Header("盲盒奖励权重表")]
    [Tooltip("配置盲盒可能产出的所有奖励及其权重。")]
    public List<BlindBoxRewardEntry> rewards = new List<BlindBoxRewardEntry>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// 触发一次盲盒抽取，返回本次结果。
    /// </summary>
    public BlindBoxResult OpenOnce()
    {
        var entry = PickReward();
        if (entry == null)
        {
            Debug.LogWarning("[BlindBoxManager] No reward entry picked. Check rewards configuration.");
            return null;
        }

        var result = new BlindBoxResult
        {
            type = entry.type,
            amount = Mathf.Max(1, entry.amount),
            payloadId = entry.payloadId
        };

        // 按类型结算
        switch (entry.type)
        {
            case BlindBoxRewardType.Tokens:
                if (WalletController.Instance != null)
                {
                    WalletController.Instance.AddTokens(result.amount);
                }
                else
                {
                    Debug.LogWarning("[BlindBoxManager] WalletController.Instance is null, cannot grant tokens.");
                }
                break;

            case BlindBoxRewardType.Modifier:
                if (InventoryManager.Instance != null && !string.IsNullOrEmpty(entry.payloadId))
                {
                    for (int i = 0; i < result.amount; i++)
                    {
                        var modData = InventoryManager.Instance.AddModifierById(entry.payloadId);
                        result.gainedModifier = modData; // 记录最后一个，用于 UI 展示
                    }
                }
                else
                {
                    Debug.LogWarning("[BlindBoxManager] Cannot grant modifier, InventoryManager or payloadId missing.");
                }
                break;

            case BlindBoxRewardType.Skin:
                if (InventoryManager.Instance != null && !string.IsNullOrEmpty(entry.payloadId))
                {
                    var skinData = InventoryManager.Instance.AddSkinById(entry.payloadId);
                    result.gainedSkin = skinData;
                }
                else
                {
                    Debug.LogWarning("[BlindBoxManager] Cannot grant skin, InventoryManager or payloadId missing.");
                }
                break;
        }

        return result;
    }

    /// <summary>
    /// 根据权重从奖励表中随机选中一条。
    /// </summary>
    private BlindBoxRewardEntry PickReward()
    {
        if (rewards == null || rewards.Count == 0)
        {
            return null;
        }

        float totalWeight = 0f;
        foreach (var r in rewards)
        {
            if (r == null || r.weight <= 0f) continue;
            totalWeight += r.weight;
        }

        if (totalWeight <= 0f)
        {
            return null;
        }

        float rand = Random.Range(0f, totalWeight);
        float acc = 0f;

        foreach (var r in rewards)
        {
            if (r == null || r.weight <= 0f) continue;

            acc += r.weight;
            if (rand <= acc)
            {
                return r;
            }
        }

        // 理论上不会走到这里，保险起见返回最后一个非空条目
        for (int i = rewards.Count - 1; i >= 0; i--)
        {
            if (rewards[i] != null)
                return rewards[i];
        }

        return null;
    }
}

