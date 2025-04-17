using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyAIA基类 : EnemyBase
{
    private bool isFirstEvolved = true;
    private Transform bulletBornPosition;
    public Sprite adultSprite;
    private float timer = 0f;
    private Animator animator;
    public Image image;

    // 攻击控制变量
    private Coroutine attackCoroutine;
    private const int bulletsPerCircle = 16; // 每圈子弹数量
    private const float rotateSpeed = 22.5f;  // 每次旋转角度（360/16）
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.Find("Player").GetComponent<Player>();

        bulletBornPosition = transform.GetChild(0);
        Initialize();
    }
    void Enable()
    {


        // 永久禁用移动
        canMove = false;
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
    }

    private void Evolve()
    {
        // 启动旋转射击模式
        attackCoroutine = StartCoroutine(RotatingAttackPattern());
        hasEvolved = true;
    }

    // 旋转射击模式
    private IEnumerator RotatingAttackPattern()
    {
        while (player.currentState != Player.State.Over)
        {
            // 单圈射击
            for (int i = 0; i < bulletsPerCircle; i++)
            {
                transform.Rotate(Vector3.forward * rotateSpeed);
                Shoot();
                yield return new WaitForSeconds(0.5f); // 调整射击间隔
            }

            // 每圈之间的停顿
            yield return new WaitForSeconds(1f);
        }
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
        enemyType = "EnemyA基类";
        evolutionTime = 8f;
        minPosition = new Vector2(-5.7f, -4.1f);
        maxPosition = new Vector2(8f, 3.95f);
        canMove = false; // 确保永远不能移动
        hasEvolved = false;
    }

    void OnDisable()
    {
        Health = 4;
        enabled = true;
        hasEvolved = false;
    }
}