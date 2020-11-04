using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PickupTextPrompt : MonoBehaviour
{
    public TextMeshProUGUI Text;

    bool fadeOut = false;

    private void OnEnable()
    {
        Text.enabled = true;
        Text.alpha = 0;
    }

    private void OnDisable()
    {
        Text.enabled = false;
    }

    private void Update()
    {
        if (fadeOut)
        {
            Text.alpha = Mathf.MoveTowards(Text.alpha, 0, 10f * Time.deltaTime);

            if (Text.alpha == 0)
            {
                enabled = false;
            }
        }
        else
        {
            Text.alpha = Mathf.MoveTowards(Text.alpha, 1, 10f * Time.deltaTime);
        }
    }

    public void EnableText()
    {
        fadeOut = false;
        if(!enabled) enabled = true;
    }

    public void DisableText()
    {
        fadeOut = true;
    }
}
