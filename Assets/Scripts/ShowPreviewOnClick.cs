using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShowPreviewOnClick : MonoBehaviour
{
    [Header("UI 组件引用")]
    public Button triggerButton;      // 拖入点击按钮，例如 TextButton2
    public GameObject previewObject;  // 拖入用于显示的图片 GameObject，比如 LevelPreviewImage

    [Header("显示设置")]
    public float displayDuration = 10f;     // 显示时长（秒）

    void Awake()
    {
        if (triggerButton == null || previewObject == null)
        {
            Debug.LogError("[ShowPreviewOnClick] 请在 Inspector 中关联 triggerButton 和 previewObject！");
            return;
        }

        // 初始时隐藏图片对象
        previewObject.SetActive(false);

        // 按钮点击监听
        triggerButton.onClick.AddListener(OnButtonClicked);
    }

    void OnDestroy()
    {
        // 清理监听
        if (triggerButton != null)
            triggerButton.onClick.RemoveListener(OnButtonClicked);
    }

    void OnButtonClicked()
    {
        // 停止所有正在进行的协程，避免重复点击累积
        StopAllCoroutines();
        // 启动新的显示流程
        StartCoroutine(ShowThenHide());
    }

    IEnumerator ShowThenHide()
    {
        // 显示图片
        previewObject.SetActive(true);

        // 等待指定秒数
        yield return new WaitForSeconds(displayDuration);

        // 隐藏图片
        previewObject.SetActive(false);
    }
}
