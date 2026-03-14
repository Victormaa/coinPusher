using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家背包：管理已拥有的装备和皮肤，以及当前装备到推币机槽位上的配件。
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private const string INVENTORY_KEY = "Inventory_Data";

    [Header("配置数据库引用")]
    public ModifierConfigDatabase modifierDatabase;
    public SkinConfigDatabase skinDatabase;

    [Header("当前拥有的装备/皮肤")]
    public List<SlotModifierData> ownedModifiers = new List<SlotModifierData>();
    public List<SkinData> ownedSkins = new List<SkinData>();

    [Header("当前已装备到推币机的装备 ID（3 个槽位）")]
    public string[] equippedModifierIds = new string[3];

    [Serializable]
    private class InventoryModifierSaveData
    {
        public string id;
        public int level;
    }

    [Serializable]
    private class InventorySaveData
    {
        public List<InventoryModifierSaveData> modifiers = new List<InventoryModifierSaveData>();
        public List<string> skins = new List<string>();
        public string[] equippedModifierIds = new string[3];
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        LoadFromPrefs();
    }

    /// <summary>
    /// 根据 ID 通过配置表创建一件装备并加入背包，返回新建的数据。
    /// </summary>
    public SlotModifierData AddModifierById(string id, int level = -1)
    {
        if (modifierDatabase == null)
        {
            Debug.LogWarning("[InventoryManager] ModifierConfigDatabase not set.");
            return null;
        }

        var config = modifierDatabase.GetById(id, level);
        if (config == null)
        {
            Debug.LogWarning($"[InventoryManager] No modifier config found for id={id}, level={level}.");
            return null;
        }

        // 创建一个副本放入背包，避免直接引用配置表对象
        var copy = new SlotModifierData
        {
            id = config.id,
            level = config.level,
            sideTiltOffset = config.sideTiltOffset,
            pusherSpeedMultiplier = config.pusherSpeedMultiplier,
            dropTiltOffset = config.dropTiltOffset
        };

        ownedModifiers.Add(copy);
        SaveToPrefs();
        return copy;
    }

    /// <summary>
    /// 根据 ID 通过配置表创建一件皮肤并加入背包，返回新建的数据。
    /// </summary>
    public SkinData AddSkinById(string id)
    {
        if (skinDatabase == null)
        {
            Debug.LogWarning("[InventoryManager] SkinConfigDatabase not set.");
            return null;
        }

        var config = skinDatabase.GetById(id);
        if (config == null)
        {
            Debug.LogWarning($"[InventoryManager] No skin config found for id={id}.");
            return null;
        }

        var copy = new SkinData
        {
            id = config.id,
            displayName = config.displayName
        };

        ownedSkins.Add(copy);
        SaveToPrefs();
        return copy;
    }

    /// <summary>
    /// 把某个装备 ID 装备到指定槽位，仅记录 ID，真实数值由配置表提供。
    /// </summary>
    public void EquipModifierToSlot(int slotIndex, string modifierId)
    {
        if (slotIndex < 0 || slotIndex >= equippedModifierIds.Length)
            return;

        equippedModifierIds[slotIndex] = modifierId;
        SaveToPrefs();
    }

    /// <summary>
    /// 返回根据当前装备 ID 列表生成的 SlotModifierData 集合（长度为 3，可包含 null）。
    /// </summary>
    public SlotModifierData[] GetEquippedModifiers()
    {
        var result = new SlotModifierData[3];
        if (modifierDatabase == null || equippedModifierIds == null)
            return result;

        for (int i = 0; i < result.Length; i++)
        {
            var id = (equippedModifierIds != null && i < equippedModifierIds.Length) ? equippedModifierIds[i] : null;
            if (string.IsNullOrEmpty(id)) continue;

            var config = modifierDatabase.GetById(id);
            if (config == null) continue;

            result[i] = new SlotModifierData
            {
                id = config.id,
                level = config.level,
                sideTiltOffset = config.sideTiltOffset,
                pusherSpeedMultiplier = config.pusherSpeedMultiplier,
                dropTiltOffset = config.dropTiltOffset
            };
        }

        return result;
    }

    private void LoadFromPrefs()
    {
        if (!PlayerPrefs.HasKey(INVENTORY_KEY))
            return;

        string json = PlayerPrefs.GetString(INVENTORY_KEY, string.Empty);
        if (string.IsNullOrEmpty(json))
            return;

        try
        {
            var data = JsonUtility.FromJson<InventorySaveData>(json);
            if (data == null) return;

            ownedModifiers.Clear();
            ownedSkins.Clear();

            // 还原装备
            if (data.modifiers != null)
            {
                foreach (var m in data.modifiers)
                {
                    if (m == null || string.IsNullOrEmpty(m.id)) continue;
                    var config = modifierDatabase != null ? modifierDatabase.GetById(m.id, m.level) : null;
                    if (config == null) continue;

                    ownedModifiers.Add(new SlotModifierData
                    {
                        id = config.id,
                        level = config.level,
                        sideTiltOffset = config.sideTiltOffset,
                        pusherSpeedMultiplier = config.pusherSpeedMultiplier,
                        dropTiltOffset = config.dropTiltOffset
                    });
                }
            }

            // 还原皮肤
            if (data.skins != null)
            {
                foreach (var id in data.skins)
                {
                    if (string.IsNullOrEmpty(id)) continue;
                    var config = skinDatabase != null ? skinDatabase.GetById(id) : null;
                    if (config == null) continue;

                    ownedSkins.Add(new SkinData
                    {
                        id = config.id,
                        displayName = config.displayName
                    });
                }
            }

            // 还原已装备 ID
            if (data.equippedModifierIds != null && data.equippedModifierIds.Length == equippedModifierIds.Length)
            {
                for (int i = 0; i < equippedModifierIds.Length; i++)
                {
                    equippedModifierIds[i] = data.equippedModifierIds[i];
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[InventoryManager] Failed to load inventory from prefs: {e.Message}");
        }
    }

    private void SaveToPrefs()
    {
        var data = new InventorySaveData();

        // 存储装备：只存 id + level
        if (ownedModifiers != null)
        {
            foreach (var m in ownedModifiers)
            {
                if (m == null || string.IsNullOrEmpty(m.id)) continue;
                data.modifiers.Add(new InventoryModifierSaveData
                {
                    id = m.id,
                    level = m.level
                });
            }
        }

        // 存储皮肤：只存 id
        if (ownedSkins != null)
        {
            foreach (var s in ownedSkins)
            {
                if (s == null || string.IsNullOrEmpty(s.id)) continue;
                data.skins.Add(s.id);
            }
        }

        // 存储已装备 ID
        if (equippedModifierIds != null && equippedModifierIds.Length == 3)
        {
            data.equippedModifierIds = new string[3];
            for (int i = 0; i < 3; i++)
            {
                data.equippedModifierIds[i] = equippedModifierIds[i];
            }
        }

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(INVENTORY_KEY, json);
        PlayerPrefs.Save();
    }
}

