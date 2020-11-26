using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomHint : MonoBehaviour
{
    [TextArea(5,10)]
    public string Hint;

    private TextMeshProUGUI tmp;

    private void Start()
    {
        tmp = GameObject.FindGameObjectWithTag("PauseHint").GetComponent<TextMeshProUGUI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(tmp == null)
            {
                Debug.LogWarning("No Pause hint object.");
            }
            else
            {
                tmp.text = Hint;
                enabled = false;
            }
        }
    }
}
