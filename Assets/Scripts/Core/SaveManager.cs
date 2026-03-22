using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// 统一存档管理，使用 Newtonsoft.Json 将 GameSaveData 持久化到 JSON 文件。
/// 各管理器从 Data 读取、通过 RequestSave 触发保存；内部防抖，避免频繁写盘。
/// </summary>
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const string SAVE_FILENAME = "save.json";
    private const float SAVE_THROTTLE_SECONDS = 2f;

    private GameSaveData _data = new GameSaveData();
    private string _savePath;
    private float _lastSaveTime = -999f;

    public GameSaveData Data => _data;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateInstance()
    {
        if (Instance != null) return;

        var go = new GameObject("SaveManager");
        go.AddComponent<SaveManager>();
        DontDestroyOnLoad(go);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _savePath = Path.Combine(Application.persistentDataPath, SAVE_FILENAME);
        Load();
    }

    private void OnApplicationQuit()
    {
        Save(force: true);
    }

    /// <summary>
    /// 请求保存；内部防抖，2 秒内重复调用仅写入一次（退出时强制写入）。
    /// </summary>
    public void RequestSave()
    {
        Save(force: false);
    }

    private void Load()
    {
        ClearOldPlayerPrefs();

        if (File.Exists(_savePath))
        {
            try
            {
                string json = File.ReadAllText(_savePath, Encoding.UTF8);
                var loaded = JsonConvert.DeserializeObject<GameSaveData>(json);
                if (loaded != null)
                    _data = loaded;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SaveManager] Failed to load save: {e.Message}");
            }
        }
    }

    private void Save(bool force)
    {
        float now = Time.realtimeSinceStartup;
        if (!force && (now - _lastSaveTime) < SAVE_THROTTLE_SECONDS)
            return;

        _lastSaveTime = now;

        try
        {
            string json = JsonConvert.SerializeObject(_data, Formatting.Indented);
            string dir = Path.GetDirectoryName(_savePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllText(_savePath, json, Encoding.UTF8);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[SaveManager] Failed to save: {e.Message}");
        }
    }

    /// <summary>
    /// 首次无 JSON 文件时，尝试从 PlayerPrefs 迁移旧数据。
    /// </summary>
    private void MigrateFromPlayerPrefs()
    {
        bool migrated = false;

        if (PlayerPrefs.HasKey("Wallet_Points"))
        {
            _data.points = PlayerPrefs.GetInt("Wallet_Points", 0);
            migrated = true;
        }

        if (PlayerPrefs.HasKey("Wallet_Tokens"))
        {
            _data.tokens = PlayerPrefs.GetInt("Wallet_Tokens", 0);
            migrated = true;
        }

        if (PlayerPrefs.HasKey("SpecialCoinTimer"))
        {
            _data.specialCoinTimer = PlayerPrefs.GetFloat("SpecialCoinTimer", 0f);
            migrated = true;
        }

        if (PlayerPrefs.HasKey("GlobalInput_TotalInputs"))
        {
            _data.globalInputCounter = PlayerPrefs.GetInt("GlobalInput_TotalInputs", 0);
            migrated = true;
        }

        if (PlayerPrefs.HasKey("Inventory_Data"))
        {
            try
            {
                string json = PlayerPrefs.GetString("Inventory_Data", "");
                if (!string.IsNullOrEmpty(json))
                {
                    var inv = JsonConvert.DeserializeObject<InventorySaveDataStub>(json);
                    if (inv != null)
                    {
                        _data.modifiers = inv.modifiers ?? new System.Collections.Generic.List<InventoryModifierSaveData>();
                        _data.skins = inv.skins ?? new System.Collections.Generic.List<string>();
                        if (inv.equippedModifierIds != null && inv.equippedModifierIds.Length == 3)
                            _data.equippedModifierIds = inv.equippedModifierIds;
                        migrated = true;
                    }
                }
            }
            catch { /* ignore */ }
        }

        if (migrated)
        {
            Save(force: true);
            Debug.Log("[SaveManager] Migrated from PlayerPrefs to JSON save.");
        }
    }
    /// <summary>
    /// 清理旧的 PlayerPrefs 存档键，避免与新 JSON 存档并存。
    /// </summary>
    private void ClearOldPlayerPrefs()
    {
        PlayerPrefs.DeleteKey("Wallet_Points");
        PlayerPrefs.DeleteKey("Wallet_Tokens");
        PlayerPrefs.DeleteKey("SpecialCoinTimer");
        PlayerPrefs.DeleteKey("GlobalInput_TotalInputs");
        PlayerPrefs.DeleteKey("Inventory_Data");
        PlayerPrefs.Save();
    }
    [Serializable]
    private class InventorySaveDataStub
    {
        public System.Collections.Generic.List<InventoryModifierSaveData> modifiers;
        public System.Collections.Generic.List<string> skins;
        public string[] equippedModifierIds;
    }
}
