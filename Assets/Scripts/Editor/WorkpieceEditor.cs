// WorkpieceEditor.cs
// 必须放在名为 "Editor" 的文件夹下

using UnityEngine;
using UnityEditor; // 引入UnityEditor命名空间
using System.Collections.Generic;

// [CustomEditor(typeof(Workpiece))] 告诉Unity这个脚本是为Workpiece组件定制的Inspector界面
[CustomEditor(typeof(Workpiece))]
public class WorkpieceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 绘制默认的Inspector界面 (例如，显示correctShapeTemplate列表)
        base.OnInspectorGUI();

        // target是Unity编辑器脚本中对当前正在编辑的组件的引用
        // 我们需要把它转换成我们自己的Workpiece类型
        Workpiece workpiece = (Workpiece)target;

        // 添加一个间隔，让界面更美观
        EditorGUILayout.Space();

        // 创建一个醒目的按钮
        GUI.backgroundColor = Color.cyan; // 让按钮颜色更突出
        if (GUILayout.Button("Generate Template From Active Children", GUILayout.Height(30)))
        {
            // 当按钮被点击时，调用我们自己编写的函数
            GenerateTemplate(workpiece);
        }
        GUI.backgroundColor = Color.white; // 恢复默认颜色
        
        EditorGUILayout.HelpBox("操作指南：\n1. 在Hierarchy中，只保留该零件正确的子方块为激活(active)状态，禁用多余的。\n2. 点击上面的按钮，即可自动生成坐标模板。", MessageType.Info);
    }

    private void GenerateTemplate(Workpiece workpiece)
    {
        // 在进行修改前，记录撤销操作，这样如果弄错了可以用 Ctrl+Z 恢复
        Undo.RecordObject(workpiece, "Generate Shape Template");

        // 如果workpiece的模板列表不是空的，先清空它
        if (workpiece.correctShapeTemplate == null)
        {
            workpiece.correctShapeTemplate = new List<Vector3>();
        }
        else
        {
            workpiece.correctShapeTemplate.Clear();
        }

        // 遍历workpiece游戏对象的所有子Transform
        foreach (Transform child in workpiece.transform)
        {
            // 只处理那些当前是激活状态的子对象
            if (child.gameObject.activeSelf)
            {
                // 将子对象的局部坐标添加到列表中
                workpiece.correctShapeTemplate.Add(child.localPosition);
            }
        }

        // 标记workpiece对象为“已修改”，这样Unity才会保存更改
        EditorUtility.SetDirty(workpiece);

        Debug.Log($"为 {workpiece.name} 成功生成了 {workpiece.correctShapeTemplate.Count} 个坐标点的模板！", workpiece.gameObject);
    }
}