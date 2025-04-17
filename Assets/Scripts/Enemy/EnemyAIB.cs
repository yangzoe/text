using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyAIB : EnemyBase
{
    private Transform bulletBornPosition1;
    private Transform bulletBornPosition2;
    private Transform bulletBornPosition3;
    public Sprite adultSprite;
    private float timer = 0f;
    public Image image;

    // 新增攻击参数
    private Coroutine attackCoroutine;
    private const float rotateAngle = 45f;    // 每次旋转角度
    private const float shootInterval = 1f;   // 射击间隔
    private const float rotateDelay = 0.2f;   // 旋转后射击延迟

    public Animator animator;
    void Start()
    {

        animator = GetComponent<Animator>();
        player = GameObject.Find("Player").GetComponent<Player>();

        bulletBornPosition1 = transform.GetChild(0);
        bulletBornPosition2 = transform.GetChild(1);
        bulletBornPosition3 = transform.GetChild(2);
        Initialize();
    }
    void Enable()
    {

        
        canMove = false; // 初始禁用移动
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
            if (adultSprite != null)
            {
                GetComponent<SpriteRenderer>().sprite = adultSprite;
                animator.SetBool("EvolveB", true);
            }
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

        // 启动分步攻击协程
        if (attackCoroutine == null)
        {
            attackCoroutine = StartCoroutine(StepAttack());
        }
    }

    // 新版分步攻击模式
    private IEnumerator StepAttack()
    {
        while (player.currentState != Player.State.Over)
        {
            // 旋转阶段
            //transform.Rotate(Vector3.forward * rotateAngle);
            //yield return new WaitForSeconds(rotateDelay);

            // 射击阶段
            Shoot();
            yield return new WaitForSeconds(shootInterval);
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
        Transform[] shootPoints = { bulletBornPosition1, bulletBornPosition2, bulletBornPosition3 };

        foreach (var point in shootPoints)
        {
            if (bulletPrefab == null || point == null) continue;

            GameObject bullet = ObjectPoolManager.Instance.GetObject("Bullet");
            bullet.transform.position = point.position;
            bullet.transform.rotation = point.rotation;
            bullet.GetComponent<Bullet>().dir = point.up;
        }
    }

    public override void Initialize()
    {
        enemyType = "EnemyB";
        evolutionTime = 8f;
        minPosition = new Vector2(-5.7f, -4.1f);
        maxPosition = new Vector2(8f, 3.95f);
        canMove = false;
        hasEvolved = false;
    }

    void OnDisable()
    {
        Health = 4;
        enabled = true;
        hasEvolved = false;
        isDead = false;
    }
}