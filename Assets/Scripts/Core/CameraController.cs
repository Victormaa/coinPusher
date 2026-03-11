using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("用于把鼠标屏幕点转换为射线的 Camera；为空则尝试用本物体上的 Camera，再不行用 Camera.main。")]
    public Camera targetCamera;

    [Tooltip("若相机在一个父物体(相机Rig)下，建议勾选并指定 rigRoot；平移会作用在 rigRoot 上。")]
    public bool useRigRoot = false;
    public Transform rigRoot;

    [Header("Pan (RMB Drag)")]
    [Tooltip("拖拽参考平面高度：y = planeY。一般设为桌面表面高度。")]
    public float planeY = 0f;

    [Tooltip("拖拽灵敏度倍率。1=真实一比一拖拽；>1 更灵敏。")]
    [Min(0f)]
    public float panMultiplier = 1f;

    [Header("Optional Clamp")]
    public bool clampPosition = false;
    public Vector2 clampX = new Vector2(-5f, 5f);
    public Vector2 clampZ = new Vector2(-5f, 5f);

    private bool _dragging;
    //private Vector3 _dragStartWorld;
    private Vector3 _lastMouseWorld; // 替代原来的 _dragStartWorld

    private Transform MoveTarget => (useRigRoot && rigRoot != null) ? rigRoot : transform;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = GetComponent<Camera>();
        }

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (targetCamera == null) return;

        // 1. 鼠标按下：记录初始落点
        if (Input.GetMouseButtonDown(1))
        {
            if (TryGetMouseWorldOnPlane(out _lastMouseWorld))
            {
                _dragging = true;
            }
        }

        // 2. 鼠标抬起：停止拖拽
        if (Input.GetMouseButtonUp(1))
        {
            _dragging = false;
        }

        if (!_dragging) return;

        // 3. 鼠标拖拽中：计算【当前帧】的射线落点
        if (!TryGetMouseWorldOnPlane(out var dragNowWorld)) return;

        // 计算当前鼠标位置与上一帧鼠标位置的差值
        var delta = (dragNowWorld - _lastMouseWorld) * panMultiplier;

        // 直接从【当前相机位置】减去这个差值
        var targetPos = MoveTarget.position - delta;

        // --- 强烈建议：锁死 Y 轴 ---
        // 因为你的平面是倾斜的(-forward)，delta包含Y轴位移。
        // 如果你不锁死Y轴，相机会钻进地里或者飞上天。
        //targetPos.y = MoveTarget.position.y;

        // 应用范围限制
        if (clampPosition)
        {
            targetPos.x = Mathf.Clamp(targetPos.x, Mathf.Min(clampX.x, clampX.y), Mathf.Max(clampX.x, clampX.y));
            targetPos.z = Mathf.Clamp(targetPos.z, Mathf.Min(clampZ.x, clampZ.y), Mathf.Max(clampZ.x, clampZ.y));
        }

        // 移动相机
        MoveTarget.position = targetPos;

        // 4. 【解决抖动的终极秘诀：重新校准】
        // 因为相机移动了，如果不更新参考点，下一帧射线会算错！
        // 我们必须在相机移动后，重新获取一次落点，作为下一帧的对比基准。
        TryGetMouseWorldOnPlane(out _lastMouseWorld);
    }

    private bool TryGetMouseWorldOnPlane(out Vector3 worldPoint)
    {
        var ray = targetCamera.ScreenPointToRay(Input.mousePosition);

        var localUp = -this.transform.forward;

        var plane = new Plane(localUp, new Vector3(0f, planeY, 0f));

        if (!plane.Raycast(ray, out var enter))
        {
            worldPoint = default;
            return false;
        }

        worldPoint = ray.GetPoint(enter);

        // --- DEBUG 可视化代码 ---
        // 画出射线（红色）
        Debug.DrawLine(ray.origin, worldPoint, Color.red);
        // 在交点处画一个十字（绿色）
        Debug.DrawLine(worldPoint + Vector3.left, worldPoint + Vector3.right, Color.green);
        Debug.DrawLine(worldPoint + Vector3.forward, worldPoint + Vector3.back, Color.green);
        // --- END DEBUG ---

        return true;
    }
}
