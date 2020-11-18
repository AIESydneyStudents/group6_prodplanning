using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CleanBedTask : Task
{
    public MeshFilter Fitter;
    public Mesh CleanMesh;
    public Image PanelToFade;

    private F_PlayerMovement player;
    private bool fadeingOut = false;

    private Color fadeColour = new Color(0,0,0,0);

    // Start is called before the first frame update
    void Start()
    {
        
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<F_PlayerMovement>();
    }

    private void Update()
    {
        if(taskRunning)
        {
            if(fadeingOut)
            {
                fadeColour.a = Mathf.MoveTowards(fadeColour.a, 0, 5f * Time.deltaTime);

                if (fadeColour.a <= 0)
                {
                    TaskFinished();
                }
            }
            else
            {
                fadeColour.a = Mathf.MoveTowards(fadeColour.a,1,5f*Time.deltaTime);

                if(fadeColour.a >= 1)
                {
                    StartCoroutine(WaitForFadeOut());
                }
            }

            PanelToFade.color = fadeColour;
        }
    }

    IEnumerator WaitForFadeOut()
    {
        yield return new WaitForSeconds(1.5f);

        Fitter.mesh = CleanMesh;
        fadeingOut = true;
        player.ChangeState(F_PlayerMovement.PlayerState.Gameplay);
    }

    public void CleanBed()
    {
        if (!taskFinished)
        {
            taskRunning = true;
            player.ChangeState(F_PlayerMovement.PlayerState.Cutscene);
            gameObject.layer = transform.parent.gameObject.layer;
        }
        //Play sound here
    }
}
