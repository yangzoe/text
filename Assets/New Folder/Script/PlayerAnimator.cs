using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator animator;
    void Start()
    {
        //Normal
        animator.SetBool("EvolvePlayer", false);
        animator.SetBool("EvolvePlayer(back)", false);
    }

    private void OnMouseDown()
    {
        //Eating
        animator.SetBool("EvolvePlayer", true);
        animator.SetBool("EvolvePlayer(back)",false);
        
        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //Normal¡ªBack
            animator.SetBool("EvolvePlayer(back)", true);
            animator.SetBool("EvolvePlayer", false);
        }
    }
}
