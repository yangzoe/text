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

    // 新增控制变量
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



        canMove = false; // 进化前不能移动
        Health = 4;
        enabled = true;
        // 初始状态设置

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

        // 进化计时
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

        // 进化后的移动逻辑
        if (hasEvolved)
        {
            HandleMovement();
            HandleShooting();
        }
    }

    // 进化后启用移动
    private void HandleMovement()
    {
        if (player.currentState == Player.State.Eating ||
            player.currentState == Player.State.Full)
        {
            Move(2); // 慢速模式
        }
        else
        {
            Move(1); // 正常模式
        }
    }

    // 进化后启用射击
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
        canMove = true; // 进化后启用移动
        dir = Random.insideUnitCircle.normalized; // 重置移动方向
        StartCoroutine(EnhancedAttackPattern());
        hasEvolved = true;
    }

    // 固定间隔攻击模式
    private IEnumerator AttackPattern()
    {
        while (true)
        {
            Shoot();
            yield return new WaitForSeconds(1.5f); // 固定射击间隔
        }
    }

    // 强化攻击模式（进化后专属）
    private IEnumerator EnhancedAttackPattern()
    {
        isInAttackState = true;

        while (player.currentState != Player.State.Over)
        {
            // 第一阶段：旋转射击
            for (int i = 0; i < 8; i++)
            {
                transform.Rotate(Vector3.forward * 45f);
                Shoot();
                yield return new WaitForSeconds(0.4f);
            }

            // 第二阶段：快速两连射
            for (int j = 0; j < 2; j++)
            {
                Shoot();
                yield return new WaitForSeconds(0.2f);
            }

            // 第三阶段：移动射击
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
        canMove = false; // 确保初始不能移动
        hasEvolved = false;
    }

    void OnDisable()
    {
        Health = 4;
        enabled = true;
        hasEvolved = false;
    }
}