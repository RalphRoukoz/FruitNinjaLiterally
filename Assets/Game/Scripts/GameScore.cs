using UnityEngine;
using TMPro;

public class GameScore : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // Assign in Inspector
    public TextMeshProUGUI endGameScore; // Assign in Inspector
    public string bestScoreKey = "BestScore";

    private int score = 0;
    private float scoreTimer = 0f;
    private bool isRunning = false;

    void Update()
    {
        if (isRunning)
        {
            scoreTimer += Time.deltaTime;
            if (scoreTimer >= 1f)
            {
                AddScore(1);
                scoreTimer = 0f;
            }
        }
    }

    public void StartScoring()
    {
        score = 0;
        scoreTimer = 0f;
        isRunning = true;
        UpdateScoreDisplay();
    }

    public void StopScoring()
    {
        isRunning = false;
        SaveBestScore();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    private void SaveBestScore()
    {
        int bestScore = PlayerPrefs.GetInt(bestScoreKey, 0);
        if (score > bestScore)
        {
            PlayerPrefs.SetInt(bestScoreKey, score);
            PlayerPrefs.Save();
            Debug.Log("New Best Score: " + score);
            endGameScore.text = $"NEW HIGH SCORE:\n{score}";
        }
        else
        {
            endGameScore.text = $"High Score:\n{bestScore}\n\nScore:\n{score}";
            Debug.Log("Score: " + score + " | Best: " + bestScore);
        }
    }

    public int GetBestScore()
    {
        return PlayerPrefs.GetInt(bestScoreKey, 0);
    }

    public string GetBestScoreFormatted()
    {
        int best = GetBestScore();
        return best == 0 ? "N/A" : best.ToString();
    }
}