using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemSpecialText : MonoBehaviour
{
    public PanelFadeIn PanelFade;
    public TextMeshProUGUI  Text;

    public void ActivatePanel(string ItemText)
    {
        Text.text = ItemText;
        PanelFade.StartFadeIn();
    }

    public void DeactivatePanel()
    {
        PanelFade.StartFadeOut();
    }
}
