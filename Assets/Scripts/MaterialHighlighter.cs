// MaterialHighlighter.cs (Optimized Version)
// 目的：控制单个对象的材质颜色以实现高亮。
// 挂载位置：每一个可被切削的小立方体预制体上。

using UnityEngine;

public class MaterialHighlighter : MonoBehaviour
{
    [Tooltip("高亮时显示的颜色")]
    [SerializeField] private Color highlightColor = Color.yellow;

    private MeshRenderer meshRenderer;
    private Color originalColor;
    
    // 我们将在Start中直接设置好一切，不再需要额外的布尔值检查
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogError("MaterialHighlighter 脚本需要同对象上有 MeshRenderer 组件!", this);
            enabled = false;
            return;
        }

        // 关键改动：在脚本一开始就访问 .material，为这个对象创建一个独立的材质实例。
        // 这样可以安全地存储和修改颜色，而不会影响其他对象。
        originalColor = meshRenderer.material.color;
    }

    // 公开方法：高亮此对象
    public void Highlight()
    {
        if (meshRenderer != null)
        {
            meshRenderer.material.color = highlightColor;
        }
    }

    // 公开方法：取消高亮，恢复原状
    public void Unhighlight()
    {
        if (meshRenderer != null)
        {
            meshRenderer.material.color = originalColor;
        }
    }
}