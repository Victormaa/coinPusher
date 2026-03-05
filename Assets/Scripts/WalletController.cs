using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.UIElements;

public class WalletController : MonoBehaviour
{
    public static WalletController Instance { get; private set; }
    private void Awake()
    {
        Instance = this;

        // 从本地存档加载初始数据
        LoadFromPrefs();

        // 把加载到的数值同步给 Debug 面板
        if (DebugManager.Instance != null)
        {
            DebugManager.Instance.points = points;
            DebugManager.Instance.tokens = tokens;
        }
    }

    public int points = 0; 
    public int tokens = 0;

    // --- PlayerPrefs 键名 ---
    private const string POINTS_KEY = "Wallet_Points";
    private const string TOKENS_KEY = "Wallet_Tokens";

    private void LoadFromPrefs()
    {
        if (PlayerPrefs.HasKey(POINTS_KEY))
        {
            points = PlayerPrefs.GetInt(POINTS_KEY, 0);
        }

        if (PlayerPrefs.HasKey(TOKENS_KEY))
        {
            tokens = PlayerPrefs.GetInt(TOKENS_KEY, 0);
        }
    }

    private void SavePoints()
    {
        PlayerPrefs.SetInt(POINTS_KEY, points);
    }

    private void SaveTokens()
    {
        PlayerPrefs.SetInt(TOKENS_KEY, tokens);
    }

    /// <summary>
    /// 增加一定数量的 token，并同步更新 DebugManager 的显示。
    /// </summary>
    /// <param name="amount">要增加的数量，可以为负数但会被夹到 0 以上。</param>
    public void AddTokens(int amount)
    {
        if (amount == 0) return;

        tokens += amount;
        if (tokens < 0) tokens = 0;

        if (DebugManager.Instance != null)
        {
            DebugManager.Instance.tokens = tokens;
        }
    }

    /// <summary>
    /// 扣除一定数量的 token，并同步更新 DebugManager 的显示。
    /// </summary>
    /// <param name="amount">要扣除的数量，自动夹到 0 不会减成负数。</param>
    public void SubTokens(int amount)
    {
        if (amount <= 0) return;

        tokens -= amount;
        if (tokens < 0) tokens = 0;

        if (DebugManager.Instance != null)
        {
            DebugManager.Instance.tokens = tokens;
        }
    }

    /// <summary>
    /// 增加一定数量的 points，并同步更新 DebugManager + 本地存档。
    /// </summary>
    public void AddPoints(int amount)
    {
        if (amount == 0) return;

        points += amount;
        if (points < 0) points = 0;

        if (DebugManager.Instance != null)
        {
            DebugManager.Instance.points = points;
        }
    }

    /// <summary>
    /// 扣除一定数量的 points，并同步更新 DebugManager + 本地存档。
    /// </summary>
    public void SubPoints(int amount)
    {
        if (amount <= 0) return;

        points -= amount;
        if (points < 0) points = 0;

        if (DebugManager.Instance != null)
        {
            DebugManager.Instance.points = points;
        }
    }

    private void OnApplicationQuit()
    {
        // 退出游戏时统一保存一次所有钱包相关的数据
        SavePoints();
        SaveTokens();
        PlayerPrefs.Save();
    }
}
