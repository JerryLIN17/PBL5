// Workpiece.cs
// 目的：管理一个工件的所有可切削方块，并提供形状检测功能。
// 挂载位置：测试模式工件预制体的根对象上。

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Workpiece : MonoBehaviour
{
    [Header("模板数据")]
    [Tooltip("定义此榫卯结构正确的形状。坐标是相对于此工件根对象的局部坐标。")]
    [SerializeField] public List<Vector3> correctShapeTemplate;

    private List<GameObject> allCubes;
    
   
    
    void Awake()
    {
        // 自动获取所有子对象作为可切削的方块
        allCubes = new List<GameObject>();
        
        // 遍历所有子Transform，无论它们当前是否激活
        foreach (Transform child in transform)
        {
            // 将所有子方块设置为激活状态，确保玩家开始时看到的是完整的工件
            child.gameObject.SetActive(true); 
            allCubes.Add(child.gameObject);
        }

        Debug.Log($"Workpiece '{name}' initialized with {allCubes.Count} cubes, all activated.");
    }
    
    // --- 新增公共方法，用于从外部移除方块并记录 ---
    public void RemoveCube(GameObject cubeToRemove)
    {
        if (cubeToRemove != null && cubeToRemove.activeSelf)
        {
            cubeToRemove.SetActive(false);
        
        }
    }
    
    // **新增方法: RestoreCube**
    // 提供一个公共方法来恢复指定的方块。
    public void RestoreCube(GameObject cubeToRestore)
    {
        if (cubeToRestore != null)
        {
            cubeToRestore.SetActive(true);
        }
    }
    

    
    /// <summary>
    /// 自动雕刻工件，使其与模板形状完全一致。
    /// 这是一个用于调试和测试的功能。
    /// </summary>
    public void AutoComplete()
    {
        if (correctShapeTemplate == null || correctShapeTemplate.Count == 0)
        {
            Debug.LogWarning($"无法为 '{name}' 自动完成，因为其模板为空！", this);
            return;
        }

        // 为了快速查找，将模板坐标列表转换成HashSet。
        // 这样查找一个坐标是否存在的时间复杂度从 O(n) 降到了 O(1)。
        var templatePositionsSet = new HashSet<Vector3>(correctShapeTemplate);

        // 遍历所有的子立方体
        foreach (GameObject cube in allCubes)
        {
            bool shouldBeActive = false;
            // 检查当前立方体的局部坐标是否存在于模板中
            // 同样，使用距离检查以避免浮点数精度问题
            foreach (var templatePos in templatePositionsSet)
            {
                if (Vector3.Distance(cube.transform.localPosition, templatePos) < 0.01f)
                {
                    shouldBeActive = true;
                    break;
                }
            }
            
            // 根据检查结果设置立方体的激活状态
            cube.SetActive(shouldBeActive);
        }

        Debug.Log($"Workpiece '{name}' has been auto-completed to match its template.", this);
    }
    // 检测当前形状是否与模板匹配
    public bool CheckShape()
    {
        // 获取当前所有激活（未被切除）的方块的局部坐标
        List<Vector3> currentActiveCubesPositions = new List<Vector3>();
        foreach (var cube in allCubes)
        {
            if (cube.activeSelf)
            {
                // 四舍五入到最近的整数，以避免浮点精度问题
                currentActiveCubesPositions.Add(cube.transform.localPosition);
            }
        }

        // 1. 检查数量是否一致
        if (currentActiveCubesPositions.Count != correctShapeTemplate.Count)
        {
            Debug.Log($"形状检查失败：数量不匹配。当前: {currentActiveCubesPositions.Count}, 期望: {correctShapeTemplate.Count}");
            return false;
        }

        // 2. 检查每个方块的位置是否都在模板中 (使用HashSet以提高效率)
        var templatePositionsSet = new HashSet<Vector3>(correctShapeTemplate);

        foreach (var pos in currentActiveCubesPositions)
        {
            bool found = false;
            // 由于浮点数精度问题，直接比较可能失败，我们检查一个很小的公差范围
            foreach (var templatePos in templatePositionsSet)
            {
                if (Vector3.Distance(pos, templatePos) < 0.01f)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Debug.Log($"形状检查失败：位置 {pos} 不在模板中。");
                return false;
            }
        }

        return true;
    }
}