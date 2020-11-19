using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHintHighlighter : MonoBehaviour
{
    private struct HintRendererData
    {
        public GameObject Obj;
        public MeshRenderer Renderer;
        public Material OriginalMat;
        
        public HintRendererData(GameObject obj)
        {
            Obj = obj;
            Renderer = obj.GetComponent<MeshRenderer>();
            OriginalMat = Renderer.material;
        }

        public void ActivateGlow(Material glowMat)
        {
            glowMat.CopyPropertiesFromMaterial(OriginalMat);
            Renderer.material = glowMat;
        }

        public void DeactivateGlow()
        {
            Renderer.material = OriginalMat;
        }
    }

    public PickupObject pickuper;
    public Material glowMaterialToSwap;

    private Dictionary<string, List<HintRendererData>> hintObjects = new Dictionary<string, List<HintRendererData>>();
    private List<HintRendererData> glowingObjects = new List<HintRendererData>();

    private bool glowRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        SetupTag("Bin");
        SetupTag("Sink");
    }

    // Update is called once per frame
    void Update()
    {
        if(!glowRunning && pickuper.IsHoldingObject())
        {
            string tagToActivate = ParseKey(pickuper.HeldObject.tag);

            if(tagToActivate != "")
            {
                ActivateHintObjects(tagToActivate);
            }
        }
        else if(!pickuper.IsHoldingObject())
        {
            DeactivateGlowingObjects();
        }
    }

    void ActivateHintObjects(string Key)
    {
        glowRunning = true;
        if (hintObjects.ContainsKey(Key))
        {
            foreach(HintRendererData obj in hintObjects[Key])
            {
                obj.ActivateGlow(glowMaterialToSwap);
                glowingObjects.Add(obj);
            }
        }
    }

    void DeactivateGlowingObjects()
    {
        glowRunning = false;
        foreach(HintRendererData obj in glowingObjects)
        {
            obj.DeactivateGlow();
        }

        glowingObjects.Clear();
    }

    //Gets all gameobjects of tag adds them to the dictinary
    void SetupTag(string Tag)
    {
        List<HintRendererData> tempData = new List<HintRendererData>();
        GameObject[] foundObjects = GameObject.FindGameObjectsWithTag(Tag);

        for(int i = 0; i < foundObjects.Length; i++)
        {
            tempData.Add(new HintRendererData(foundObjects[i]));
        }

        hintObjects[Tag] = tempData;
    }

    string ParseKey(string HoldingTag)
    {
        switch(HoldingTag)
        {
            default: return "";
            case "Trash": return "Bin";
            case "Plate": return "Sink";
        }
    }
}
