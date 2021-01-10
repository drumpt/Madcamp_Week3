using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartToPlay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("menu is opened");

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeScenePlay()
    {
        SceneManager.LoadScene("Scenes/PlayGame");
        Debug.Log("startbutton is clicked");
    }
}