using UnityEngine;

/// <summary>
/// Handles saving and loading high scores using PlayerPrefs.
/// Static class - no need to attach to GameObject.
/// </summary>
public static class HighScoreManager
{
    // Key used for PlayerPrefs storage
    private const string HIGH_SCORE_KEY = "HighScore";

    /// <summary>
    /// Saves score if it's higher than current high score
    /// </summary>
    /// <param name="score">The score to save</param>
    public static void SaveHighScore(int score)
    {
        int currentHighScore = GetHighScore();

        if (score > currentHighScore)
        {
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, score);
            PlayerPrefs.Save();
            Debug.Log($"New High Score: {score}! (Previous: {currentHighScore})");
        }
    }

    /// <summary>
    /// Returns the current high score
    /// </summary>
    public static int GetHighScore()
    {
        return PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
    }

    /// <summary>
    /// Resets the high score to 0
    /// </summary>
    public static void ResetHighScore()
    {
        PlayerPrefs.SetInt(HIGH_SCORE_KEY, 0);
        PlayerPrefs.Save();
        Debug.Log("High Score reset to 0.");
    }
}