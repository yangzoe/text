using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public ParticleSystem particleTailing;
    public float moveSpeed = 5f; // 移动速度
    public float rotationSmoothness = 10f; // 旋转平滑过渡系数
    void Update()
    {
        // 1. 获取输入方向
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 moveDirection = new Vector2(horizontal, vertical).normalized;
        // 2. 移动物体
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        // 3. 如果存在输入，调整物体朝向
        if (moveDirection != Vector2.zero)
        {
            //PPS();          //给个信号触发拖尾
            // 计算目标旋转角度（使 Y 轴朝向移动方向）
            float targetAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            // 平滑过渡旋转
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                1 - Mathf.Exp(-rotationSmoothness * Time.deltaTime)
            );
        }
    }
    void PPS()//粒子系统
    {
        particleTailing.Play();
    }
}