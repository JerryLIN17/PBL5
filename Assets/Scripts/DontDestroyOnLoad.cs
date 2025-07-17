using UnityEngine;

// 你可以把这个脚本命名为 PersistentSingleton.cs
public class DontDestroyOnLoad : MonoBehaviour // 或者 public class PersistentSingleton : MonoBehaviour
{
    // 1. 创建一个静态变量来“持有”唯一的实例
    // static 意味着这个变量属于类本身，而不是类的某个具体实例
    private static DontDestroyOnLoad instance;

    void Awake()
    {
        // 2. 检查这个静态变量是否已经有值
        if (instance == null)
        {
            // 如果 instance 是空的，意味着“我是第一个”
            // a. 把我自己赋值给这个静态变量
            instance = this;
            
            // b. 告诉Unity不要在加载新场景时销毁我
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 如果 instance 不是空的，意味着场景中已经有一个唯一的实例了
            // “我”就是多余的那个，所以“我”必须被销毁
            Destroy(gameObject);
        }
    }
}