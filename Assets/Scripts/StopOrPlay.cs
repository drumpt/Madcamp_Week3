using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StopOrPlay : MonoBehaviour
{

    public Image image;
    public Sprite stop;
    public Sprite play;

    public PhoneCamera phoneCamera;
    public BeatScroller beatScroller;

    public ExitToMenu exitToMenu;

    public GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeButton()
    {
        if(image.sprite.name == stop.name)
        {
            //change button Image
            image.sprite = play;
            //Stop scene
            phoneCamera.PauseFrontCam();
            beatScroller.StopScroll();
            exitToMenu.ExitActive();
            gameManager.theMusic.Pause();

        }
        else if(image.sprite.name == play.name)
        {
            //change button Image
            image.sprite = stop;
            //play scene
            phoneCamera.PlayFrontCam();
            beatScroller.PlayScroll();
            exitToMenu.ExitInactive();
            gameManager.theMusic.Play();

        }
    }
}
