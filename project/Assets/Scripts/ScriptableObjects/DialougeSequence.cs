using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialougeLine
{
    public string DialougeText;
    public AudioClip SpokenLine;
    public float EndWaitTime;

    public DialougeLine()
    {
        DialougeText = "Empty";
        SpokenLine = null;
        EndWaitTime = 0;
    }

    public DialougeLine(string text)
    {
        DialougeText = text;
        SpokenLine = null;
        EndWaitTime = 5;
    }

    public DialougeLine(string text,float time)
    {
        DialougeText = text;
        SpokenLine = null;
        EndWaitTime = time;
    }
}

[CreateAssetMenu(fileName = "NewDialougeSequence", menuName = "Dialouge/Sequence")]
public class DialougeSequence : ScriptableObject
{
    [SerializeField]
    public List<DialougeLine> textStrings;
}
