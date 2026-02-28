using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // 如果硬币进入DeadZone，则将其回收到对象池
        if (other.CompareTag("Coin"))
        {
            CoinPool.Instance.RecycleCoin(other.gameObject);
        }
    }
}
