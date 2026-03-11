using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 在 ABTest 场景的推币机台面上预置一批硬币，用于 A/B 测试。
/// 不消耗玩家 tokens，直接实例化硬币并放置在台面上方的网格随机位置。
/// </summary>
public class ABTestCoinSpawner : MonoBehaviour
{
    [Header("台面边界")]
    [Tooltip("推币机台面的 Collider（通常是 dropPlatform 或其子物体上的 BoxCollider）")]
    public Collider tableSurface;

    [Header("生成参数")]
    [Tooltip("初始在台面上生成的硬币数量")]
    public int coinCount = 150;

    [Tooltip("硬币相对台面最高点的高度偏移，避免穿模")]
    public float heightOffset = 0.02f;

    [Tooltip("仅在指定场景名时执行（为空则所有场景都执行）")]
    public string runOnlyInScene = "ABTest";

    private void Start()
    {
        // 如果设置了场景名，只在对应场景中执行
        if (!string.IsNullOrEmpty(runOnlyInScene))
        {
            var scene = SceneManager.GetActiveScene();
            if (!scene.name.Equals(runOnlyInScene))
            {
                return;
            }
        }

        if (tableSurface == null)
        {
            Debug.LogWarning("[ABTestCoinSpawner] tableSurface 未设置，无法生成预置硬币。");
            return;
        }

        if (CoinPool.Instance == null)
        {
            Debug.LogWarning("[ABTestCoinSpawner] CoinPool.Instance 不存在，无法生成预置硬币。");
            return;
        }

        if (coinCount <= 0)
        {
            return;
        }

        Bounds bounds = tableSurface.bounds;

        // 计算网格尺寸，使得网格数接近 coinCount
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(coinCount));

        Transform parent = CoinPool.Instance.transform.Find("Coins Pool");
        if (parent == null)
        {
            parent = CoinPool.Instance.transform;
        }

        GameObject prefab = CoinPool.Instance.coinPrefab;
        if (prefab == null)
        {
            Debug.LogWarning("[ABTestCoinSpawner] CoinPool.coinPrefab 未设置，无法生成预置硬币。");
            return;
        }

        for (int i = 0; i < coinCount; i++)
        {
            int gx = i % gridSize;
            int gy = i / gridSize;

            // 防御：当 coinCount > gridSize * gridSize 时，简单回绕行索引
            if (gy >= gridSize)
            {
                gy = gy % gridSize;
            }

            // 在每个网格内加入一定随机抖动，让分布更自然
            float u = (gx + Random.Range(0.2f, 0.8f)) / gridSize;
            float v = (gy + Random.Range(0.2f, 0.8f)) / gridSize;

            float x = Mathf.Lerp(bounds.min.x, bounds.max.x, u);
            float z = Mathf.Lerp(bounds.min.z, bounds.max.z, v);
            float y = bounds.max.y + heightOffset;

            Vector3 position = new Vector3(x, y, z);

            GameObject coin = Instantiate(
                prefab,
                position,
                Quaternion.Euler(-90f, 0f, 0f),
                parent
            );

            Rigidbody rb = coin.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            coin.SetActive(true);
        }

        DebugManager.Instance.tokens = coinCount;
    }
}

