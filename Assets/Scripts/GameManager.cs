// GameManager.cs
// 目的：在场景之间保存玩家选择的关卡和模式。
// 使用：这是一个静态类，无需附加到任何GameObject上。

public static class GameManager
{
    public static int SelectedLevel { get; set; }
    public static string SelectedMode { get; set; } // "Teaching" 或 "Testing"
}