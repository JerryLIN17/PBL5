// GameSceneController.cs (FINAL FLEXIBLE VERSION)
// 目的：在游戏场景加载时，根据GameManager中的设置，初始化正确的模型和模式。
// 挂载位置：GameScene中的一个空对象，例如 "GameSceneController"。

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI; // 引入Linq以便使用更方便的集合操作

public class GameSceneController : MonoBehaviour
{
    // LevelData类保持不变
    [System.Serializable]
    public class LevelData
    {
        public string levelName;
        public GameObject teachingModelPrefab;
        public GameObject testingWorkpiecePrefab; // "组合工件"预制体 (可包含任意数量的零件)
    }

    [Header("关卡数据列表")]
    [SerializeField] private List<LevelData> levels;

    [Header("UI 和反馈")]
    [SerializeField] private GameObject teachingUI;
    [SerializeField] private GameObject testingUI;
    [SerializeField] private GameObject successFeedback;
    [SerializeField] private GameObject errorFeedback;
    [SerializeField] private Text errorText;//TMPro.TextMeshProUGUI errorText; 
    // --- 新增内容 ---
    [Header("关卡预览图设置")]
    [Tooltip("拖入场景中用于显示预览图的Image组件")]
    [SerializeField] private Image levelPreviewImage; // 用于显示预览图的UI Image

    [Tooltip("按关卡顺序存放的预览图列表 (关卡1对应索引0, ...)")]
    [SerializeField] private List<Sprite> levelPreviewSprites; // 所有关卡的预览图
    // --- 新增内容结束 ---
    [Header("教学模式UI组件")]
    [SerializeField] private Button assembleButton;
    [SerializeField] private Button disassembleButton;
    
    // --- 计时器新增变量 ---
    [Header("计时器设置")]
    [SerializeField] private Text timerText; // 拖入TimerText对象
    private bool isTimerRunning = false;
    private float elapsedTime = 0f;
    // --- 计时器新增变量结束 ---

    private GameObject currentPairedWorkpiece;
    private GameObject currentTeachingModel;

    void Start()
    {
        // Start方法保持不变
        if (successFeedback) successFeedback.SetActive(false);
        if (errorFeedback) errorFeedback.SetActive(false);

        int levelIndex = GameManager.SelectedLevel - 1;
        string mode = GameManager.SelectedMode;
        // --- 新增逻辑：设置预览图 ---
        UpdateLevelPreview(levelIndex);
        // --- 新增逻辑结束 ---
        if (levelIndex < 0 || levelIndex >= levels.Count)
        {
            Debug.LogError("无效的关卡索引！返回首页。");
            GoToHome();
            return;
        }

        LevelData selectedLevel = levels[levelIndex];

        if (mode == "Teaching")
        {
            currentTeachingModel = Instantiate(selectedLevel.teachingModelPrefab);
            if (teachingUI) teachingUI.SetActive(true);
            if (testingUI) testingUI.SetActive(false);
        }
        else if (mode == "Testing")
        {
            currentPairedWorkpiece = Instantiate(selectedLevel.testingWorkpiecePrefab);
            if (teachingUI) teachingUI.SetActive(false);
            if (testingUI) testingUI.SetActive(true);
        }
        else
        {
            Debug.LogError("无效的模式选择！返回首页。");
            GoToHome();
        }
        
        // 初始化计时器显示
        if (timerText != null)
        {
            timerText.text = "00:00:00";
        }
    }
    
