using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public ParticleSystem particleTailing;
    public Vector2 minBoundary;
    public Vector2 maxBoundary;
    public string Name;
    //�ƶ�
    public float bulletSpeed = 5f;
    public Vector2 dir;
    
    public int index = -1;  //0�ǽ�ɫֱ�ӽӴ���1���ӵ��Ӵ�
    public float slowScale = 10f;
    
    private ObjectPoolManager poolManager;
    private Bomb bomb;
    private Player player;

    void Start()
    {
        player = GameObject.FindObjectOfType<Player>();
        poolManager = ObjectPoolManager.Instance;
        bomb = GetComponent<Bomb>();
    }

    void FixedUpdate()
    {
        if(player.currentState == Player.State.Eating || player.currentState == Player.State.Full)
        {
            Move(2);
        }

        else if (player.currentState == Player.State.Normal || player.currentState == Player.State.Rebirth)
        {
            if (index == -1)
            {
                Move(1);
            }
            if (index == 0 || index == 1 || index == 2)
            {
                //�˴���ӱ�ը�߼�
                bomb.Explode();
            }
        }
    }

    void Update()
    {
        if(transform.position.x < minBoundary.x ||transform.position.x> maxBoundary.x
            || transform.position.y < minBoundary.y || transform.position.y > maxBoundary.y)
        {
            StartCoroutine(Particles());
        } 
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(index == 0)
        {
            if(other.gameObject.tag == "Bullet")
            {
                other.gameObject.GetComponent<Bullet>().GetComponent<SpriteRenderer>().color = 
                    new Color(246/255f, 183/255f, 27/255f);
                other.gameObject.GetComponent<Bullet>().index = 1;      
            }
        }
    }

    void Move(int num)
    {
        if(num == 1)
        {
            this.transform.position = this.transform.position - new Vector3( dir.x * Time.deltaTime * bulletSpeed,
                dir.y * Time.deltaTime * bulletSpeed,0 );
        }
        if(num == 2)
        {
            this.transform.position = this.transform.position - new Vector3(dir.x * Time.deltaTime * bulletSpeed/slowScale,
                dir.y * Time.deltaTime * bulletSpeed/slowScale, 0);
        }
    }

    void Recycle()   //�����ӵ��������
    {
        this.GetComponent<SpriteRenderer>().color = Color.white;
        this.index = -1;
        poolManager.ReturnObject("Bullet", gameObject);
    }

    private IEnumerator Particles()
    {
        // ����������Ч
        particleTailing.Play();
        // �ȴ����Ӳ������
        yield return new WaitForSeconds(0.1f);

        particleTailing.Stop();
        Recycle();
    }

}
