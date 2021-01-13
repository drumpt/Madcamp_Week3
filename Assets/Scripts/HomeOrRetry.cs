using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeOrRetry : MonoBehaviour
{
    private Image image;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToStartMenu()
    {
        SceneManager.LoadScene("Scenes/StartMenu");
    }

    public void GoToPlayGame()
    {
        SceneManager.LoadScene("Scenes/PlayGame");
    }
}