using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneDialouge : MonoBehaviour
{
    public DialougeSequence SequenceToPlay;
    public AudioClip AnswerSound;
    public MeshRenderer PhoneScreen;

    private Interactable interactScript;
    private DialougeManager dialougeManager;
    private AudioSource audioSource;
    private bool hasPlayed;

    private int interactableLayer;
    private int parentLayer;

    void Start()
    {
        interactScript = GetComponent<Interactable>();
        audioSource = GetComponent<AudioSource>();
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
        audioSource.Play();
    }

    private void Update()
    {
        if(!hasPlayed && audioSource.isPlaying)
        {
            if(audioSource.time < audioSource.clip.length/2f)
            {
                PhoneScreen.enabled = true;
            }
            else
            {
                PhoneScreen.enabled = false;
            }
        }
    }

    public void OnInteract()
    {
        if (!hasPlayed)
        {
            gameObject.layer = parentLayer;
            hasPlayed = true;
            dialougeManager.PlayDialouge(SequenceToPlay);
            audioSource.Stop();
            audioSource.loop = false;
            if(AnswerSound != null) audioSource.PlayOneShot(AnswerSound);
            PhoneScreen.enabled = false;
        }
    }

    public bool PhoneDone()
    {
        return (hasPlayed && !dialougeManager.IsPlayingDialouge);
    }
}
