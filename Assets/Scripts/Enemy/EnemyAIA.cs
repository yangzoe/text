using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyAIA : EnemyBase
{
    private bool isFirstEvolved = true;
    private Transform bulletBornPosition;
    public Sprite adultSprite;
    private float timer = 0f;
    private Animator animator;
    public Image image;

    // �������Ʊ���
    private Coroutine attackCoroutine;
    private bool isInAttackState;
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.Find("Player").GetComponent<Player>();
        bulletBornPosition = transform.GetChild(0);
        Initialize();
    }
    void Enable()
    {



        canMove = false; // ����ǰ�����ƶ�
        Health = 4;
        enabled = true;
        // ��ʼ״̬����

    }

    void Update()
    {
        image.fillAmount = Mathf.Lerp(image.fillAmount, Health / 4f, 3f);

        if (Health <= 0 && !isDead)
        {
            isDead = true;
            StartCoroutine(Particles());
            enabled = false;
            return;
        }

        // ������ʱ
        timer += Time.deltaTime;
        if (timer >= evolutionTime && !hasEvolved)
        {
            if (adultSprite != null)
            {
                GetComponent<SpriteRenderer>().sprite = adultSprite;
            }
            if (isFirstEvolved)
            {
                animator.SetBool("EvolveA", true);
                isFirstEvolved = false;
            }
            Evolve();
        }

        // ��������ƶ��߼�
        if (hasEvolved)
        {
            HandleMovement();
            HandleShooting();
        }
    }

    // �����������ƶ�
    private void HandleMovement()
    {
        if (player.currentState == Player.State.Eating ||
            player.currentState == Player.State.Full)
        {
            Move(2); // ����ģʽ
        }
        else
        {
            Move(1); // ����ģʽ
        }
    }

    // �������������
    private void HandleShooting()
    {
        if (player.currentState == Player.State.Normal && !isInAttackState)
        {
            if (attackCoroutine == null)
            {
                attackCoroutine = StartCoroutine(AttackPattern());
            }
        }
        else
        {
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
        }
    }

    private void Evolve()
    {
        canMove = true; // �����������ƶ�
        dir = Random.insideUnitCircle.normalized; // �����ƶ�����
        StartCoroutine(EnhancedAttackPattern());
        hasEvolved = true;
    }

    // �̶��������ģʽ
    private IEnumerator AttackPattern()
    {
        while (true)
        {
            Shoot();
            yield return new WaitForSeconds(1.5f); // �̶�������
        }
    }

    // ǿ������ģʽ��������ר����
    private IEnumerator EnhancedAttackPattern()
    {
        isInAttackState = true;

        while (player.currentState != Player.State.Over)
        {
            // ��һ�׶Σ���ת���
            for (int i = 0; i < 8; i++)
            {
                transform.Rotate(Vector3.forward * 45f);
                Shoot();
                yield return new WaitForSeconds(0.4f);
            }

            // �ڶ��׶Σ�����������
            for (int j = 0; j < 2; j++)
            {
                Shoot();
                yield return new WaitForSeconds(0.2f);
            }

            // �����׶Σ��ƶ����
            float moveDuration = 2f;
            float elapsed = 0f;
            while (elapsed < moveDuration)
            {
                Shoot();
                elapsed += 1f;
                yield return new WaitForSeconds(1f);
            }
        }
        isInAttackState = false;
    }

    public override void Shoot()
    {
        if (bulletPrefab != null && bulletBornPosition != null)
        {
            GameObject bullet = ObjectPoolManager.Instance.GetObject("Bullet");
            bullet.transform.position = bulletBornPosition.position;
            bullet.transform.rotation = bulletBornPosition.rotation;
            bullet.GetComponent<Bullet>().dir = bulletBornPosition.up;
        }
    }

    public override void Initialize()
    {
        enemyType = "EnemyA";
        evolutionTime = 8f;
        minPosition = new Vector2(-5.7f, -4.1f);
        maxPosition = new Vector2(8f, 3.95f);
        moveSpeed = 3f;
        canMove = false; // ȷ����ʼ�����ƶ�
        hasEvolved = false;
    }

    void OnDisable()
    {
        Health = 4;
        enabled = true;
        hasEvolved = false;
    }
}