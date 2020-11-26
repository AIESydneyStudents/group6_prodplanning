using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogoAnimation : MonoBehaviour
{
    public TextMeshProUGUI MeshPro;
    private float timer = 0;

    private int max = 0;
    private Color col;

    // Start is called before the first frame update
    void Start()
    {
        col = new Color(255, 255, 255, 0);
        MeshPro.color = col;
    }

    // Update is called once per frame
    void Update()
    {
        col.a = Mathf.MoveTowards(col.a,255,5 * Time.deltaTime);
        MeshPro.color = col;

        if (MeshPro.maxVisibleCharacters >= max)
        {

        }
    }
}
