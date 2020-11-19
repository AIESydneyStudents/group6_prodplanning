using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;

public class LoopManager : MonoBehaviour
{
    private struct ObjectRoomPair
    {
        public GameObject obj;
        public RoomManager kitchen;
        public PortalTeleporter portal;
        public LoopController cont;
        public PhoneDialouge Phone;

        public ObjectRoomPair(GameObject _obj,RoomManager _kitch)
        {
            obj = _obj;
            obj = _obj;
            kitchen = _kitch;
            portal = _obj.GetComponentInChildren<PortalTeleporter>();
            cont = obj.GetComponent<LoopController>();
            Phone = obj.GetComponentInChildren<PhoneDialouge>();
        }
    }

    private List<ObjectRoomPair> loops = new List<ObjectRoomPair>();

    private F_PlayerMovement player;
    private int currentLoop = 0;
    private bool init = false;

    private float saturationValue = 0.5f;
    private Keyframe saturationFrame = new Keyframe(0,0.5f);

    private ColorCurves saturationCurve;

    private void Start()
    {
        Object.FindObjectOfType<Volume>().profile.TryGet<ColorCurves>(out saturationCurve);
        saturationCurve.satVsSat.value.AddKey(0,0);
        StartCoroutine(LateStart());
    }

    // Start is called before the first frame update
    IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();

        //Get all Loops, and there kitchen mangers to make sure the next loop is loaded when needs to be.
        foreach(Transform child in transform)
        {
            loops.Add(new ObjectRoomPair(child.gameObject,FindKitchen(child.gameObject)));
        }

        //Disable all loops but the first.
        for(int i = 0; i < loops.Count; i++)
        {
            if(loops[i].portal != null)
            {
                loops[i].portal.CanTeleport = false;
            }

            if(i != 0) loops[i].obj.SetActive(false);
        }

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<F_PlayerMovement>();
        player.OnTeleport += Player_OnTeleport;
        init = true;
    }

    private void Player_OnTeleport(object sender, System.EventArgs e)
    {
        //Deactivate old loop.
        currentLoop++;
        loops[currentLoop - 1].obj.SetActive(false);
        LoopLoaded = false;

        //Activate the //current// loop(should be already, but just to be safe)
        if (currentLoop < loops.Count)
        {
            loops[currentLoop].obj.SetActive(true);
            saturationValue = loops[currentLoop].cont.SaturationLevel / 2.0f;
        }
    }

    void ActiveNext()
    {
        if (currentLoop+1 < loops.Count)
        {
            loops[currentLoop+1].obj.SetActive(true);
            loops[currentLoop + 1].portal.ContainingDoor.ToggleDoor(true);
            loops[currentLoop].portal.CanTeleport = true;
        }
    }

    //Checks if a loop needs to be loaded (Reset when teleported)
    bool LoopLoaded = false;
    bool PhoneDone = false;

    // Update is called once per frame
    void Update()
    {
        saturationFrame.value = Mathf.MoveTowards(saturationFrame.value,saturationValue,0.25f * Time.deltaTime);
        saturationCurve.satVsSat.value.MoveKey(0,saturationFrame);

        if (init)
        {
            if (!LoopLoaded)
            {
                //Checks if kitchen tasks are complete and then actiavtes the next loop
                if (loops[currentLoop].kitchen.Complete)
                {
                    //Play Phone call
                    LoopLoaded = true;
                    loops[currentLoop].Phone.ActivatePhone();
                    PhoneDone = false;
                }
            }
            else //Should be playing phone
            {
                //Check if phone is done;
                if (!PhoneDone && loops[currentLoop].Phone.PhoneDone())
                {
                    PhoneDone = true;
                    loops[currentLoop].portal.ContainingDoor.ToggleDoor(true);
                    LoopLoaded = true;
                    ActiveNext();
                }
            }
        }
    }
    
    RoomManager FindKitchen(GameObject obj)
    {
        //Find spacifically the kitchen loop, should probably be done with tags.
        var obs = obj.GetComponentsInChildren<RoomManager>();

        foreach(RoomManager man in obs)
        {
            if(man.gameObject.name == "KitchenTaskManger")
            {
                return man;
            }
        }

        return null;
    }
}
