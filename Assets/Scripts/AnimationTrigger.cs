// AnimationTrigger.cs
// 目的：提供一个公共方法来播放动画。
// 挂载位置：教学模型上，或一个世界空间的按钮上。

using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    [Header("动画设置")]
    [Tooltip("拖入带有Animator组件的教学模型对象")]
    [SerializeField] private Animator targetAnimator;

    [Tooltip("要播放的动画状态名")]
    [SerializeField] private string animationName = "AssembleAnimation";
    
    // 用于存储初始状态的数据结构
    private struct TransformState
    {
        public Vector3 localPosition;
        public Quaternion localRotation;
    }

    // 字典：键是子物体的Transform，值是它对应的初始状态
    private Dictionary<Transform, TransformState> initialStates;


   
    void Awake()
    {
        targetAnimator = GetComponent<Animator>();
        RecordInitialStates();
    }
    /// <summary>
    /// 记录此对象及其所有子对象的初始局部位置和旋转。
    /// </summary>
    private void RecordInitialStates()
    {
        initialStates = new Dictionary<Transform, TransformState>();

        // 使用GetComponentsInChildren来获取包括自身在内的所有Transform
        Transform[] allTransforms = GetComponentsInChildren<Transform>(true); // true表示也获取非激活的子对象

        foreach (Transform t in allTransforms)
        {
            // 存储每个Transform的初始局部位置和旋转
            initialStates[t] = new TransformState
            {
                localPosition = t.localPosition,
                localRotation = t.localRotation
            };
        }

        Debug.Log($"为 {gameObject.name} 记录了 {initialStates.Count} 个对象的初始状态。");
    }
    
    /// <summary>
    /// 将此对象及其所有子对象重置到记录的初始状态。
    /// </summary>
    private void ResetToInitialStates()
    {
        if (initialStates == null)
        {
            Debug.LogError("初始状态未被记录！无法重置。");
            return;
        }

        // 遍历字典中记录的每一个Transform
        foreach (var entry in initialStates)
        {
            Transform t = entry.Key;
            TransformState state = entry.Value;

            // 恢复其局部位置和旋转
            t.localPosition = state.localPosition;
            t.localRotation = state.localRotation;
        }
        
        Debug.Log($"为 {gameObject.name} 重置了所有部件的状态。");
    }
    
    /// <summary>
    /// 公共方法：重置模型状态并播放动画。
    /// 此方法应由UI按钮的OnClick事件或其它交互事件调用。
    /// </summary>
    public void PlayAnimation()
    {
        if (targetAnimator != null)
        {
            // 步骤 1: 禁用 Animator，防止它干扰我们手动设置Transform
            targetAnimator.enabled = false;
            
            // 步骤 2: 将所有部件重置到它们的初始局部状态
            ResetToInitialStates();

            // 步骤 3: 重新启用 Animator 并立即播放动画
            // 使用协程或Invoke来延迟启用，确保Transform的设置在本帧生效
            // 这是最稳妥的做法，避免在同一帧内设置Transform和启用Animator可能发生的冲突
            Invoke(nameof(EnableAndPlayAnimation), 0f);
        }
        else
        {
            Debug.LogWarning("未找到Animator组件！", this);
        }
    }
    
    private void EnableAndPlayAnimation()
    {
        targetAnimator.enabled = true;
        // -1 代表在任何状态下都能切换到这个动画；0f 代表从动画的开头播放
        targetAnimator.Play(animationName, -1, 0f);
    }
    // 此方法由UI按钮的OnClick事件或其它交互事件调用
    // public void PlayAnimation()
    // {
    //     if (targetAnimator != null)
    //     {
    //         // 步骤 1: 重置Transform
    //         // targetAnimator.transform 指的是挂载Animator组件的那个物体的Transform
    //         targetAnimator.transform.position = startPosition;
    //         targetAnimator.transform.rotation = Quaternion.Euler(startRotationEuler);
    //         
    //         // 步骤 2: 播放动画
    //         targetAnimator.Play(animationName, -1, 0f); // -1代表当前层, 0f代表从头播放
    //     }
    //     else
    //     {
    //         Debug.LogWarning("未指定目标Animator！", this);
    //     }
    // }
}