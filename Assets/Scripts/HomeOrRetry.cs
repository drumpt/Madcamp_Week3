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
        Debug.Log("GoToStartMenu1");
        SceneManager.LoadScene("Scenes/StartMenu");
        Debug.Log("GoToStartMenu2");
    }

    public void GoToPlayGame()
    {
        Debug.Log("GoToPlayGame1");
        SceneManager.LoadScene("Scenes/PlayGame");
        Debug.Log("GoToPlayGame2");
    }
}