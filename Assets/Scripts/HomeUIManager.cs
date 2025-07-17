// HomeUIManager.cs
// 目的：处理首页的UI逻辑，如关卡和模式选择。
// 挂载位置：HomeScene中的Canvas或一个UI管理器对象。

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections; // 异步加载需要使用协程，所以必须引入这个命名空间

public class HomeUIManager : MonoBehaviour
{
    [Header("UI 面板")]
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject modeSelectPanel;

    [Header("加载界面 (新增)")]
    [Tooltip("用于显示加载进度的UI面板")]
    [SerializeField] private GameObject loadingScreenPanel; // 拖入你的加载界面Panel
    [Tooltip("用于显示加载进度的滑动条")]
    [SerializeField] private Slider loadingSlider; // 拖入你的加载进度条Slider

    void Start()
    {
        // 初始状态：显示关卡选择，隐藏模式和加载界面
        levelSelectPanel.SetActive(true);
        modeSelectPanel.SetActive(false);
        if (loadingScreenPanel != null)
        {
            loadingScreenPanel.SetActive(false);
        }
    }

    // 此方法由关卡选择按钮调用
    public void OnLevelSelected(int level)
    {
        GameManager.SelectedLevel = level;
        Debug.Log($"选择了关卡: {level}");

        levelSelectPanel.SetActive(false);
        modeSelectPanel.SetActive(true);
    }

    // 此方法由模式选择按钮调用
    public void OnModeSelected(string mode)
    {
        GameManager.SelectedMode = mode;
        Debug.Log($"选择了模式: {mode}");

        // --- 修改点：不再直接加载，而是启动一个协程来异步加载 ---
        StartCoroutine(LoadSceneAsyncRoutine("GameScene"));
    }

    // 新增：用于异步加载场景的协程
    private IEnumerator LoadSceneAsyncRoutine(string sceneName)
    {
        // 1. 显示加载界面
        if (loadingScreenPanel != null)
        {
            loadingScreenPanel.SetActive(true);
        }

        // 2. 开始异步加载新场景
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        // 3. 在场景加载完成前，持续更新进度条
        while (!operation.isDone)
        {
            // operation.progress 的值从 0.0 到 0.9 表示加载进度。
            // 当它达到 0.9 时，表示加载已完成，场景准备好被激活。
            // 我们将它转换为 0.0 到 1.0 的值，以便于在UI上显示。
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (loadingSlider != null)
            {
                loadingSlider.value = progress;
            }

            Debug.Log("加载进度: " + (progress * 100f) + "%");

            // 等待下一帧，让出CPU时间，避免游戏卡死
            yield return null;
        }

        // 加载完成后，新场景会自动显示。这个协程所在的旧场景对象会被销毁。
        // 所以我们不需要在这里手动隐藏加载界面。
    }


    // 返回按钮，从模式选择返回到关卡选择
    public void GoBackToLevelSelect()
    {
        modeSelectPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }
}