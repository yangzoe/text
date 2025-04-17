using UnityEngine;

[RequireComponent(typeof(Transform))]
public class EyeMovement : MonoBehaviour
{
    [Header("��������")]
    [SerializeField] private Transform parentTransform; // ������Transform���
    [SerializeField] private float maxOffset = 0.2f;     // ���ƫ����
    [SerializeField] private float smoothTime = 0.1f;    // ƽ������ʱ��
    [Header("�������")]
    [SerializeField] private PlayerMove playerController; // �����������
    [SerializeField] private Player player; // �����������
    private Vector3 originalLocalPosition; // ��ʼ����λ��
    private Vector3 currentVelocity;       // ��ǰ��ֵ�ٶ�
    private void Awake()
    {
        // ��ʼ�����λ��
        originalLocalPosition = transform.localPosition;
        // �Զ���ȡ���������
        if (parentTransform == null)
            parentTransform = transform.parent;

        if (playerController == null)
            playerController = parentTransform.GetComponent<PlayerMove>();
    }
    private void FixedUpdate()
    {
        if (playerController == null || !player.canMove) return;
        // �����ƶ��������Ŀ��ƫ��
        Vector2 moveDirection = playerController.moveDirection;
        Vector3 targetOffset = new Vector3(
            moveDirection.x * maxOffset, // X�ᷴ��ƫ��
            moveDirection.y * maxOffset, // Y�ᷴ��ƫ��
            0
        );
        // ʹ��ƽ���������λ��
        Vector3 newPosition = Vector3.SmoothDamp(
            transform.localPosition,
            originalLocalPosition + targetOffset,
            ref currentVelocity,
            smoothTime
        );

        // Ӧ����λ�ã�����Z�᲻�䣩
        newPosition.z = originalLocalPosition.z;
        transform.localPosition = newPosition;
    }
}