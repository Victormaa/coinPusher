using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 推币机整体控制：基础属性 + 插槽配件修正。
/// 把“推板移动速度”等核心数值集中到这里统一管理。
/// </summary>
public class CoinPusherController : MonoBehaviour
{
    [Header("基础属性（可在 Inspector 调整）")]
    [Tooltip("侧板的基础倾角（正数向内倾斜，负数向外）。单位：度")]
    [Range(-30f, 30f)] public float baseSideTiltAngle = 0f;

    [Tooltip("推币台面的基础往复移动速度。")]
    [Range(0.1f, 5f)] public float basePusherSpeed = 1f;

    [Tooltip("落币台面的基础倾角（正数抬高前端，负数降低前端）。单位：度")]
    [Range(-20f, 20f)] public float baseDropTiltAngle = 0f;

    [Header("场景引用")]
    [Tooltip("左侧侧板 Transform")]
    public Transform leftSide;

    [Tooltip("右侧侧板 Transform")]
    public Transform rightSide;

    [Tooltip("落币台面 Transform")]
    public Transform dropPlatform;

    [Tooltip("负责实际往复移动的推板脚本")]
    public PushBoard pushBoard;

    [Header("配件插槽（最多三个）")]
    [Tooltip("三个插槽，每个插槽上的配件可以对推币机属性做加成/修正。")]
    public SlotModifier[] slots = new SlotModifier[3];

    /// <summary>
    /// 单个插槽的修正数据，可以理解为“配件”的效果。
    /// 你可以把不同的配置保存成 Prefab，拖到不同槽位即可。
    /// </summary>
    [System.Serializable]
    public class SlotModifier
    {
        [Tooltip("方便在 Inspector 里区分这个插槽用的名字（例如：左槽/中槽/右槽）")]
        public string slotName = "Slot";

        [Tooltip("是否启用这个插槽的修正效果")]
        public bool enabled = true;

        [Header("对侧板倾角的修正")]
        [Tooltip("在基础侧板倾角的基础上增加或减少的角度（度）。")]
        [Range(-15f, 15f)] public float sideTiltOffset = 0f;

        [Header("对推板速度的修正")]
        [Tooltip("乘法修正，例如 1.2 = 速度提高 20%，0.8 = 降低 20%。")]
        [Range(0.2f, 3f)] public float pusherSpeedMultiplier = 1f;

        [Header("对落币台面倾角的修正")]
        [Tooltip("在基础落币台倾角的基础上增加或减少的角度（度）。")]
        [Range(-10f, 10f)] public float dropTiltOffset = 0f;
    }

    private void Start()
    {
        ApplySettings();
    }

#if UNITY_EDITOR
    // 在编辑器里调 Inspector 数值时就立即更新效果，方便调试“手感”。
    private void OnValidate()
    {
        // 防止在反序列化早期阶段（还没填充完字段）就执行逻辑导致报错。
        if (!Application.isPlaying)
        {
            ApplySettings();
        }
    }
#endif

    /// <summary>
    /// 计算插槽修正后的最终属性，并应用到对应的 Transform / 推板脚本上。
    ///</summary>
    public void ApplySettings()
    {
        // 1. 先从基础属性开始
        float finalSideTilt = baseSideTiltAngle;
        float finalPusherSpeed = basePusherSpeed;
        float finalDropTilt = baseDropTiltAngle;

        // 2. 叠加所有启用中的插槽修正
        if (slots != null)
        {
            foreach (var slot in slots)
            {
                if (slot == null || !slot.enabled) continue;

                finalSideTilt += slot.sideTiltOffset;
                finalDropTilt += slot.dropTiltOffset;
                finalPusherSpeed *= slot.pusherSpeedMultiplier;
            }
        }

        // 3. 把最终数值应用到场景物体上
        ApplySideTilt(finalSideTilt);
        ApplyDropTilt(finalDropTilt);
        ApplyPusherSpeed(finalPusherSpeed);
    }

    /// <summary>
    /// 应用左右侧板的倾角。
    /// 这里假设侧板的本地旋转以 Z 轴为“向内/向外倾斜”，
    /// 左右两侧取相反方向，形成对称的斜面。
    /// </summary>
    private void ApplySideTilt(float angle)
    {
        if (leftSide != null)
        {
            var euler = leftSide.localEulerAngles;
            euler.z = angle;
            leftSide.localEulerAngles = euler;
        }

        if (rightSide != null)
        {
            var euler = rightSide.localEulerAngles;
            euler.z = -angle;
            rightSide.localEulerAngles = euler;
        }
    }

    /// <summary>
    /// 应用落币台面的倾角。
    /// 这里假设绕本地 X 轴旋转可以表示前高后低/前低后高。
    /// </summary>
    private void ApplyDropTilt(float angle)
    {
        if (dropPlatform == null) return;

        var euler = dropPlatform.localEulerAngles;
        euler.x = angle;
        dropPlatform.localEulerAngles = euler;
    }

    /// <summary>
    /// 把最终推板速度同步到 PushBoard 脚本。
    /// 这样场景里实际移动推板的逻辑仍然在 PushBoard 里，
    /// 但所有数值统一由 CoinPusherController 管理。
    /// </summary>
    private void ApplyPusherSpeed(float speed)
    {
        if (pushBoard == null) return;

        pushBoard.speed = Mathf.Max(0f, speed);
    }
}
