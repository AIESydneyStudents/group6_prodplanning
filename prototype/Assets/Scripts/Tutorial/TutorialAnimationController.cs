using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialAnimationController : MonoBehaviour
{
    bool playAnimations = false;
    Animator playerAni;
    void Start()
    {
        playerAni = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayAnimation();
        }

#if UNITY_EDITOR
        if(Input.GetKey(KeyCode.LeftShift))
        {
            playerAni.speed = 10f;
        }
        else
        {
            playerAni.speed = 1f;
        }
#endif

        if (playerAni.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !playerAni.IsInTransition(0))
        {
            playerAni.enabled = false;
            GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<F_PlayerMovement>().enabled = true;
        }

        if (!playAnimations) return;
    }

    void PlayAnimation()
    {
        playerAni.Play("tut_LookLeft");
    }
}
