using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    // 爆炸半径和力量
    public float explosionRadius = 5f;
    public float explosionForce = 1000f;
    //public GameObject explosionEffect; // 爆炸粒子特效
    public float waitTime = .2f;
    private ObjectPoolManager poolManager;
    // 当炸弹需要爆炸时调用

    private void Awake()
    {
        poolManager = ObjectPoolManager.Instance;
    }

    public void Explode()
    {
        // 1. 在炸弹位置检测范围内的物体
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D nearbyObject in colliders)
        {
            // 2. 获取物体的刚体（如果存在）
            Rigidbody2D rb = nearbyObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                if (rb.tag != "Player") 
                {
                    if (rb.tag == "Bullet")
                    {
                        Bullet bullet = rb.gameObject.GetComponent<Bullet>();
                        if (bullet.index == -1)
                        {
                            bullet.index = 2;
                            bullet.GetComponent<SpriteRenderer>().color = new Color(246/255f, 183/255f, 27/255f);
                        }
                        // 计算爆炸方向
                        Vector2 explosionDirection = nearbyObject.transform.position - transform.position;

                        // 根据距离调整爆炸力，距离越近力越大
                        float distance = Vector2.Distance(nearbyObject.transform.position, transform.position);
                        float force = explosionForce * (1 - (distance / explosionRadius));

                        // 向物体施加力
                        rb.AddForce(explosionDirection.normalized * force);
                    }
                }
            }
        }

        // 3. 显示爆炸的视觉效果
        //ShowExplosionEffects();
        StartCoroutine(WaitAndExplode());
        // 4. 销毁炸弹物体
        
    }

    // 可选：爆炸的视觉效果
    /*void ShowExplosionEffects()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
    }
    */
    // 可选：为了在编辑器中可视化爆炸范围
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    IEnumerator WaitAndExplode()
    {
        yield return new WaitForSeconds(waitTime);
        this.GetComponent<Bullet>().index = -1;
        this.GetComponent<SpriteRenderer>().color = Color.white;
        poolManager.ReturnObject("Bullet", this.gameObject);
    }
}
