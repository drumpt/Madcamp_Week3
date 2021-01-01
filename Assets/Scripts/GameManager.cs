using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public AudioSource theMusic;

    public static GameManager instance;

    public int currentScore;
    public int scorePerNote = 100;

    public TextMeshPro scoreText;
    public TextMeshPro multiText;

    public int currentMultiplier;
    public int multiplierTracker;
    public int[] multiplierThresholds;

    public float totalNotes;
    public float perfectHits;
    public float missedHits;
   
    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        multiplierThresholds = new int[] {4, 8, 16};
        currentScore = 0;
        currentMultiplier = 1;
        multiplierTracker = 0;
        scoreText.text = "Score: " + currentScore;
        multiText.text = "Multiplier: x" + currentMultiplier;

        totalNotes = FindObjectsOfType<NoteObject>().Length;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NoteHit()
    {
        Debug.Log("Hit On Time");

        // Debug.Log(multiplierThresholds.Length);
        Debug.Log("currentMultiplier");
        Debug.Log(currentMultiplier);
        Debug.Log("multiplierTracker");
        Debug.Log(multiplierTracker);
        Debug.Log("multiplierThresholds[currentMultiplier - 1]");
        Debug.Log(multiplierThresholds[currentMultiplier - 1]);

        if(currentMultiplier - 1 < multiplierThresholds.Length)
        {
            Debug.Log("Reached");
            Debug.Log("multiplierTracker");
            Debug.Log(multiplierTracker);
            multiplierTracker++;
            Debug.Log("multiplierTracker");
            Debug.Log(multiplierTracker);

            print(multiplierThresholds[currentMultiplier - 1]);
            print(multiplierTracker >= multiplierThresholds[currentMultiplier - 1]);

            if(multiplierTracker >= multiplierThresholds[currentMultiplier - 1])
            {
                Debug.Log("Reached 2");
                Debug.Log("multiplierTracker");
                multiplierTracker = 0;
                currentMultiplier++;
            }
        }

        Debug.Log("multiplierTracker");
        Debug.Log(multiplierTracker);

        currentScore += scorePerNote * currentMultiplier;
        scoreText.text = "Score: " + currentScore;
        multiText.text = "Multiplier: x" + currentMultiplier;

        perfectHits++;
    }

    public void NoteMissed()
    {
        Debug.Log("Missed Note");
        Debug.Log("multiplierTracker");
        Debug.Log(multiplierTracker);

        currentMultiplier = 1;
        multiplierTracker = 0;

        multiText.text = "Multiplier: x" + currentMultiplier;

        missedHits++;

        Debug.Log("Missed!!!");
        Debug.Log(missedHits);

    }

}