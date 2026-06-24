using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles the Main Menu screen.
/// Contains Play and Quit buttons.
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI titleText;

    private void Start()
    {
        // Setup button listeners
        if (playButton != null)
        {
            playButton.onClick.AddListener(OnPlayClicked);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitClicked);
        }

        // Display high score
        UpdateHighScore();

        // Play background music
        AudioManager.Instance?.PlayMusic();
    }

    /// <summary>
    /// Called when Play button is clicked
    /// </summary>
    private void OnPlayClicked()
    {
        AudioManager.Instance?.PlaySFX("ButtonClick");
        SceneManager.LoadScene("Gameplay");
    }

    /// <summary>
    /// Called when Quit button is clicked
    /// </summary>
    private void OnQuitClicked()
    {
        AudioManager.Instance?.PlaySFX("ButtonClick");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    /// <summary>
    /// Shows the current high score
    /// </summary>
    private void UpdateHighScore()
    {
        if (highScoreText != null)
        {
            int highScore = HighScoreManager.GetHighScore();
            highScoreText.text = $"Best: {highScore}";
        }
    }
}