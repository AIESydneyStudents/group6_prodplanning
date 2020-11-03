using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialougeManager : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public AudioSource AudioPlayer;

    public DialougeSequence testSequence;

    private DialougeSequence playingSequence;
    private int dialougeIndex = 0;

    private bool lineHasPlayed = false;
    private bool active = false;

    public void PlayDialouge(DialougeSequence ToPlay)
    {
        dialougeIndex = 0;
        lineHasPlayed = false;
        playingSequence = ToPlay;
        active = true;
        Text.enabled = true;
    }

    private void Start()
    {
        PlayDialouge(testSequence);
    }

    private float lineTimer = 0;
    private float lineMaxTimer = 0;
    private float charIncereaseRate = 0;

    private void Update()
    {
        if(active)
        {
            if(!lineHasPlayed)
            {
                //Set dialouge text
                Text.text = playingSequence.textStrings[dialougeIndex].DialougeText;

                Text.maxVisibleCharacters = 0;
                lineTimer = 0;

                //Play Dialouge line.
                if (playingSequence.textStrings[dialougeIndex].SpokenLine)
                {
                    AudioPlayer.PlayOneShot(playingSequence.textStrings[dialougeIndex].SpokenLine);
                    lineMaxTimer = playingSequence.textStrings[dialougeIndex].SpokenLine.length;
                }

                lineMaxTimer += playingSequence.textStrings[dialougeIndex].EndWaitTime;
                charIncereaseRate = Mathf.Ceil(Text.textInfo.characterCount / (lineTimer));

                lineHasPlayed = true;
            }
            else //Check for audio to finish
            {
                lineTimer += Time.unscaledDeltaTime; // Unscaled because we don't want long seconds.
                Text.maxVisibleCharacters = Mathf.CeilToInt(((lineTimer) / (lineMaxTimer * 0.5f)) * Text.textInfo.characterCount);

                if (lineTimer >= lineMaxTimer) // Lined finish speaking
                {
                    //Go to next line
                    dialougeIndex++;
                    lineHasPlayed = false;

                    if (dialougeIndex >= playingSequence.textStrings.Count)
                    {
                        Text.enabled = false;
                        active = false;
                    }
                }
            }
        }
    }
}
