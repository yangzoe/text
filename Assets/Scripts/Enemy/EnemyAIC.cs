using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyAIC : EnemyBase
{
    private Transform bulletBornPosition1;
    private Transform bulletBornPosition2;
    private Transform bulletBornPosition3;
    private Transform bulletBornPosition4;

    public Sprite adultSprite;
    private float timer = 0f;
    public Image image;

    // 攻击参数
    private Coroutine attackCoroutine;
    private const float rotateAngle = 90f;    // 每次旋转角度
    private const float shootInterval = 0.8f; // 射击间隔
    private const float rotateDelay = 0.15f;  // 旋转后射击延迟

    public Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.Find("Player").GetComponent<Player>();
        bulletBornPosition1 = transform.GetChild(0);
        bulletBornPosition2 = transform.GetChild(1);
        bulletBornPosition3 = transform.GetChild(2);
        bulletBornPosition4 = transform.GetChild(3);
        Initialize();
    }
    void Enable()
    {



        dir = Random.insideUnitCircle.normalized;

        hasEvolved = false;
        canMove = false; // 初始禁用移动
        Health = 4;
        enabled = true;
        Health = 4;
        enabled = true;
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

        timer += Time.deltaTime;
        if (timer >= evolutionTime && !hasEvolved)
        {
            Evolve();
        }

        if (hasEvolved)
        {
            HandleMovement();
        }
    }

    private void Evolve()
    {
        hasEvolved = true;
        canMove = true;
        dir = Random.insideUnitCircle.normalized;

        if (adultSprite != null)
        {
            GetComponent<SpriteRenderer>().sprite = adultSprite;
            animator.SetBool("EvolveA",true);
        }

        // 启动菱形攻击模式
        if (attackCoroutine == null)
        {
            attackCoroutine = StartCoroutine(DiamondAttackPattern());
        }
    }

    // 菱形攻击模式
    private IEnumerator DiamondAttackPattern()
    {
        while (player.currentState != Player.State.Over)
        {
            // 第一阶段：十字射击
            ShootCross();
            yield return new WaitForSeconds(rotateDelay);

            // 第二阶段：对角射击
            ShootDiagonal();
            yield return new WaitForSeconds(shootInterval);
        }
    }

    private void ShootCross()
    {
        ShootPoints(new[] { bulletBornPosition1, bulletBornPosition3 });
    }

    private void ShootDiagonal()
    {
        ShootPoints(new[] { bulletBornPosition2, bulletBornPosition4 });
    }

    private void ShootPoints(Transform[] points)
    {
        foreach (var point in points)
        {
            if (bulletPrefab == null || point == null) continue;

            GameObject bullet = ObjectPoolManager.Instance.GetObject("Bullet");
            if (bullet == null) continue;

            bullet.transform.position = point.position;
            bullet.transform.rotation = point.rotation;
            bullet.GetComponent<Bullet>().dir = point.up;
        }
    }

    private void HandleMovement()
    {
        if (player.currentState == Player.State.Eating ||
            player.currentState == Player.State.Full)
        {
            Move(2);
        }
        else
        {
            Move(1);
        }
    }

    public override void Shoot()
    {
        Transform[] shootPoints = {
            bulletBornPosition1, bulletBornPosition2,
            bulletBornPosition3, bulletBornPosition4
        };

        foreach (var point in shootPoints)
        {
            if (bulletPrefab == null || point == null) continue;

            GameObject bullet = ObjectPoolManager.Instance.GetObject("Bullet");
            if (bullet == null) continue;

            bullet.transform.position = point.position;
            bullet.transform.rotation = point.rotation;
            bullet.GetComponent<Bullet>().dir = point.up;
        }
    }

    public override void Initialize()
    {
        enemyType = "EnemyC";
        evolutionTime = 8f;
        minPosition = new Vector2(-5.7f, -4.1f);
        maxPosition = new Vector2(8f, 3.95f);
        canMove = false;
       
    }
    void OnDisable()
    {
        Health = 4;
        enabled = true;
        isDead = false;
        hasEvolved = false;
        bulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet");
    }
}