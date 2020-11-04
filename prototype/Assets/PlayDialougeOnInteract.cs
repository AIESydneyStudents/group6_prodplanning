using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayDialougeOnInteract : MonoBehaviour
{
    public bool OnlyOnce = true;
    public DialougeSequence DialougeToPlay;

    private bool hasPlayed = false;
    private Interactable interactScript;
    private DialougeManager dialougeManager;

    // Start is called before the first frame update
    void Start()
    {
        interactScript = GetComponent<Interactable>();
        interactScript.OnInteract.AddListener(PlayDialouge);

        dialougeManager = GameObject.FindGameObjectWithTag("DialougeSystem").GetComponent<DialougeManager>();
    }

    private void PlayDialouge()
    {
        if(!hasPlayed)
        {
            dialougeManager.PlayDialouge(DialougeToPlay);
            if (OnlyOnce) hasPlayed = true;
        }
    }
}
