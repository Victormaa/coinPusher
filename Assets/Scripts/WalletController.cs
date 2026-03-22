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

        if (SaveManager.Instance != null)
        {
            points = SaveManager.Instance.Data.points;
            tokens = SaveManager.Instance.Data.tokens;
        }

        if (DebugManager.Instance != null)
        {
            DebugManager.Instance.points = points;
            DebugManager.Instance.tokens = tokens;
        }
    }

    public int points = 0;
    public int tokens = 0;

    private void FlushToSave()
    {
        if (SaveManager.Instance == null) return;
        SaveManager.Instance.Data.points = points;
        SaveManager.Instance.Data.tokens = tokens;
        SaveManager.Instance.RequestSave();
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
            DebugManager.Instance.tokens = tokens;

        FlushToSave();
    }

    /// <summary>
    /// 扣除一定数量的 token，并同步更新 DebugManager 的显示。
    /// </summary>
    public void SubTokens(int amount)
    {
        if (amount <= 0) return;

        tokens -= amount;
        if (tokens < 0) tokens = 0;

        if (DebugManager.Instance != null)
            DebugManager.Instance.tokens = tokens;

        FlushToSave();
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
            DebugManager.Instance.points = points;

        FlushToSave();
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
            DebugManager.Instance.points = points;

        FlushToSave();
    }
}
