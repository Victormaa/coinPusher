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

    [Tooltip("发射力度")]    
    public float launchForce = 5f;                           // 投掷硬币的初始力
    private Vector3 spawnPos; 

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
        if (pool.Count > 0 && WalletController.Instance.tokens > 0)
        {
            GameObject coin = pool.Dequeue();                         // 从池子里取出一个coin       
            coin.transform.position = spawnPos;                       // 设置位置
            coin.transform.rotation = Quaternion.Euler(-90f, 0f, 0f); // 设置旋转
            WalletController.Instance.tokens --;                      // 扣除一个token
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
