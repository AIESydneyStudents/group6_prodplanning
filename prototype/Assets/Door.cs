using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    bool Open = false;
    public float TargetOpenRotationY;

    private float initRotationY;
    private DialougeManager dialougeManager;
    private List<DialougeLine> cannotProgress = new List<DialougeLine>();
    private AudioSource audioSource;

    public DialougeSequence[] FailedToOpenSequence;

#if UNITY_EDITOR // So we can skip tasks >:>
    private Collider col;
#endif

    private void Start()
    {
        dialougeManager = GameObject.FindGameObjectWithTag("DialougeSystem").GetComponent<DialougeManager>();
        initRotationY = transform.rotation.eulerAngles.y;
        audioSource = GetComponent<AudioSource>();

        cannotProgress.Add(new DialougeLine("I still have things to do in here.",3f));

#if UNITY_EDITOR // So we can skip tasks >:>
            col = GetComponent<Collider>();
        #endif
    }

    public void ToggleDoor()
    {
        Open = !Open;
        if (Open) audioSource.Play();

        Moving = true;
    }

    public void ToggleDoor(bool open)
    {
        Open = open;
        if (Open) audioSource.Play();

        Moving = true;
    }

    private bool Moving = false;

    public void Update()
    {
        #if UNITY_EDITOR // So we can skip tasks >:>
            if(Input.GetKey(KeyCode.LeftControl))
            {
                col.isTrigger = true;
            }
            else
            {
                col.isTrigger = false;
            }
        #endif

        //Rotate the door if need to
        if (Moving)
        {
            float targetDir = Open ? TargetOpenRotationY : initRotationY;
            transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler( new Vector3(0, targetDir, 0)),5f * Time.deltaTime);
            if(transform.rotation == Quaternion.Euler(new Vector3(0, targetDir, 0)))
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, targetDir, 0));
                Moving = false;
            }

        }
    }

    public void Interact()
    {
        if(!Open)
        {
            if (FailedToOpenSequence == null)
            {
                dialougeManager.PlayDialougeIfNotPlaying(cannotProgress);
            }
            else
            {
                dialougeManager.PlayDialougeIfNotPlaying(FailedToOpenSequence[Random.Range(0,FailedToOpenSequence.Length)]);
            }
        }
    }
}
