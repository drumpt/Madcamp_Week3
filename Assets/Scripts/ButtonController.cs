using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    private SpriteRenderer theSR;
    public Sprite defaultImage;
    public Sprite pressedImage;
    public KeyCode keyToPress;
    // public bool virtualPress;

    // Start is called before the first frame update
    void Start()
    {
        theSR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(InputBroker.GetKeyDown(keyToPress))
        {
            // pressed = true;
            theSR.sprite = pressedImage;
        }

        if(InputBroker.GetKeyUp(keyToPress))
        {
            // pressed = false;
            theSR.sprite = defaultImage;
        }
        // if(pressed)
        // {
        //     theSR.sprite = pressedImage;
        // }
        // else
        // {
        //     theSR.sprite = defaultImage;
        // }
    }
}