using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    //范围
    public Vector2 minPosition = new Vector2(-5.7f,-4.1f);
    public Vector2 maxPosition = new Vector2( 8f,3.95f ) ;
    
    //移动
    public bool canMove = false;
    public float moveSpeed = 5f;
    public Vector2 dir;

    public string enemyType; // 如 "EnemyA"
    public GameObject bulletPrefab; // 子弹Prefab
    //public ObjectPoolManager poolManager; // 对象池管理器
    //成长
    public float evolutionTime = 10f; // 幼体到成体的进化时间
    public bool hasEvolved = false; // 是否已经进化

    //减慢速率
    public float slowScale = 10f;
    //获取角色状态信息
    public Player player;

    //人物状态
    public int Health = 4;
    public int damage = 1;
    protected bool isDead = false;
    public ParticleSystem particleTailing;
    void Start()
    {
        //poolManager = gameObject.GetComponent<ObjectPoolManager>();
        player = GameObject.Find("Player").GetComponent<Player>();
        dir = Random.insideUnitCircle.normalized;
        Spawn();
    }

    public void Spawn()
    {
        Vector2 spawnPosition = new Vector2(Random.Range(minPosition.x,maxPosition.x),
            Random.Range(minPosition.y,maxPosition.y));
        transform.position = spawnPosition;
    }

    public void Move(int num)
    {
        if (canMove)
        {
            if (num == 1)
            {
                Vector2 newPosition = (Vector2)transform.position + dir * moveSpeed * Time.deltaTime;
                if (newPosition.x > maxPosition.x || newPosition.y > maxPosition.y)
                {
                    if (newPosition.x > maxPosition.x)
                    {
                        dir.x *= -1;
                    }
                    if (newPosition.y > maxPosition.y)
                    {
                        dir.y *= -1;
                    }
                    newPosition = (Vector2)transform.position + dir * moveSpeed * Time.deltaTime;
                }

                if (newPosition.x < minPosition.x || newPosition.y < minPosition.y)
                {
                    if (newPosition.x < maxPosition.x)
                    {
                        dir.x *= -1;
                    }
                    if (newPosition.y < maxPosition.y)
                    {
                        dir.y *= -1;
                    }
                    newPosition = (Vector2)transform.position + dir * moveSpeed * Time.deltaTime;
                }

                transform.position = newPosition;
            }

            if(num == 2)
            {
                Vector2 newPosition = (Vector2)transform.position + dir * moveSpeed/slowScale * Time.deltaTime;
                if (newPosition.x > maxPosition.x || newPosition.y > maxPosition.y)
                {
                    if (newPosition.x > maxPosition.x)
                    {
                        dir.x *= -1;
                    }
                    if (newPosition.y > maxPosition.y)
                    {
                        dir.y *= -1;
                    }
                    newPosition = (Vector2)transform.position + dir * moveSpeed/slowScale * Time.deltaTime;
                }

                if (newPosition.x < minPosition.x || newPosition.y < minPosition.y)
                {
                    if (newPosition.x < maxPosition.x)
                    {
                        dir.x *= -1;
                    }
                    if (newPosition.y < maxPosition.y)
                    {
                        dir.y *= -1;
                    }
                    newPosition = (Vector2)transform.position + dir * moveSpeed/slowScale * Time.deltaTime;
                }

                transform.position = newPosition;
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Bullet")
        {
            if (player.currentState == Player.State.Normal)
            {
                if (other.gameObject.GetComponent<Bullet>().index == 0 ||
                    other.gameObject.GetComponent<Bullet>().index == 1
                    || other.gameObject.GetComponent<Bullet>().index == 2)
                {
                    Health -= damage;
                }
            }           
        }
    }
    public abstract void Initialize();
    public abstract void Shoot(); // 子类实现不同射击逻辑

    protected IEnumerator Particles()
    {
        // 播放粒子特效
        if (particleTailing != null)
        {
            particleTailing.Play();
            yield return new WaitForSeconds(0.5f);
        }

        // 手动停止协程，避免 OnDisable 的干扰
        StopAllCoroutines();

        // 回收对象
        if (ObjectPoolManager.Instance != null && gameObject.activeSelf)
        {
            ObjectPoolManager.Instance.ReturnObject(enemyType, gameObject);
        }
    }

    protected IEnumerator hurtFlash()
    {
        Material mt = this.GetComponent<SpriteRenderer>().material;
        float x = 1f;
        float maxAlphaValue = 1f;
        float alphaValue = maxAlphaValue / x;
        mt.SetFloat("_Alpha", alphaValue);
        yield return 0;
        while (mt.GetFloat("_Alpha") > 0)
        {
            x += 0.1f;
            alphaValue = maxAlphaValue / x;
            alphaValue = alphaValue < 0.02 ? 0 : alphaValue;
            mt.SetFloat("_Alpha", alphaValue);
            yield return 0;
        }
    }
   
}