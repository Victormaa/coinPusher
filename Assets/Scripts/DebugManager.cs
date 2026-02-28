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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        inputCounter.text = "InputCounter: " + counter;


    }
}
