using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    public KeyCode keyToPress;
    public GameObject perfectEffect, missEffect;

    private bool canBePressed;
    private bool alreadyPressed;

    // Start is called before the first frame update
    void Start()
    {
        canBePressed = false;
        alreadyPressed = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Activator")
        {
            canBePressed = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(canBePressed && InputBroker.GetKey(keyToPress))
        {
            alreadyPressed = true;
            gameObject.SetActive(false);
            GameManager.instance.NoteHit();
            Instantiate(perfectEffect, transform.position, perfectEffect.transform.rotation);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Activator" && !alreadyPressed)
        {
            canBePressed = false;
            GameManager.instance.NoteMissed();
            Instantiate(missEffect, transform.position, missEffect.transform.rotation);
        }
    }
}