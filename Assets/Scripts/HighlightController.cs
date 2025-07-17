// HighlightController.cs
// 目的：通过代码监听XR Ray Interactor的悬停事件，并控制高亮效果。
// 挂载位置：拥有 XR Ray Interactor 的右手控制器对象上 (Right Controller)。

using HighlightingSystem;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// 强制要求此脚本必须和XRRayInteractor在同一个GameObject上
[RequireComponent(typeof(XRRayInteractor))]
public class HighlightController : MonoBehaviour
{
    private XRRayInteractor rayInteractor;
    
    // 用于存储上一帧高亮的对象上的 MaterialHighlighter 组件
    private MaterialHighlighter lastHighlighter = null;

    void Awake()
    {
        // 获取同一个对象上的 XRRayInteractor 组件
        rayInteractor = GetComponent<XRRayInteractor>();
        if (rayInteractor == null)
        {
            Debug.LogError("RayHighlightController 脚本需要和 XRRayInteractor 在同一个对象上！", this);
            enabled = false; // 禁用此脚本
            return;
        }
    }

    void Update()
    {
        MaterialHighlighter currentHighlighter = null;

        // 尝试从射线交互器获取当前指向的3D对象
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            // 如果击中了物体，尝试从该物体获取我们自定义的 MaterialHighlighter 组件
            hit.collider.TryGetComponent<MaterialHighlighter>(out currentHighlighter);
        }

        // 核心逻辑：比较当前对象和上一帧的对象
        // 如果当前指向的对象和上一帧不一样了...
        if (currentHighlighter != lastHighlighter)
        {
            // 步骤 1: 如果上一帧有高亮的对象，先取消它的高亮
            if (lastHighlighter != null)
            {
                lastHighlighter.Unhighlight();
            }

            // 步骤 2: 如果当前指向了一个新的可高亮对象，则高亮它
            if (currentHighlighter != null)
            {
                currentHighlighter.Highlight();
            }

            // 步骤 3: 更新记录，将当前对象作为下一帧的“上一个对象”
            lastHighlighter = currentHighlighter;
        }
    }
    
    // 当此脚本被禁用或销毁时，确保最后一个高亮对象被取消高亮
    private void OnDisable()
    {
        if (lastHighlighter != null)
        {
            lastHighlighter.Unhighlight();
            lastHighlighter = null;
        }
    }
}