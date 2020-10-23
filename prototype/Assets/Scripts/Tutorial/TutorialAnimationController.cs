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

        if (playerAni.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !playerAni.IsInTransition(0))
        {
            GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<F_PlayerMovement>().enabled = true;
            playerAni.enabled = false;
        }

        if (!playAnimations) return;
    }

    void PlayAnimation()
    {
        playerAni.Play("tut_LookLeft");
    }
}
