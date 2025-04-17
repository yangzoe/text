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

    // ��������
    private Coroutine attackCoroutine;
    private const float rotateAngle = 90f;    // ÿ����ת�Ƕ�
    private const float shootInterval = 0.8f; // ������
    private const float rotateDelay = 0.15f;  // ��ת������ӳ�

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
        canMove = false; // ��ʼ�����ƶ�
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

        // �������ι���ģʽ
        if (attackCoroutine == null)
        {
            attackCoroutine = StartCoroutine(DiamondAttackPattern());
        }
    }

    // ���ι���ģʽ
    private IEnumerator DiamondAttackPattern()
    {
        while (player.currentState != Player.State.Over)
        {
            // ��һ�׶Σ�ʮ�����
            ShootCross();
            yield return new WaitForSeconds(rotateDelay);

            // �ڶ��׶Σ��Խ����
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