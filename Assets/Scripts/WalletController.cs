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
    }

    public int points = 0; 
    public int tokens = 0;

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

    // 后续如果有 points 的加减，也可以用类似的封装来统一更新 Debug UI。
}
