using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayDialougeOnCollision : MonoBehaviour
{
    public bool OnlyOnce = true;
    public bool DontDestory = false;
    public DialougeSequence DialougeToPlay;

    private bool hasPlayed = false;
    private DialougeManager dialougeManager;

    public GameObject InterfacePrompt = null;

    // Start is called before the first frame update
    void Start()
    {
        dialougeManager = GameObject.FindGameObjectWithTag("DialougeSystem").GetComponent<DialougeManager>();
    }

    private void PlayDialouge()
    {
        if (!hasPlayed)
        {
            dialougeManager.PlayDialouge(DialougeToPlay);
            if (OnlyOnce) 
            {
                hasPlayed = true;
                if(!DontDestory) Destroy(gameObject);
            };
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            PlayDialouge();

            if (InterfacePrompt != null)
                InterfacePrompt.SetActive(true);
        }
    }
}
