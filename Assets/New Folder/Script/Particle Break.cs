using UnityEngine;
using System.Collections;
public class ParticleBreak : MonoBehaviour
{
    public ParticleSystem particleTailing;
    private void OnMouseDown()
    {
        //����ǽ��
        StartCoroutine(Particles());
    }
    private IEnumerator Particles()
    {
        // ����������Ч
        particleTailing.Play();
        // ����������ϵͳ���뵱ǰ����㼶
        particleTailing.transform.SetParent(null);
        // ���õ�ǰ���壨ֹͣ���������
        gameObject.SetActive(false);
        // �ȴ����Ӳ������
        yield return new WaitForSeconds(particleTailing.main.duration);
        // ���ٶ������ڵ�����ϵͳ
        Destroy(particleTailing.gameObject);
    }
}