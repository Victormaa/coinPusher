using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems; // 引用这个以支持UI检测

public class Transparent2 : MonoBehaviour
{
   // --- Windows API 定义 ---
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    private struct MARGINS { public int cxLeftWidth; public int cxRightWidth; public int cyTopHeight; public int cyBottomHeight; }

    // --- 常量定义 ---
    const int GWL_EXSTYLE = -20;
    const uint WS_EX_LAYERED = 0x00080000;
    const uint WS_EX_TRANSPARENT = 0x00000020; // 鼠标穿透属性
    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

    // --- 变量 ---
    private IntPtr hWnd;
    private bool isWindowClickable = true; // 记录当前窗口状态
    private Camera mainCamera;

    // 可选：指定哪些层是可以点击的
    public LayerMask clickableLayers; 

    void Start()
    {
        mainCamera = Camera.main;

#if !UNITY_EDITOR
        hWnd = GetActiveWindow();

        // 1. 开启 Alpha 通道透明 (视觉)
        MARGINS margins = new MARGINS { cxLeftWidth = -1 };
        DwmExtendFrameIntoClientArea(hWnd, ref margins);

        // 2. 初始化窗口样式
        // 默认先设为 Layered，不加 Transparent，保证一开始能被点到
        SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
        SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, 0);
#endif
    }

    void Update()
    {
#if !UNITY_EDITOR
        // 检测鼠标是否悬停在“有意义”的东西上
        bool isHovering = CheckIfHovering();

        // 只有当状态发生改变时，才去调用 Windows API (优化性能)
        if (isHovering != isWindowClickable)
        {
            isWindowClickable = isHovering;
            SetClickThrough(!isWindowClickable);
        }
#endif
    }

    // 检测鼠标下面有没有东西
    private bool CheckIfHovering()
    {
        // 1. 优先检测 UI (如果你有按钮的话)
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }

        // 2. 发射射线检测 3D 物体
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        // 如果你的乌龟有 Collider，射线就会击中它
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, clickableLayers)) 
        {
            return true;
        }

        return false;
    }

    // 设置是否穿透
    private void SetClickThrough(bool clickThrough)
    {
        if (clickThrough)
        {
            // 开启穿透：鼠标点不到窗口
            SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
        }
        else
        {
            // 关闭穿透：鼠标可以点击窗口
            SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
        }
    }
}