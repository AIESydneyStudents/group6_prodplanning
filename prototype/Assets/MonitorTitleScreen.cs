using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonitorTitleScreen : MonoBehaviour
{

    public CanvasGroup MainGroup;
    public CanvasGroup HowToPlayGroup;

    bool fadingIn = true;

    private bool howToPlay = false;
    private float alpha = 1;

    // Update is called once per frame
    void Update()
    {
        MainGroup.alpha = alpha;
        HowToPlayGroup.alpha = 1f - alpha;

        if (howToPlay)
        {
            alpha = Mathf.MoveTowards(alpha, 0, 5f * Time.deltaTime);
        }
        else
        {
            alpha = Mathf.MoveTowards(alpha, 1, 5f * Time.deltaTime);
        }
    }

    public void OpenHowToPlay()
    {
        howToPlay = true;
        MainGroup.interactable = false;
        MainGroup.blocksRaycasts = false;
        HowToPlayGroup.interactable = true;
        HowToPlayGroup.blocksRaycasts = true;
    }

    public void CloseHowToPlay()
    {
        howToPlay = false;
        MainGroup.interactable = true;
        MainGroup.blocksRaycasts = true;
        HowToPlayGroup.interactable = false;
        HowToPlayGroup.blocksRaycasts = false;
    }
}
