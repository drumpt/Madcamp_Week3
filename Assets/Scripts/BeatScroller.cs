using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatScroller : MonoBehaviour
{
    // Start is called before the first frame update

    public float beatTempo;

    public bool hasStarted;
 
    
    void Start()
    {
        beatTempo = beatTempo / 60f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasStarted)
        {
            // if (Input.anyKeyDown)
            // {
            //     hasStarted = true;
            // }
            // Do nothing
        }
        else
        {
            transform.position -= new Vector3(0f, beatTempo * Time.deltaTime, 0f);
        }
    }

    public void StopScroll()
    {
        hasStarted = false;
    }

    public void PlayScroll()
    {
        hasStarted = true;
    }
}
