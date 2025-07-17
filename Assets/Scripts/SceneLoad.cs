using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour
{
    void Start()
    {
        // 这个脚本的唯一任务，就是在所有全局管理器（比如XR Setup）初始化后
        // 立刻加载你的主菜单或起始场景。
        // 把 "HomeScene" 替换成你实际的起始场景名字。
        SceneManager.LoadScene("HomeScene"); 
    }
}