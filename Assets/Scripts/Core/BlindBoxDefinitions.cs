using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 与盲盒系统相关的通用数据结构和枚举。
/// 这些类型被盲盒管理器、背包以及配置数据库共同使用。
/// </summary>
public enum BlindBoxRewardType
{
    Skin,
    Modifier,
    Tokens,
}

/// <summary>
/// 一条盲盒奖励配置，用于权重随机。
/// </summary>
[Serializable]
public class BlindBoxRewardEntry
{
    [Tooltip("奖励类型：皮肤 / 装备 / 代币")]
    public BlindBoxRewardType type = BlindBoxRewardType.Tokens;

    [Tooltip("数量：对 Tokens 是加多少，对其他类型通常填 1。")]
    public int amount = 1;

    [Tooltip("权重：数值越大，被抽中的概率越高。")]
    public float weight = 1f;

    [Tooltip("额外载荷 ID，比如皮肤 ID 或装备 ID。Tokens 类型可以留空。")]
    public string payloadId;
}

/// <summary>
/// 一次盲盒抽取的结果，用于逻辑层和 UI 展示。
/// </summary>
public class BlindBoxResult
{
    public BlindBoxRewardType type;
    public int amount;
    public string payloadId;

    public SlotModifierData gainedModifier;
    public SkinData gainedSkin;
}

/// <summary>
/// 推币机装备（配件）的抽象数据，用于背包和配置。
/// </summary>
[Serializable]
public class SlotModifierData
{
    [Tooltip("唯一 ID，例如 mod_pusher_speed_lv1")]
    public string id;

    [Tooltip("等级：1 / 2 / 3")]
    public int level = 1;

    [Header("数值效果")]
    public float sideTiltOffset;
    public float pusherSpeedMultiplier = 1f;
    public float dropTiltOffset;
}

/// <summary>
/// 推币机皮肤的数据结构。
/// </summary>
[Serializable]
public class SkinData
{
    [Tooltip("唯一 ID，例如 skin_gold")]
    public string id;

    [Tooltip("展示名称")]
    public string displayName;
}

