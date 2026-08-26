using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreController : MonoBehaviour
{
    private TextMeshProUGUI scoreText;

    private int score = 0;

    private void Awake()
    {
        scoreText = gameObject.GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        RefreshUI();
    }

    public void AddScore(int points)
    {
        score += points;
        RefreshUI();
    }

    private void RefreshUI()
    {
        scoreText.text = "Score: " + score;
    }
}