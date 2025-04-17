using UnityEngine;
using System.Collections;
public class GameStart : MonoBehaviour
{
    [Header("��������")]
    public GameObject Text;
    private GameObject Player;
    public GameObject Manager;

    [Header("����ϵͳ")]
    public GameObject gameObject1;
    public GameObject gameObject2;
    public GameObject gameObject3;
    public GameObject gameObject4;
    public ParticleSystem[] particleTailings = new ParticleSystem[3];
    public ParticleSystem[] particleTailings1 = new ParticleSystem[4];
    private bool isBegin;

    public AudioSource audioSource_particle;
    void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        if (Player != null && Manager != null)
        {
            Player.SetActive(false);
            Manager.SetActive(false);
        }
    }
    private void Start()
    {
        isBegin = false;
    }
    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space)||Input.anyKeyDown) && isBegin == false )
        {
            isBegin = true;
            // �������壨ֹͣ����������
            gameObject1.SetActive(false);
            gameObject2.SetActive(false);
            gameObject3.SetActive(false);

            audioSource_particle.Play();
            foreach (ParticleSystem ps in particleTailings)
            {
                ps.Play();
                    
                // ʹ��Э���ӳ�����
                StartCoroutine(DestroyAfterDelay(ps.gameObject));
            }
            StartCoroutine(DelayedParticleStart());
        }
    }
    // Э��ʵ���ӳ�����
    private IEnumerator DestroyAfterDelay(GameObject obj)
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(obj);
    }
    private IEnumerator DelayedParticleStart()
    {
        // �ȴ�0.6���ӳ�
        yield return new WaitForSeconds(0.6f);

        // �������Ӳ���Э��
        yield return StartCoroutine(PlayAllParticles());
    }
    private IEnumerator PlayAllParticles()
    {
        gameObject4.SetActive(false);
        // ͬʱ������������ϵͳ
        foreach (ParticleSystem ps1 in particleTailings1)
        {
            ps1.Play(); 
            audioSource_particle.Play();
        }
        float maxDuration = 0;
        foreach (ParticleSystem ps1 in particleTailings1)
        {
            float totalTime = ps1.main.duration + ps1.main.startLifetime.constantMax;
            if (totalTime > maxDuration) maxDuration = totalTime;
        }
        { 
            Player.SetActive(true);
            Manager.SetActive(true);

            Destroy(Text);
        }
        yield return new WaitForSeconds(maxDuration);
        foreach (ParticleSystem ps1 in particleTailings1)
        {
            Destroy(this.transform.parent.gameObject);
        }

    }
}