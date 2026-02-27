using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("输入总次数")]
    public int counter = 0;

    void Update()
    {
        // 检测任意键盘按键或鼠标点击（左、中、右键等）
        if (Input.anyKeyDown)
        {
            counter++;
            DebugManager.Instance.counter = counter;
            Debug.Log($"检测到输入！当前 counter 的值为: {counter}");
        }
    }
}