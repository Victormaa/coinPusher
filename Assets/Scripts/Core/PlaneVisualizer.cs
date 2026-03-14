using UnityEngine;

public class PlaneVisualizer : MonoBehaviour
{
    public float planeY = 0f;
    private GameObject visualPlane; // 用于显示的真实平面

    void Update()
    {
        // 1. 这里的逻辑和你提供的数学逻辑一致
        Vector3 localUp = -this.transform.forward;
        //Vector3 localUp = Vector3.up;
        Vector3 pointOnPlane = new Vector3(0f, planeY, 0f);

        // 数学平面（用于计算）
        //var plane = new Plane(localUp, pointOnPlane);

        // 2. 在 Runtime 生成或更新真实的可见平面
        UpdateVisualPlane(pointOnPlane, localUp);
    }

    private void UpdateVisualPlane(Vector3 position, Vector3 normal)
    {
        if (visualPlane == null)
        {
            // 如果不存在，创建一个 Unity 原生平面
            visualPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            visualPlane.name = "DEBUG_PLANE";
            // 取消碰撞体，防止干扰射线
            Destroy(visualPlane.GetComponent<MeshCollider>());
            // 设置一个半透明材质（可选）
            visualPlane.GetComponent<MeshRenderer>().material.color = new Color(0, 1, 1, 0.5f);
        }

        // --- 核心同步逻辑 ---

        // A. 位置：设置在点 (0, planeY, 0)
        visualPlane.transform.position = position;

        // B. 旋转：让平面的正面（Y轴）指向你的 localUp 向量
        // Unity 的 Plane 默认向上，所以我们修改它的 up 指向我们的法线
        visualPlane.transform.up = normal;

        // C. 大小：数学平面是无限的，我们把可见平面改大一点
        visualPlane.transform.localScale = new Vector3(10f, 1f, 10f);
    }
}