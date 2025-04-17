using UnityEngine;
using System.Collections;
public class ParticleBreak : MonoBehaviour
{
    public ParticleSystem particleTailing;
    private void OnMouseDown()
    {
        //碰到墙壁
        StartCoroutine(Particles());
    }
    private IEnumerator Particles()
    {
        // 播放粒子特效
        particleTailing.Play();
        // 立即将粒子系统脱离当前物体层级
        particleTailing.transform.SetParent(null);
        // 禁用当前物体（停止后续点击）
        gameObject.SetActive(false);
        // 等待粒子播放完成
        yield return new WaitForSeconds(particleTailing.main.duration);
        // 销毁独立存在的粒子系统
        Destroy(particleTailing.gameObject);
    }
}