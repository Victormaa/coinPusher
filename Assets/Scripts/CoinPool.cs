using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class CoinPool : MonoBehaviour
{
    public static CoinPool Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    [Header("硬币对象池")]
    public GameObject coinPrefab;                             // 硬币预制体
    public int poolSize = 100;                                // 初始数量
    private Queue<GameObject> pool = new Queue<GameObject>(); // 对象池, FIFO

    // 阶梯扩容：第一次+100，第二次+50，其后每次+25
    private int expandStep = 0;                               // 已经扩容过的次数

    [Tooltip("发射力度")]    
    public float launchForce = 5f;                           // 投掷硬币的初始力
    private Vector3 spawnPos; 

    /// <summary>
    /// 根据当前扩容阶段返回本次应扩充的硬币数量。
    /// 第一次：100，第二次：50，其后每次：25。
    /// </summary>
    private int GetNextExpandAmount()
    {
        if (expandStep == 0) return 100;
        if (expandStep == 1) return 50;
        return 25;
    }

    void Start()
    {
        // 初始化对象池
        Transform coinsParent = transform.Find("Coins Pool");
        for (int i = 0; i < poolSize; i++)
        {
            GameObject coin = Instantiate(coinPrefab, coinsParent);
            coin.SetActive(false);
            pool.Enqueue(coin);
        }
        // 获取其它变量
        spawnPos = DaoGuiController.Instance.spawnPoint.position;
    }

    // 投硬币
    public void DropCoin()
    {
        // 没有 token 就直接返回
        if (WalletController.Instance.tokens <= 0)
        {
            return;
        }

        // 如果对象池空了，按阶梯规则批量扩容，防止因为池子耗尽而无法投币
        if (pool.Count == 0)
        {
            Transform coinsParent = transform.Find("Coins Pool");

            int amountToAdd = GetNextExpandAmount();

            for (int i = 0; i < amountToAdd; i++)
            {
                GameObject newCoin = Instantiate(coinPrefab, coinsParent);
                newCoin.SetActive(false);
                pool.Enqueue(newCoin);
            }

            poolSize += amountToAdd;   // 更新池子容量统计
            expandStep++;              // 记录已经扩容过一次
        }

        GameObject coin = pool.Dequeue();                         // 从池子里取出一个coin       
        coin.transform.position = spawnPos;                       // 设置位置
        coin.transform.rotation = Quaternion.Euler(-90f, 0f, 0f); // 设置旋转

        WalletController.Instance.tokens--;                      // 扣除一个token
        DebugManager.Instance.tokens = WalletController.Instance.tokens; // 同步更新到Debug面板

        coin.SetActive(true);

        // 重置硬币的物理状态
        Rigidbody rb = coin.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            Vector3 launchDirection = -DaoGuiController.Instance.transform.up;
            rb.AddForce(launchDirection * launchForce, ForceMode.Impulse);
        }
    }

    // 回收硬币
    public void RecycleCoin(GameObject coin)
    {
        coin.SetActive(false); // 隐藏硬币
        pool.Enqueue(coin);    // 放回池子
    }
    

    // Debug
    void Update()
    {
        DebugManager.Instance.coinsInScene = poolSize - pool.Count;
    }
    

}
