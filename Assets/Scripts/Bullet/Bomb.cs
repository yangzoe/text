using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    // ��ը�뾶������
    public float explosionRadius = 5f;
    public float explosionForce = 1000f;
    //public GameObject explosionEffect; // ��ը������Ч
    public float waitTime = .2f;
    private ObjectPoolManager poolManager;
    // ��ը����Ҫ��ըʱ����

    private void Awake()
    {
        poolManager = ObjectPoolManager.Instance;
    }

    public void Explode()
    {
        // 1. ��ը��λ�ü�ⷶΧ�ڵ�����
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D nearbyObject in colliders)
        {
            // 2. ��ȡ����ĸ��壨������ڣ�
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
                        // ���㱬ը����
                        Vector2 explosionDirection = nearbyObject.transform.position - transform.position;

                        // ���ݾ��������ը��������Խ����Խ��
                        float distance = Vector2.Distance(nearbyObject.transform.position, transform.position);
                        float force = explosionForce * (1 - (distance / explosionRadius));

                        // ������ʩ����
                        rb.AddForce(explosionDirection.normalized * force);
                    }
                }
            }
        }

        // 3. ��ʾ��ը���Ӿ�Ч��
        //ShowExplosionEffects();
        StartCoroutine(WaitAndExplode());
        // 4. ����ը������
        
    }

    // ��ѡ����ը���Ӿ�Ч��
    /*void ShowExplosionEffects()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
    }
    */
    // ��ѡ��Ϊ���ڱ༭���п��ӻ���ը��Χ
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
