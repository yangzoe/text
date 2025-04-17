using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public Vector2 moveDirection;
    Rigidbody2D rb;
    Player player;
    public float rotationSmoothness = 10f; // ��תƽ������ϵ��
    public float Speed = 1f;

    public Vector2 boundaryMin = new Vector2(-5, -5);
    public Vector2 boundaryMax = new Vector2(5, 5);

    
    void Start()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical =  Input.GetAxis("Vertical");

        if(Mathf.Abs(horizontal)<0.2f)horizontal = 0;
        if (Mathf.Abs(vertical) < 0.2f) vertical = 0;
        moveDirection = new Vector2(horizontal, vertical).normalized;
    }

    void FixedUpdate()
    {
        if (player.currentState == Player.State.Normal|| player.currentState == Player.State.Eating || 
            player.currentState == Player.State.Rebirth)
        {
            //HandleMovement(1);
            Move(1);
        }
        if(player.currentState == Player.State.Full)
        {
            //HandleMovement(2);
            Move(2);
        }
        
        //HandleRotation();
    }

    /*void HandleMovement(int num)
    {
        if (num == 1)
        {
            Vector2 targetVelocity = moveDirection * Speed;
            rb.velocity = Vector2.Lerp(
                rb.velocity,
                targetVelocity,
                1 - Mathf.Exp(-rotationSmoothness * Time.fixedDeltaTime)
            );
        }
        else if (num == 2)
        {
            Vector2 targetVelocity = moveDirection * Speed * 3;
            rb.velocity = Vector2.Lerp(
                rb.velocity,
                targetVelocity,
                1 - Mathf.Exp(-rotationSmoothness * Time.fixedDeltaTime)
            );
        }

        // ����߽�
        Vector2 currentPos = rb.position;
        Vector2 clampedPos = new Vector2(
            Mathf.Clamp(currentPos.x, boundaryMin.x, boundaryMax.x),
            Mathf.Clamp(currentPos.y, boundaryMin.y, boundaryMax.y)
        );

        // ���λ�ñ����ƣ���ֹͣ��Ӧ������ٶ�
        if (clampedPos.x != currentPos.x)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        if (clampedPos.y != currentPos.y)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        // Ӧ�����ƺ��λ��
        rb.position = clampedPos;
    }

    void HandleRotation()
    {
        if (moveDirection != Vector2.zero)
        {
            // ����Ŀ��Ƕȣ���ԭʼ�߼�һ�£�
            float targetAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90f;

            // ʹ��������ת����
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            float smoothFactor = 1 - Mathf.Exp(-rotationSmoothness * Time.fixedDeltaTime);
            Quaternion newRotation = Quaternion.Slerp(
                rb.transform.rotation,
                targetRotation,
                smoothFactor
            );

            rb.MoveRotation(newRotation);
        }
    }
    */
    /*void Move(int num)      //ԭ�����ƶ�����
    {
        if (num == 1)
        {
            if (player.canMove)
            {
                Vector2 direction = Vector2.zero;
                Vector2 newPosition = new Vector2(0, 0);

                if (Input.GetKey(KeyCode.W)) direction += Vector2.up;
                if (Input.GetKey(KeyCode.A)) direction += Vector2.left;
                if (Input.GetKey(KeyCode.S)) direction += Vector2.down;
                if (Input.GetKey(KeyCode.D)) direction += Vector2.right;

                if (direction.magnitude > 0)
                {
                    direction.Normalize();
                }
                newPosition = rb.position + direction * Speed * Time.deltaTime;
                newPosition.x = Mathf.Clamp(newPosition.x, boundaryMin.x, boundaryMax.x);
                newPosition.y = Mathf.Clamp(newPosition.y, boundaryMin.y, boundaryMax.y);

                rb.position = newPosition;
            }
        }
        else if (num == 2)
        {
            if (player.canMove)
            {
                Vector2 direction = Vector2.zero;
                Vector2 newPosition = new Vector2(0, 0);

                if (Input.GetKey(KeyCode.W)) direction += Vector2.up;
                if (Input.GetKey(KeyCode.A)) direction += Vector2.left;
                if (Input.GetKey(KeyCode.S)) direction += Vector2.down;
                if (Input.GetKey(KeyCode.D)) direction += Vector2.right;

                if (direction.magnitude > 0)
                {
                    direction.Normalize();
                }
                newPosition = rb.position + direction * Speed * Time.deltaTime * 3;
                newPosition.x = Mathf.Clamp(newPosition.x, boundaryMin.x, boundaryMax.x);
                newPosition.y = Mathf.Clamp(newPosition.y, boundaryMin.y, boundaryMax.y);

                rb.position = newPosition;
            }
        }
    
    }
    */

    void Move(int num)
    {
        if (!player.canMove) return;

        // ����ģʽѡ���ٶȱ���
        float speedMultiplier = num == 2 ? 3f : 1f;

        // ������λ��
        Vector2 newPosition = rb.position +
                            moveDirection *
                            Speed *
                            speedMultiplier *
                            Time.deltaTime;

        // Ӧ�ñ߽�����
        newPosition.x = Mathf.Clamp(newPosition.x, boundaryMin.x, boundaryMax.x);
        newPosition.y = Mathf.Clamp(newPosition.y, boundaryMin.y, boundaryMax.y);

        // ���¸���λ��
        rb.position = newPosition;
    }

}
