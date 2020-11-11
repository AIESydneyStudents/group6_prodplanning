using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialougeManager : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public AudioSource AudioPlayer;

    public bool IsPlayingDialouge { get { return active; } }

    private Queue<List<DialougeLine>> playingSequence = new Queue<List<DialougeLine>>();
    private int dialougeIndex = 0;

    private bool lineHasPlayed = false;
    private bool active = false;

    //Incase you need to play dialouge without a scriptable object
    public void PlayDialouge(List<DialougeLine> ToPlay)
    {
        lineHasPlayed = false;
        playingSequence.Enqueue(ToPlay);

        if (!active)
        {
            dialougeIndex = 0;
            active = true;
            Text.enabled = true;
        }
    }

    public void PlayDialouge(DialougeSequence ToPlay)
    {
        lineHasPlayed = false;
        playingSequence.Enqueue(ToPlay.textStrings);

        if (!active)
        {
            dialougeIndex = 0;
            active = true;
            Text.enabled = true;
        }
    }

    public void PlayDialougeIfNotPlaying(List<DialougeLine> ToPlay)
    {
        if(!active)
        {
            PlayDialouge(ToPlay);
        }
    }

    public void PlayDialougeIfNotPlaying(DialougeSequence ToPlay)
    {
        if (!active)
        {
            PlayDialouge(ToPlay);
        }
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
                Text.text = playingSequence.Peek()[dialougeIndex].DialougeText;

                Text.alpha = 0;
                Text.maxVisibleCharacters = 0;
                lineTimer = 0;

                //Play Dialouge line.
                if (playingSequence.Peek()[dialougeIndex].SpokenLine)
                {
                    AudioPlayer.PlayOneShot(playingSequence.Peek()[dialougeIndex].SpokenLine);
                    lineMaxTimer = playingSequence.Peek()[dialougeIndex].SpokenLine.length;
                }

                lineMaxTimer += playingSequence.Peek()[dialougeIndex].EndWaitTime;
                charIncereaseRate = Mathf.Ceil(Text.textInfo.characterCount / (lineTimer));

                lineHasPlayed = true;
            }
            else //Check for audio to finish
            {
                lineTimer += Time.unscaledDeltaTime; // Unscaled because we don't want long seconds.
                Text.maxVisibleCharacters = Mathf.CeilToInt(((lineTimer) / (lineMaxTimer * 0.5f)) * Text.textInfo.characterCount);

                //Fade text if started or done
                float perc = (lineTimer) / (lineMaxTimer);
                FadeText(perc);

                if(perc > 0.95f)
                {
                    Text.alpha = 1 - (perc - 0.95f) * 20;
                }

                if (lineTimer >= lineMaxTimer) // Lined finish speaking
                {
                    //Go to next line
                    dialougeIndex++;
                    lineHasPlayed = false;

                    if (dialougeIndex >= playingSequence.Peek().Count)
                    {
                        dialougeIndex = 0;
                        playingSequence.Dequeue();
                        if (playingSequence.Count <= 0)
                        {
                            Text.enabled = false;
                            active = false;
                        }
                    }
                }
            }
        }
    }

    private void FadeText(float percentDone)
    {
        if (percentDone >= 0.95f)
        {
            Text.alpha = 1 - (percentDone - 0.95f) * 20;
        }
        else if (percentDone <= 0.05f)
        {
            Text.alpha = (percentDone) * 20;
        }

        Text.alpha = Mathf.Clamp(Text.alpha,0f,1f);
    }
}
