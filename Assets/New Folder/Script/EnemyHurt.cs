using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
public class EnemyHurt : MonoBehaviour
{
    public float hp;
    void Update()
    {
        
    }
    private void OnMouseDown()
    {
        hurt();
    }
    private void hurt()
    {
        /*this.hp -= dmg;
        if (this.hp < 0)
        {
            this.die();
        }*/
            StartCoroutine(hurtFlash());
    }
    private void die()
    {

    }
    private IEnumerator hurtFlash()
    {
        Material mt = this.GetComponent<SpriteRenderer>().material;
        float x = 1f;
        float maxAlphaValue = 1f;
        float alphaValue = maxAlphaValue / x;
        mt.SetFloat("_Alpha", alphaValue);
        yield return 0;
        while(mt.GetFloat("_Alpha")>0)
        {
            x += 0.1f;
            alphaValue = maxAlphaValue / x;
            alphaValue = alphaValue < 0.02 ? 0 : alphaValue;
            mt.SetFloat("_Alpha", alphaValue);
            yield return 0;
        }
    }
}
