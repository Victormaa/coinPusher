using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DebugManager : MonoBehaviour
{
    private static DebugManager instance;
    public static DebugManager Instance
    {
        get
        {
            // 增加健壮性：如果由于脚本执行顺序导致 instance 还没赋值就被调用，
            // 可以尝试在场景中寻找一次，防止报空指针异常。
            if (instance == null)
            {
                instance = FindObjectOfType<DebugManager>();
            }
            return instance;
        }
    }
    [Header("全局按键/鼠标点击次数")]
    public int counter = 0;
    public TMP_Text inputCounter;

    [Header("当前场景内的硬币数")]
    public int coinsInScene = 0;
    public TMP_Text coinsInSceneCounter;

    [Header("鼠标是否覆盖在可交互对象上")]
    public bool isHovering = false;
    public TMP_Text isHoveringText;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Debug.LogWarning("[DebugManager] 检测到重复实例，已自动销毁。");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        inputCounter.text = "Input Counter: " + counter;
        coinsInSceneCounter.text = "Coins in Scene: " + coinsInScene;
        isHoveringText.text = "is Hovering: " + isHovering.ToString();
    }
}
