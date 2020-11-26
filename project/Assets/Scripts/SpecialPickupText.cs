using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialPickupText : MonoBehaviour
{
    [TextArea(5,10)]
    public string SpecialText = "";
    public DialougeSequence OptionalSequence;

    private PickupObject pickupManager;
    private ItemSpecialText specialTextUI;

    private DialougeManager dialougeManager;
    private bool dialougePlayed = false;

    // Start is called before the first frame update
    void Start()
    {
        pickupManager = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PickupObject>();
        specialTextUI = GameObject.FindGameObjectWithTag("SpecialTextPanel").GetComponent<ItemSpecialText>();
        dialougeManager = GameObject.FindGameObjectWithTag("DialougeSystem").GetComponent<DialougeManager>();
    }

    bool wasHeld = false; // Makes sure deactivate code is called once.

    // Update is called once per frame
    void Update()
    {
        if(pickupManager.HeldObject == gameObject) // Is Currently being Held
        {
            if (!wasHeld)
            {
                if (!dialougePlayed && OptionalSequence != null)
                {
                    dialougeManager.PlayDialouge(OptionalSequence);
                    dialougePlayed = true;
                }

                wasHeld = true;
                specialTextUI.ActivatePanel(SpecialText);
            }
        }
        else
        {
            if (wasHeld)
            {
                wasHeld = false; // Stop it from deactivating it on other objects.
                specialTextUI.DeactivatePanel();
            }
        }
    }
}
