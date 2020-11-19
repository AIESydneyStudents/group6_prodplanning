using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomSound : MonoBehaviour
{
    public AudioClip[] Clips;

    private AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void RandomPlay()
    {
        source.PlayOneShot(Clips[Random.Range(0,Clips.Length)]);
    }
}
