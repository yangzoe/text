using UnityEngine;

[RequireComponent(typeof(Transform))]
public class EyeMovement : MonoBehaviour
{
    [Header("跟随设置")]
    [SerializeField] private Transform parentTransform; // 父物体Transform组件
    [SerializeField] private float maxOffset = 0.2f;     // 最大偏移量
    [SerializeField] private float smoothTime = 0.1f;    // 平滑过渡时间
    [Header("组件引用")]
    [SerializeField] private PlayerMove playerController; // 父物体控制器
    [SerializeField] private Player player; // 父物体控制器
    private Vector3 originalLocalPosition; // 初始本地位置
    private Vector3 currentVelocity;       // 当前插值速度
    private void Awake()
    {
        // 初始化相对位置
        originalLocalPosition = transform.localPosition;
        // 自动获取父物体组件
        if (parentTransform == null)
            parentTransform = transform.parent;

        if (playerController == null)
            playerController = parentTransform.GetComponent<PlayerMove>();
    }
    private void FixedUpdate()
    {
        if (playerController == null || !player.canMove) return;
        // 根据移动方向计算目标偏移
        Vector2 moveDirection = playerController.moveDirection;
        Vector3 targetOffset = new Vector3(
            moveDirection.x * maxOffset, // X轴反向偏移
            moveDirection.y * maxOffset, // Y轴反向偏移
            0
        );
        // 使用平滑阻尼过渡位置
        Vector3 newPosition = Vector3.SmoothDamp(
            transform.localPosition,
            originalLocalPosition + targetOffset,
            ref currentVelocity,
            smoothTime
        );

        // 应用新位置（保持Z轴不变）
        newPosition.z = originalLocalPosition.z;
        transform.localPosition = newPosition;
    }
}