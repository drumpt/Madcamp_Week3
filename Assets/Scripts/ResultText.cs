using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultText : MonoBehaviour
{
    public TextMeshPro perfectHits;
    public TextMeshPro missedHits;
    public TextMeshPro percentageHits;
    public TextMeshPro totalScore;


    // Start is called before the first frame update
    void Start()
    {
        perfectHits.text = GameManager.perfectHits.ToString();
        missedHits.text = GameManager.missedHits.ToString();
        percentageHits.text = (GameManager.perfectHits / GameManager.totalNotes * 100f).ToString("F1") + "%";
        totalScore.text = GameManager.currentScore.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}