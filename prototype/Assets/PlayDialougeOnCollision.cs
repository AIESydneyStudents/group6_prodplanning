using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayDialougeOnCollision : MonoBehaviour
{
    public bool OnlyOnce = true;
    public DialougeSequence DialougeToPlay;

    private bool hasPlayed = false;
    private DialougeManager dialougeManager;

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
                Destroy(gameObject);
            };
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            PlayDialouge();
        }
    }
}
