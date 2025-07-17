using UnityEngine;

public class ObjectDragger : MonoBehaviour
{
    private GameObject targetObject; // 我们当前拖动的目标物体
    private Vector3 mouseOffset;     // 鼠标指针与物体中心的偏移量
    private Rigidbody targetRigidbody; // 目标物体的刚体

    void Update()
    {
        // --- 鼠标按下时 ---
        if (Input.GetMouseButtonDown(0)) // 0代表鼠标左键
        {
            // 从摄像机向鼠标位置发射一条射线
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // 如果射线击中了某个物体
            if (Physics.Raycast(ray, out hitInfo))
            {
                // 并且这个物体的标签是我们设置的 "Draggable"
                if (hitInfo.collider.gameObject.CompareTag("Draggable"))
                {
                    // 记录下这个物体作为我们的目标
                    targetObject = hitInfo.collider.gameObject;
                    targetRigidbody = targetObject.GetComponent<Rigidbody>();
                    
                    // 让物体在被拖动时变为 "Kinematic"，这样它就不会乱晃或被其他物理影响
                    if (targetRigidbody != null)
                    {
                        targetRigidbody.isKinematic = true;
                    }

                    // 计算鼠标和物体中心的偏移，这样拖动时物体不会突然跳到鼠标中心
                    mouseOffset = targetObject.transform.position - hitInfo.point;
                }
            }
        }

        // --- 鼠标按住并拖动时 ---
        if (targetObject != null && Input.GetMouseButton(0))
        {
            // 将鼠标在屏幕上的位置转换成世界坐标
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            // 我们需要知道射线应该延伸多远
            float distanceToScreen = Vector3.Distance(ray.origin, targetObject.transform.position);
            Vector3 mouseWorldPosition = ray.GetPoint(distanceToScreen);

            // 更新物体的位置
            targetObject.transform.position = mouseWorldPosition + mouseOffset;
        }

        // --- 鼠标松开时 ---
        if (Input.GetMouseButtonUp(0) && targetObject != null)
        {
            // 恢复物体的物理属性
            if (targetRigidbody != null)
            {
                targetRigidbody.isKinematic = false;
            }
            
            // 清空目标，表示我们已经松手了
            targetObject = null;
        }
    }
}