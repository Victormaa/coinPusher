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
    
    [Header("投币范围")]
    public GameObject sprawnRange;                            // 范围对象
    private float xMin, xMax, y, z;                           // 投币边界

    [Header("硬币手感调整")]
    [Range(1f, 10f)] public float coinMass = 1f;                             // 硬币质量
    [Range(0f, 5f)] public float coinDrag = 0.1f;                             // 硬币阻力
    [Range(0f, 5f)] public float coinAngularDrag = 0.05f;                     // 硬币角阻力
    [Range(0f, 5f)] public float coinDynamicFriction = 0.4f;                  // 硬币动摩擦力
    [Range(0f, 5f)] public float coinStaticFriction = 0.6f;                   // 硬币静摩擦力
    [Range(0f, 0.5f)] public float coinBounciness = 0.5f;                       // 硬币弹性  
    [Header("推币机手感调整")]
    [Range(0f, 5)] public float pusherDynamicFriction = 0.2f;                // 推板动摩擦力
    [Range(0f, 5f)] public float pusherStaticFriction = 0.2f;                 // 推板静摩擦力
    [Range(0f, 0.5f)] public float pusherBounciness = 0.5f;                     // 推板弹性 
    private Rigidbody coinRb;
    private PhysicMaterial coinMat;
    private PhysicMaterial pusherMat;

    void Start()
    {
        // 初始化对象池
        Transform coinsParent = transform.Find("Coins");
        for (int i = 0; i < poolSize; i++)
        {
            GameObject coin = Instantiate(coinPrefab, coinsParent);
            coin.SetActive(false);
            pool.Enqueue(coin);
        }
        // 获取投币范围
        Collider col = sprawnRange.GetComponent<BoxCollider>();
        xMin = col.bounds.min.x;
        xMax = col.bounds.max.x;
        y = col.bounds.center.y;
        z = col.bounds.center.z;
    }

    // 投硬币
    public GameObject DropCoin()
    {
        if (pool.Count > 0)
        {
            GameObject coin = pool.Dequeue();                         // 从池子里取出一个coin
            float randomX = Random.Range(xMin, xMax);                 // 随机生成X坐标
            coin.transform.position = new Vector3(randomX, y, z);     // 设置位置
            coin.transform.rotation = Quaternion.Euler(-90f, 0f, 0f); // 设置旋转
            // 重置硬币的物理状态
            Rigidbody rb = coin.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            coin.SetActive(true);
            return coin;
        }
        else
        {
            // 如果池子空了 ------------------------------------
            return null; 
        }
    }

    // 回收硬币
    public void RecycleCoin(GameObject coin)
    {
        coin.SetActive(false); // 隐藏硬币
        pool.Enqueue(coin);    // 放回池子
    }
    
    // 手感调整
    private void OnValidate()
    {
        if (coinRb == null || coinMat == null || pusherMat == null)
        {
            coinRb = coinPrefab.GetComponent<Rigidbody>();
            coinMat = coinPrefab.GetComponent<BoxCollider>().sharedMaterial;
            pusherMat = GameObject.Find("PushBoard").GetComponent<BoxCollider>().sharedMaterial;
        }
        coinRb.mass = coinMass;
        coinRb.drag = coinDrag;
        coinRb.angularDrag = coinAngularDrag;
        coinMat.bounciness = coinBounciness;
        coinMat.dynamicFriction = coinDynamicFriction;
        coinMat.staticFriction = coinStaticFriction;
        pusherMat.dynamicFriction = pusherDynamicFriction;
        pusherMat.staticFriction = pusherStaticFriction;
        pusherMat.bounciness = pusherBounciness;
    }

/*
    // Debug
    void Update()
    {
        Debug.Log($"当前对象池中可用的硬币数量: {pool.Count}");
    }
    
*/
}
