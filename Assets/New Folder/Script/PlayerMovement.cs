using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public ParticleSystem particleTailing;
    public float moveSpeed = 5f; // �ƶ��ٶ�
    public float rotationSmoothness = 10f; // ��תƽ������ϵ��
    void Update()
    {
        // 1. ��ȡ���뷽��
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 moveDirection = new Vector2(horizontal, vertical).normalized;
        // 2. �ƶ�����
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        // 3. ����������룬�������峯��
        if (moveDirection != Vector2.zero)
        {
            //PPS();          //�����źŴ�����β
            // ����Ŀ����ת�Ƕȣ�ʹ Y �ᳯ���ƶ�����
            float targetAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            // ƽ��������ת
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                1 - Mathf.Exp(-rotationSmoothness * Time.deltaTime)
            );
        }
    }
    void PPS()//����ϵͳ
    {
        particleTailing.Play();
    }
}