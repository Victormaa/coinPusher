using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 盲盒 UI 两段式流程：
/// 1. SpecialCoin 掉入奖励区后显示「可打开盲盒」面板，玩家点击「打开」即确认；
/// 2. 执行抽奖并发奖，短暂展示「获得了 XXX」，然后自动消失。
/// 挂到常驻激活的对象（如 GameManager），并指定 blindBoxRoot 为 BlindBoxOpenCanvas。
/// </summary>
public class BlindBoxUIController : MonoBehaviour
{
    [Header("盲盒 UI 根节点")]
    [Tooltip("整个盲盒 Canvas 或面板根节点，无待开盲盒时会 SetActive(false)")]
    public GameObject blindBoxRoot;

    [Header("阶段一：可打开盲盒")]
    [Tooltip("显示「点击打开盲盒」的面板")]
    public GameObject panelTapToOpen;

    [Tooltip("打开按钮（点击后执行抽奖）")]
    public Button buttonOpen;

    [Tooltip("可选：显示盲盒数量的文本")]
    public TextMeshProUGUI textPendingCount;

    [Header("阶段二：展示结果")]
    [Tooltip("显示抽奖结果的面板")]
    public GameObject panelResult;

    [Tooltip("结果展示文本")]
    public TextMeshProUGUI textResult;

    [Tooltip("结果展示时长（秒）")]
    public float resultDisplayDuration = 2f;

    private bool _isShowingResult;

    private void Start()
    {
        if (buttonOpen != null)
            buttonOpen.onClick.AddListener(OnOpenClicked);

        HideAll();
    }

    private void Update()
    {
        if (BlindBoxManager.Instance == null) return;

        bool shouldShow = BlindBoxManager.Instance.PendingCount > 0 || _isShowingResult;
        if (blindBoxRoot != null)
            blindBoxRoot.SetActive(shouldShow);

        if (_isShowingResult) return;

        if (BlindBoxManager.Instance.PendingCount > 0)
        {
            ShowTapToOpen();
        }
        else
        {
            HideTapToOpen();
        }
    }

    private void OnOpenClicked()
    {
        if (BlindBoxManager.Instance == null) return;
        if (BlindBoxManager.Instance.PendingCount <= 0) return;

        var result = BlindBoxManager.Instance.OpenOnce();
        if (result == null) return;

        if (buttonOpen != null) buttonOpen.gameObject.SetActive(false);
        _isShowingResult = true;
        ShowResult(result);
        StartCoroutine(HideResultAfterDelay());
    }

    private void ShowTapToOpen()
    {
        if (panelTapToOpen != null && !panelTapToOpen.activeSelf)
            panelTapToOpen.SetActive(true);

        if (textPendingCount != null)
        {
            var count = BlindBoxManager.Instance.PendingCount;
            textPendingCount.text = count.ToString();
        }
    }

    private void HideTapToOpen()
    {
        if (panelTapToOpen != null)
            panelTapToOpen.SetActive(false);
    }

    private void ShowResult(BlindBoxResult result)
    {
        var desc = FormatResultText(result);
        if (textResult != null)
            textResult.text = desc;
        else if (textPendingCount != null)
            textPendingCount.text = desc;

        if (panelResult != null)
        {
            if (panelTapToOpen != null) panelTapToOpen.SetActive(false);
            panelResult.SetActive(true);
        }
    }

    private string FormatResultText(BlindBoxResult result)
    {
        if (result == null) return "获得奖励！";

        switch (result.type)
        {
            case BlindBoxRewardType.Tokens:
                return $"获得 {result.amount} 代币！";
            case BlindBoxRewardType.Modifier:
                var modName = result.gainedModifier != null ? $"Lv.{result.gainedModifier.level}" : "装备";
                return $"获得推币机装备 {modName}！";
            case BlindBoxRewardType.Skin:
                var skinName = result.gainedSkin?.displayName ?? result.payloadId ?? "皮肤";
                return $"获得推币机皮肤「{skinName}」！";
            default:
                return "获得奖励！";
        }
    }

    private IEnumerator HideResultAfterDelay()
    {
        yield return new WaitForSeconds(resultDisplayDuration);
        if (panelResult != null)
            panelResult.SetActive(false);
        if (buttonOpen != null) buttonOpen.gameObject.SetActive(true);
        _isShowingResult = false;
    }

    private void HideAll()
    {
        HideTapToOpen();
        if (panelResult != null)
            panelResult.SetActive(false);
        if (blindBoxRoot != null)
            blindBoxRoot.SetActive(false);
    }
}
