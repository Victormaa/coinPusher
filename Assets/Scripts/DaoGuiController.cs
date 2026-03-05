using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaoGuiController : MonoBehaviour
{   
    public static DaoGuiController Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public float rotationSpeed = 100f;   // 旋转速度
    public float maxRotationAngle = 30f; // 最大旋转角度
    public Transform spawnPoint;         // 硬币生成点

    void Update()
    {
        float angle = Mathf.Sin(Time.time * rotationSpeed) * maxRotationAngle;
        transform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}
