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
}

[CreateAssetMenu(fileName = "NewDialougeSequence", menuName = "Dialouge/Sequence")]
public class DialougeSequence : ScriptableObject
{
    [SerializeField]
    public List<DialougeLine> textStrings;
}
