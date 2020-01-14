using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitToMenu : MonoBehaviour
{
    private RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ExitActive()
    {
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 33f);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 33f);

    }

    public void ExitInactive()
    {
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
    }

    public void ChangeSceneMenu()
    {
        SceneManager.LoadScene("Scenes/StartMenu");
    }
}
