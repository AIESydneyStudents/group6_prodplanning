using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneDialouge : MonoBehaviour
{
    public DialougeSequence SequenceToPlay;

    private Interactable interactScript;
    private DialougeManager dialougeManager;
    private bool hasPlayed;

    private int interactableLayer;
    private int parentLayer;

    void Start()
    {
        interactScript = GetComponent<Interactable>();
        interactScript.OnInteract.AddListener(OnInteract);

        interactableLayer = gameObject.layer;
        parentLayer = gameObject.transform.parent.gameObject.layer;

        //So it cannot interact
        gameObject.layer = parentLayer;

        dialougeManager = GameObject.FindGameObjectWithTag("DialougeSystem").GetComponent<DialougeManager>();
    }

    public void ActivatePhone()
    {
        gameObject.layer = interactableLayer;
    }

    public void OnInteract()
    {
        if (!hasPlayed)
        {
            gameObject.layer = parentLayer;
            hasPlayed = true;
            dialougeManager.PlayDialouge(SequenceToPlay);
        }
    }

    public bool PhoneDone()
    {
        return (hasPlayed && !dialougeManager.IsPlayingDialouge);
    }
}
