using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class EnemyAIC基类 : EnemyBase
{
    private Transform[] shootPoints;
    public Sprite adultSprite;
    private float evolutionTimer = 0f;
    public Image healthDisplay;

    // 攻击参数
    private Coroutine attackPattern;
    private const float PatternInterval = 1.5f;
    private const float RotateAngle = 90f;

    public Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.Find("Player").GetComponent<Player>();
        InitializeShootPoints();
        Initialize();
    }
    void Enable()
    {


        Health = 4;

        enabled = true;
        canMove = false;
        Health = 4;
        enabled = true;
    }

    void Update()
    {
        if (isDead) return;

        UpdateHealthDisplay();
        ProcessEvolution();

        if (hasEvolved)
        {
            BaseMovement();
        }
    }

    private void InitializeShootPoints()
    {
        shootPoints = new Transform[4];
        for (int i = 0; i < 4; i++)
        {
            shootPoints[i] = transform.GetChild(i);
        }
    }

    private void UpdateHealthDisplay()
    {
        healthDisplay.fillAmount = Mathf.Lerp(healthDisplay.fillAmount, Health / 4f, 3f);
        if (Health <= 0)
        {
            isDead = true;
            StartCoroutine(Particles());
            enabled = false;
        }
    }

    private void ProcessEvolution()
    {
        if (!hasEvolved)
        {
            evolutionTimer += Time.deltaTime;
            if (evolutionTimer >= evolutionTime)
            {
                EvolveToAdult();
            }
        }
    }

    private void EvolveToAdult()
    {
        GetComponent<SpriteRenderer>().sprite = adultSprite;
        animator.SetBool("EvolveA", true);
        hasEvolved = true;
        canMove = true;

        // 启动攻击模式
        if (attackPattern == null)
        {
            attackPattern = StartCoroutine(AttackPattern());
        }
    }

    private IEnumerator AttackPattern()
    {
        while (player.currentState != Player.State.Over)
        {
            // 旋转枪口
            transform.Rotate(Vector3.forward * RotateAngle);
            yield return new WaitForSeconds(0.3f);

            // 四方向齐射
            Shoot();
            yield return new WaitForSeconds(PatternInterval);
        }
    }

    private void BaseMovement()
    {
        Move(player.currentState == Player.State.Eating ? 2 : 1);
    }

    public override void Shoot()
    {
        foreach (var point in shootPoints)
        {
            if (point == null) continue;

            var bullet = GetBulletFromPool();
            if (bullet != null)
            {
                InitializeBullet(bullet, point);
            }
        }
    }

    private GameObject GetBulletFromPool()
    {
        return ObjectPoolManager.Instance.GetObject("Bullet");
    }

    private void InitializeBullet(GameObject bullet, Transform point)
    {
        // 设置子弹初始位置和方向
        bullet.transform.SetPositionAndRotation(point.position, point.rotation);

        // 设置移动方向（由子弹自身处理移动逻辑）
        var bulletComponent = bullet.GetComponent<Bullet>();
        if (bulletComponent != null)
        {
            bulletComponent.dir = (point.up);
        }
    }

    public override void Initialize()
    {
        enemyType = "EnemyC基类";
        evolutionTime = 8f;
        minPosition = new Vector2(-5.7f, -4.1f);
        maxPosition = new Vector2(8f, 3.95f);
        hasEvolved = false;
    }

    void OnDisable()
    {
        Health = 4;
        enabled = true;
        isDead = false;
        hasEvolved = false;
        //bulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet");
    }
}