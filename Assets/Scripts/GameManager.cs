using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class GameManager : MonoBehaviour
{
    public AudioSource theMusic;

    public static GameManager instance;

    public static int currentScore;

    public int currentMultiplier;
    public int multiplierTracker;
    public int[] multiplierThresholds;

    public TextMeshPro scoreText;
    public TextMeshPro multiText;

    public static float totalNotes;
    public static float perfectHits;
    public static float missedHits;

    public Stopwatch sw = new Stopwatch();

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        currentScore = 0;
        currentMultiplier = 1;
        multiplierTracker = 0;

        scoreText.text = "Score: " + currentScore;
        multiText.text = "Multiplier: x" + currentMultiplier;

        totalNotes = FindObjectsOfType<NoteObject>().Length;
        perfectHits = 0;
        missedHits = 0;

        multiplierThresholds = new int[] {4, 8, 16};

        StartCoroutine(WaitForMusicEnd(theMusic));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NoteHit()
    {
        if(currentMultiplier - 1 < multiplierThresholds.Length)
        {
            multiplierTracker++;
            if(multiplierTracker >= multiplierThresholds[currentMultiplier - 1])
            {
                multiplierTracker = 0;
                currentMultiplier++;
            }
        }
        currentScore += Random.Range(95, 106) * currentMultiplier;
        scoreText.text = "Score: " + currentScore;
        multiText.text = "Multiplier: x" + currentMultiplier;
        perfectHits++;
    }

    public void NoteMissed()
    {
        currentMultiplier = 1;
        multiplierTracker = 0;
        multiText.text = "Multiplier: x" + currentMultiplier;
        missedHits++;
    }

    public IEnumerator WaitForMusicEnd(AudioSource theMusic)
    {
        while(sw.ElapsedMilliseconds / 1000f - theMusic.clip.length <= 3f)
        {
            yield return 0;
        }
        SceneManager.LoadScene("Scenes/Results");
        Debug.Log("game is finished");
        yield break;
    }
}