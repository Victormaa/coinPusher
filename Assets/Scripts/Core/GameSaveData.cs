using System;
using System.Collections.Generic;
using Newtonsoft.Json;

/// <summary>
/// 统一存档数据结构，使用 Newtonsoft.Json 序列化到 JSON 文件。
/// </summary>
[Serializable]
public class GameSaveData
{
    public int points;
    public int tokens;

    [JsonProperty("modifiers")]
    public List<InventoryModifierSaveData> modifiers = new List<InventoryModifierSaveData>();

    [JsonProperty("skins")]
    public List<string> skins = new List<string>();

    [JsonProperty("equippedModifierIds")]
    public string[] equippedModifierIds = new string[3];

    public float specialCoinTimer;
    public int globalInputCounter;
}

[Serializable]
public class InventoryModifierSaveData
{
    public string id;
    public int level;
}
