using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    public bool isRewardZone = false;
    void OnTriggerEnter(Collider other)
    {
        // 如果硬币进入DeadZone，则将其回收到对象池
        if (other.CompareTag("Coin"))
        {
            CoinPool.Instance.RecycleCoin(other.gameObject);
            if (isRewardZone)
            {
                WalletController.Instance.AddTokens(1); // 进入奖励区，获得tokens（内部已同步更新到Debug面板）
            }
        }

        else if (other.CompareTag("SpecialCoin"))
        {
            SpecialCoinPool.Instance.RecycleSpecialCoin(other.gameObject);
            if (isRewardZone)
            {
                /////////// 特殊币奖励 ////////////////////
            }
        }
    }
}
