using UnityEngine;
using TMPro;

public class StatsDisplay : MonoBehaviour
{
    public TMP_Text timerText;
    public TMP_Text scoreText;

    void OnEnable()
    {
        StatsManager.OnStatsSaved += OnStatsSavedHandler;
    }

    void OnDisable()
    {
        StatsManager.OnStatsSaved -= OnStatsSavedHandler;
    }

    void Start()
    {
        StatsManager.DisplayLastStats(timerText, scoreText);
    }

    public void Refresh() => StatsManager.DisplayLastStats(timerText, scoreText);

    private void OnStatsSavedHandler(int coins, float time)
    {
        if (timerText != null)
            timerText.text = "Time: " + StatsManager.FormatTime(time);

        if (scoreText != null)
            scoreText.text = "Score: " + coins;
    }
}