    void Update()
    {
        // #if UNITY_EDITOR 是一条预处理指令。
        // 这意味着它包裹的代码只会在Unity编辑器中被编译和执行。
        // 当你构建游戏（例如为Quest 3构建apk）时，这部分代码会被完全忽略。
        // 这可以确保调试功能不会意外地进入到最终发布版本中。
#if UNITY_EDITOR
        
        // 监听键盘的 'C' 键 (C for Complete)
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("调试按键 'C' 被按下，执行自动完成...");
            AutoCompleteAllWorkpieces();
        }
        
#endif
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }
    
    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;

        // 格式化时间为 MM:SS:MS
        float minutes = Mathf.FloorToInt(elapsedTime / 60);
        float seconds = Mathf.FloorToInt(elapsedTime % 60);
        float milliseconds = (elapsedTime % 1) * 100;

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }
    
    // --- 计时器控制公共方法 ---
    // 此方法由“开始计时”按钮的OnClick事件调用
    public void StartTimer()
    {
        if (!isTimerRunning)
        {
            // 如果希望每次开始都重置时间，取消下面这行的注释
            // elapsedTime = 0f; 
            isTimerRunning = true;
        }
    }

    // 此方法由“结束计时”按钮的OnClick事件调用
    public void StopTimer()
    {
        isTimerRunning = false;
    }
    // --- 计时器控制公共方法结束 ---
    
    // --- 新增方法 ---
    /// <summary>
    /// 根据关卡索引更新预览图
    /// </summary>
    private void UpdateLevelPreview(int levelIndex)
    {
        if (levelPreviewImage == null || levelPreviewSprites == null)
        {
            Debug.LogWarning("未设置预览图组件或预览图列表，无法显示预览图。");
            return;
        }

        // 检查索引是否在列表范围内
        if (levelIndex >= 0 && levelIndex < levelPreviewSprites.Count && levelPreviewSprites[levelIndex] != null)
        {
            levelPreviewImage.sprite = levelPreviewSprites[levelIndex];
            levelPreviewImage.gameObject.SetActive(true); // 确保Image是可见的
        }
        else
        {
            Debug.LogWarning($"找不到关卡索引 {levelIndex} 对应的预览图，将隐藏Image组件。");
            levelPreviewImage.gameObject.SetActive(false); // 如果没找到图，就隐藏起来
        }
    }
    // --- 新增方法结束 ---
    
     // 我们将原来的按钮逻辑提取到一个独立的函数中，以便键盘和按钮都能调用它
    private void AutoCompleteAllWorkpieces()
    {
        if (currentPairedWorkpiece == null)
        {
            Debug.LogWarning("场景中没有工件可以自动完成。");
            return;
        }

        Workpiece[] pieces = currentPairedWorkpiece.GetComponentsInChildren<Workpiece>();

        if (pieces.Length == 0)
        {
            Debug.LogWarning("在当前工件中找不到任何可自动完成的零件。");
            return;
        }

        Debug.Log("开始自动完成所有工件...");
        foreach (Workpiece piece in pieces)
        {
            piece.AutoComplete();
        }
    }

    // 这个方法可以保留，如果你还想有一个UI按钮的话。
    // 如果你确定只用键盘，可以删除这个方法和对应的UI按钮。
    public void OnAutoCompleteButtonPressed()
    {
        AutoCompleteAllWorkpieces();
    }

    // 此方法由“完成检查”按钮调用
    public void OnCheckWorkButtonPressed()
    {
        if (currentPairedWorkpiece == null) return;

        // --- NEW FLEXIBLE LOGIC START ---

        // 在组合对象下找到所有带有Workpiece脚本的零件
        Workpiece[] pieces = currentPairedWorkpiece.GetComponentsInChildren<Workpiece>();

        if (pieces.Length == 0)
        {
            Debug.LogError("预制体配置错误！在组合工件下没有找到任何带Workpiece脚本的零件。");
            if (errorText) errorText.text = "Prefabricated body configuration error";
            if (errorFeedback) errorFeedback.SetActive(true);
            Invoke(nameof(HideErrorFeedback), 3.0f);
            return;
        }

        // 创建一个列表来存储所有未通过检查的零件
        List<string> incorrectPieces = new List<string>();

        // 循环遍历找到的每一个零件并检查其形状
        foreach (Workpiece piece in pieces)
        {
            if (!piece.CheckShape())
            {
                // 如果形状不正确，就将它的名字记录下来
                incorrectPieces.Add(piece.name);
            }
        }

        // 检查是否有任何零件未通过检查
        if (incorrectPieces.Count == 0)
        {
            // 如果列表是空的，意味着所有零件都正确
            Debug.Log($"制作正确！所有 {pieces.Length} 个零件都匹配！");
            //if (successFeedback) successFeedback.SetActive(true);
            if (errorFeedback) errorFeedback.SetActive(true);
            if (errorText) errorText.text = "Assembly completed successfully";
            Invoke(nameof(HideErrorFeedback), 4.0f);
            //currentPairedWorkpiece.gameObject.SetActive(false);
            //GameObject finalModel = Instantiate(levels[GameManager.SelectedLevel - 1].teachingModelPrefab, currentPairedWorkpiece.transform.position, currentPairedWorkpiece.transform.rotation);
            var anim = currentPairedWorkpiece.GetComponent<AnimationTrigger>();
            if (anim)
            {
                anim.PlayAnimation();
            }
        }
        else
        {
            // 如果列表不为空，说明有错误
            // 使用 string.Join 将所有错误零件的名字组合成一个字符串
            string errorDetails = "Assembly failed, model structure error";
            Debug.Log(errorDetails);
            
            if (errorText) errorText.text = errorDetails;
            if (errorFeedback) errorFeedback.SetActive(true);
            //if (successFeedback) successFeedback.SetActive(false);

            Invoke(nameof(HideErrorFeedback), 4.0f); // 错误信息多，显示时间长一点
        }
        
        // --- NEW FLEXIBLE LOGIC END ---
    }
    /// <summary>
    /// 播放教学模型的动画。
    /// </summary>
    public void AnimationExample()
    {
        var anim = currentTeachingModel.GetComponent<AnimationTrigger>();
        if (anim)
        {
            anim.PlayAnimation();
        }
        
    }

    private void HideErrorFeedback()
    {
        if (errorFeedback) errorFeedback.SetActive(false);
    }
    
    public void GoToHome()
    {
        SceneManager.LoadScene("HomeScene");
    }
}