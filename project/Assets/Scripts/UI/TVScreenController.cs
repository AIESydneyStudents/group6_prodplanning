using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TVScreenController : MonoBehaviour
{
    public MeshRenderer Renderer;
    public VideoPlayer Video;
    public Material OffMat;
    public Material OnMat;
    public Material VideoMat;

    public List<VideoClip> RareVideos;
    public float RareVideoChance = 0.05f;

    private bool TVon = false;
    private bool activeOnce = false;

    // Start is called before the first frame update
    void Start()
    {
        Video.enabled = false;
        Renderer.material = OffMat;
    }

    public void ToggleTV()
    {
        TVon = !TVon;

        if(TVon)
        {
            if (activeOnce && RareVideos.Count >= 0 && Random.Range(0f,100f) <= RareVideoChance)
            {
                Renderer.material = VideoMat;
                Video.enabled = true;
                Video.clip = RareVideos[Random.Range(0, RareVideos.Count)];
                Video.Play();
            }
            else
            {
                Renderer.material = OnMat;
            }
            activeOnce = true;
        }
        else
        {
            Renderer.material = OffMat;
            Video.enabled = false;
        }
    }
}
