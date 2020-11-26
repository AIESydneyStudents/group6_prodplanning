using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelFadeIn : MonoBehaviour
{
    public CanvasGroup GroupToFade;
    public bool FadeActive = false;
    public bool FadeIn = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(FadeActive)
        {
            if(FadeIn) // Fading in
            {
                GroupToFade.alpha = Mathf.MoveTowards(GroupToFade.alpha,1,  2f * Time.deltaTime);
                if (GroupToFade.alpha == 1) FadeActive = false;
            }
            else // Fading Out
            {
                GroupToFade.alpha = Mathf.MoveTowards(GroupToFade.alpha, 0, 2f * Time.deltaTime);
                if (GroupToFade.alpha == 1) FadeActive = false;
            }
        }
    }

    public void StartFadeIn()
    {
        FadeIn = true;
        FadeActive = true;
    }

    public void StartFadeOut()
    {
        FadeIn = false;
        FadeActive = true;
    }
}
