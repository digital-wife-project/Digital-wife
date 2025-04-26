using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 10.0f; // 水平移动速度
    public float sensitivity = 100.0f; // 鼠标敏感度
    public float verticalSpeed = 5.0f; // 垂直移动速度
    public float minYAngle = -90f; // 最小俯角
    public float maxYAngle = 90f; // 最大仰角
    public GameObject Main_Panel;
    private Vector3 rotation = Vector3.zero;
    private bool isCursorLocked = true;
    private bool isCameraActive = true; // 相机控制是否激活
    public GameObject targetObject; // 目标对象

    void Start()
    {
        // 设置相机到原点
        transform.position = Vector3.zero;
        LockCursor(true);
    }

    public void MoveCameraToFrontOfObject()
    {
        if (targetObject == null)
            return;
        // 获取摄像机的Transform组件
        Transform cameraTransform = Camera.main.transform;

        // 计算目标位置
        Vector3 targetPosition = targetObject.transform.position + targetObject.transform.forward * 4.0f;

        // 移动摄像机到目标位置
        cameraTransform.position = targetPosition;

        // 设置摄像机的朝向，使其面向目标GameObject
        cameraTransform.LookAt(targetObject.transform);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && Main_Panel.activeSelf)
        {
            isCursorLocked = !isCursorLocked;
            isCameraActive = !isCameraActive;
            LockCursor(isCursorLocked);
        }

        // 检查相机是否激活
        if (!isCameraActive)
            return;

        // 水平移动
        float translationX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float translationZ = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        transform.Translate(translationX, 0, translationZ);

        // 垂直移动
        if (Input.GetKey(KeyCode.Space))
        {
            transform.Translate(0, verticalSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.Translate(0, -verticalSpeed * Time.deltaTime, 0);
        }

        // 鼠标旋转
        float rotationX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        rotation.y += rotationX;
        rotation.x -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        rotation.x = Mathf.Clamp(rotation.x, minYAngle, maxYAngle);

        transform.eulerAngles = rotation;

        // 锁定和解锁鼠标以及激活/禁用相机控制
    }

    void LockCursor(bool lockCursor)
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
