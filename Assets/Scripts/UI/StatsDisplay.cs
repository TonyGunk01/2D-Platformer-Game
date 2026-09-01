using UnityEngine;
using TMPro;

// Displays stats and updates automatically when SaveStats is called
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
        // initialize UI from stored values
        StatsManager.DisplayLastStats(timerText, scoreText);
    }

    public void Refresh() => StatsManager.DisplayLastStats(timerText, scoreText);

    private void OnStatsSavedHandler(int coins, float time)
    {
        if (timerText != null)
            timerText.text = StatsManager.FormatTime(time);
        if (scoreText != null)
            scoreText.text = "Score: " + coins;
    }
}
