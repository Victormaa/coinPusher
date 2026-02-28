using System.Runtime.InteropServices;
using UnityEngine;

public class GlobalInputCounter : MonoBehaviour
{
    // --- Windows API ---
    // GetAsyncKeyState 可以获取指定虚拟键码（Virtual-Key Code）的当前状态
    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    [Header("输入总次数")]
    public int counter = 0;

    // 用来记录上一帧 256 个虚拟按键的状态，用于判断“刚刚按下”的瞬间
    private bool[] keyStates = new bool[256];

    void Update()
    {
        // 遍历所有可能的虚拟键码 (0 到 255 覆盖了所有的鼠标按键、键盘按键及特殊控制键)
        for (int i = 0; i < 256; i++)
        {
            // 如果返回值的最高位是 1 (即与 0x8000 按位与的结果不为 0)，说明该键当前正被按下
            bool isPressed = (GetAsyncKeyState(i) & 0x8000) != 0;

            // 如果当前是按下状态，且上一帧是抬起状态，说明这是一次“新的有效敲击”
            if (isPressed && !keyStates[i])
            {
                counter++;
                CoinPool.Instance.DropCoin(); // 扔币

                // 同步更新到单例中
                if (DebugManager.Instance != null)
                {
                    DebugManager.Instance.counter = counter;
                }

                // 注意：虚拟键码 1 是鼠标左键，2 是鼠标右键，4 是鼠标中键
                // 字母和数字键与 ASCII 码对应
                Debug.Log($"检测到后台全局输入！键码: {i}, 当前 counter 的值为: {counter}");
            }

            // 更新按键状态供下一帧对比
            keyStates[i] = isPressed;
        }
    }
}