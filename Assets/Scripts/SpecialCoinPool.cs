using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpecialCoinPool : MonoBehaviour
{
    public static SpecialCoinPool Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    [Header("特殊硬币对象池")]
    public GameObject specialCoinPrefab;                      // 硬币预制体
    public int poolSize = 50;                                 // 初始数量
    private Queue<GameObject> pool = new Queue<GameObject>(); // 对象池, FIFO

    [Header("特殊硬币生成设置")]
    [Tooltip("生成时间（分钟）")]
    public float timeInterval = 30f;
    [Tooltip("投入特殊币所需要的积分")]
    public int pointsCost = 50;
    private bool specialCoinReady = false;
    private float currentTimer;
    
    [Header("投币设置")]
    [Tooltip("发射力度")]                                             
    public float launchForce = 7f;                           // 投掷硬币的初始力
    private Vector3 spawnPos;  

    [Header("倒计时")]  
    public TextMeshProUGUI timerText;
    
    void Start()
    {
        // 初始化对象池
        Transform coinsParent = transform.Find("Special Coins Pool");
        for (int i = 0; i < poolSize; i++)
        {
            GameObject coin = Instantiate(specialCoinPrefab, coinsParent);
            coin.SetActive(false);
            pool.Enqueue(coin);
        }
        
        // 获取其它变量
        currentTimer = timeInterval*60f;       // 转换为秒
        spawnPos = DaoGuiController.Instance.spawnPoint.position;

    }


    public void DropSpecialCoin()
    {
        if (pool.Count > 0 && specialCoinReady && WalletController.Instance.points >= pointsCost)
        {
            specialCoinReady = false;
            currentTimer = timeInterval*60f;                       // 重置计时器
            WalletController.Instance.points -= pointsCost;           // 扣除积分
            DebugManager.Instance.points = WalletController.Instance.points; // 同步更新到Debug面板
            GameObject coin = pool.Dequeue();                         // 从池子里取出一个coin       
            coin.transform.position = spawnPos;                       // 设置位置
            coin.transform.rotation = Quaternion.Euler(-90f, 0f, 0f); // 设置旋转
      
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

    public void RecycleSpecialCoin(GameObject coin)
    {
        coin.SetActive(false); // 隐藏硬币
        pool.Enqueue(coin);    // 放回池子
    }

    // 计时器
    void Update()
    {
        // 如果特殊币已经准备好，就直接reuturn
        if (specialCoinReady)
        {
            return;
        }

        // 如果没有准备好，就继续倒计时
        if (currentTimer > 0)
        {
            currentTimer -= Time.deltaTime;
        }
        else
        {
            specialCoinReady = true;
        }
        timerText.text = currentTimer > 0 ? $"Special Coin Timer: {Mathf.FloorToInt(currentTimer / 60):00}:{Mathf.FloorToInt(currentTimer % 60):00}": "Special Coin Ready, 50 points per drop";
    }
}
