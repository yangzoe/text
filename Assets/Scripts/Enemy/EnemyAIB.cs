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

    // ������������
    private Coroutine attackCoroutine;
    private const float rotateAngle = 45f;    // ÿ����ת�Ƕ�
    private const float shootInterval = 1f;   // ������
    private const float rotateDelay = 0.2f;   // ��ת������ӳ�

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

        
        canMove = false; // ��ʼ�����ƶ�
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

        // �����ֲ�����Э��
        if (attackCoroutine == null)
        {
            attackCoroutine = StartCoroutine(StepAttack());
        }
    }

    // �°�ֲ�����ģʽ
    private IEnumerator StepAttack()
    {
        while (player.currentState != Player.State.Over)
        {
            // ��ת�׶�
            //transform.Rotate(Vector3.forward * rotateAngle);
            //yield return new WaitForSeconds(rotateDelay);

            // ����׶�
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