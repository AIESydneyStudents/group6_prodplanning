using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FinalCutsceneTrigger : MonoBehaviour
{
    
    public CanvasGroup endScreenCanvas;
    private F_PlayerMovement player;
    private bool started = false;
    private bool FadeOutStarted = false;
    private DialougeManager dialougeManager;

    void Start()
    {
        dialougeManager = GameObject.FindGameObjectWithTag("DialougeSystem").GetComponent<DialougeManager>();
    }


    // Update is called once per frame
    void Update()
    {
        if(FadeOutStarted && !dialougeManager.IsPlayingDialouge)
        {
            endScreenCanvas.alpha = Mathf.MoveTowards(endScreenCanvas.alpha, 1, 1 * Time.deltaTime);
            if(endScreenCanvas.alpha == 1)
            {
                endScreenCanvas.blocksRaycasts = true;
                endScreenCanvas.interactable = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!started)
        {
            if(other.gameObject.tag == "Player")
            {
                player = other.gameObject.GetComponent<F_PlayerMovement>();
                started = true;
                StartCoroutine(DelayCutscene());
            }
        }
    }

    IEnumerator DelayCutscene()
    {
        player.ChangeState(F_PlayerMovement.PlayerState.Cutscene);

        yield return new WaitForSeconds(3);
        FadeOutStarted = true;
    }
}
