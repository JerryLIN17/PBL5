// CarvingTool.cs
// 目的：处理切削逻辑。当玩家用手柄指向一个可切削方块并按下扳机时，消除该方块。
// 挂载位置：拥有 XR Ray Interactor 的右手控制器对象上。

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class CarvingTool : MonoBehaviour
{
    // **新增数据结构**
    // 用于存储一次操作的完整信息：哪个工件的哪个方块被移除了。
    private struct UndoRecord
    {
        public Workpiece parentWorkpiece;
        public GameObject removedCube;
    }
    
    [Header("输入设置")]
    [SerializeField] private InputActionReference carveActionReference; // 从XRI Default Input Actions中拖入Select Action
    [SerializeField] private InputActionReference undoActionReference; // 新增：拖入Undo Action

    private XRRayInteractor rayInteractor;
    private Workpiece lastCarvedWorkpiece; 
    
    // **修改点 1: 使用新的数据结构来创建全局历史记录栈**
    private Stack<UndoRecord> globalUndoHistory = new Stack<UndoRecord>();

    private void Awake()
    {
        rayInteractor = GetComponent<XRRayInteractor>();
        if (rayInteractor == null)
        {
            Debug.LogError("CarvingTool 脚本需要 XRRayInteractor 组件！", this);
        }
    }

    private void OnEnable()
    {
        if (carveActionReference != null)
        {
            carveActionReference.action.performed += OnCarveAction;
        }
        if (undoActionReference != null) undoActionReference.action.performed += OnUndoAction; // 注册Undo事件
    }

    private void OnDisable()
    {
        if (carveActionReference != null)
        {
            carveActionReference.action.performed -= OnCarveAction;
        }
        if (undoActionReference != null) undoActionReference.action.performed -= OnUndoAction; // 注销Undo事件
    }

    // private void OnCarveAction(InputAction.CallbackContext context)
    // {
    //     // 检查射线是否击中了物体
    //     if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
    //     {
    //         // 检查被击中的物体是否是可切削的（通过Layer或Tag）
    //         if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Carvable"))
    //         {
    //             // 消除（禁用）该方块
    //             hit.collider.gameObject.SetActive(false);
    //             // 可选：在这里添加切削的音效和粒子效果
    //         }
    //     }
    // }
    private void OnCarveAction(InputAction.CallbackContext context)
    {
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Carvable"))
            {
                Workpiece workpiece = hit.collider.GetComponentInParent<Workpiece>();
                if (workpiece != null)
                {
                    GameObject cube = hit.collider.gameObject;

                    // **修改点 2: 在执行操作前，先记录到全局历史中**
                    // 创建一条新的撤销记录
                    UndoRecord record = new UndoRecord
                    {
                        parentWorkpiece = workpiece,
                        removedCube = cube
                    };
                    // 将记录压入栈
                    globalUndoHistory.Push(record);

                    // 然后再让工件执行移除操作
                    workpiece.RemoveCube(cube);
                }
            }
        }
    }
    
    // --- 新增Undo事件处理方法 ---
    private void OnUndoAction(InputAction.CallbackContext context)
    {
        // **修改点 3: 从全局历史记录中恢复**
        if (globalUndoHistory.Count > 0)
        {
            // 从栈顶弹出一个操作记录
            UndoRecord lastRecord = globalUndoHistory.Pop();
            
            // 使用记录中的信息，让对应的工件恢复对应的方块
            if (lastRecord.parentWorkpiece != null && lastRecord.removedCube != null)
            {
                lastRecord.parentWorkpiece.RestoreCube(lastRecord.removedCube);
            }
        }
        else
        {
            Debug.Log("没有可以撤销的操作了。");
        }
    }
}