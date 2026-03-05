using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyTokens : MonoBehaviour
{
    void Buy(int pointsUsed, int tokensToBuy)
    {
        if (WalletController.Instance.points >= pointsUsed)
        {
            WalletController.Instance.points -= pointsUsed;
            WalletController.Instance.AddTokens(tokensToBuy);
            DebugManager.Instance.points = WalletController.Instance.points; 
        }
        else
        {
            Debug.Log($"购买失败！需要 {pointsUsed} 点数，但当前只有 {WalletController.Instance.points} 点数。");
        }
    }

    public void Buy5Tokens()
    {
        Buy(30, 5);
    }

    public void Buy12Tokens()
    {
        Buy(68, 12);
    }

    public void Buy60Tokens()
    {
        Buy(328, 60);
    }

    public void Buy130Tokens()
    {
        Buy(648, 130);
    }
}
