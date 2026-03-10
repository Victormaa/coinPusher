using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
        // 从本地存档加载初始数据
        LoadFromPrefs();

        // 把加载到的数值同步给 Debug 面板
        if (DebugManager.Instance != null)
        {
            DebugManager.Instance.points = WalletController.Instance.points;
            DebugManager.Instance.tokens = WalletController.Instance.tokens;
        }
    }
    // --- PlayerPrefs 键名 ---
    private const string POINTS_KEY = "Wallet_Points";
    private const string TOKENS_KEY = "Wallet_Tokens";
    private const string SPECIAL_COIN_TIMER_KEY = "SpecialCoinTimer";

    private void LoadFromPrefs()
    {
        if (PlayerPrefs.HasKey(POINTS_KEY))
        {
            WalletController.Instance.points = PlayerPrefs.GetInt(POINTS_KEY, 0);
        }

        if (PlayerPrefs.HasKey(TOKENS_KEY))
        {
            WalletController.Instance.tokens = PlayerPrefs.GetInt(TOKENS_KEY, 0);
        }
        if (PlayerPrefs.HasKey(SPECIAL_COIN_TIMER_KEY))
        {
            SpecialCoinPool.Instance.currentTimer = PlayerPrefs.GetFloat(SPECIAL_COIN_TIMER_KEY, SpecialCoinPool.Instance.timeInterval * 60f);
        }
    }


    private void SavePoints()
    {
        PlayerPrefs.SetInt(POINTS_KEY, WalletController.Instance.points);
    }

    private void SaveTokens()
    {
        PlayerPrefs.SetInt(TOKENS_KEY, WalletController.Instance.tokens);
    }

    private void SaveTimers()
    {
        PlayerPrefs.SetFloat(SPECIAL_COIN_TIMER_KEY, SpecialCoinPool.Instance.currentTimer);
    }

    private void OnApplicationQuit()
    {
        // 退出游戏时统一保存一次所有钱包相关的数据
        SavePoints();
        SaveTokens();
        SaveTimers();
        PlayerPrefs.Save();
    }
}
