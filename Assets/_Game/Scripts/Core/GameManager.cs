using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// GameManager is the central controller of the game.
/// Handles:
/// - Game state (Playing, Won, Lost)
/// - Score tracking
/// - Card play validation
/// - Win/Lose condition checking
/// - Game flow (start, restart, quit)
/// </summary>
public class GameManager : MonoBehaviour
{
    // ----- Singleton -----
    public static GameManager Instance { get; private set; }

    // ----- Game State Enum -----
    public enum GameState
    {
        Playing,
        Won,
        Lost,
        Paused
    }

    // ----- Constants -----
    private const int HAND_SIZE = 5;          // Player always has 5 cards
    private const int POINTS_PER_MOVE = 10;   // +10 per valid move

    // ----- Game Variables -----
    private Card centerCard;                   // Current center pile card
    private List<Card> playerHand;             // Player's hand (5 cards)
    private int score;                         // Current score
    private GameState currentState;            // Current game state

    // ----- Public Properties -----
    public Card CenterCard => centerCard;
    public List<Card> PlayerHand => playerHand;
    public int Score => score;
    public GameState CurrentState => currentState;

    // ----- Events -----
    // Events allow other scripts (like UIManager) to react to changes
    public System.Action OnGameStarted;
    public System.Action OnCardPlayed;
    public System.Action OnInvalidMove;
    public System.Action OnHandUpdated;
    public System.Action OnScoreUpdated;
    public System.Action OnNoValidMoves;  // NEW EVENT
    public System.Action<GameState> OnGameStateChanged;

    // ----- Unity Lifecycle -----
    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        StartNewGame();
    }

    // ----- Public Methods -----

    /// <summary>
    /// Initializes a brand new game.
    /// Creates deck, shuffles, distributes cards.
    /// </summary>
    public void StartNewGame()
    {
        // Reset score
        score = 0;

        // Set state
        currentState = GameState.Playing;

        // Initialize hand
        playerHand = new List<Card>();

        // Create and shuffle deck
        DeckManager.Instance.ResetDeck();

        // Draw 1 card for center pile
        centerCard = DeckManager.Instance.DrawCard();
        Debug.Log($"Center Card: {centerCard}");

        // Draw 5 cards for player hand
        playerHand = DeckManager.Instance.DrawCards(HAND_SIZE);
        Debug.Log($"Player Hand: {string.Join(", ", playerHand)}");

        // Notify listeners
        OnGameStarted?.Invoke();
        OnScoreUpdated?.Invoke();
        OnHandUpdated?.Invoke();

        Debug.Log("--- NEW GAME STARTED ---");

        // Check if starting hand has any valid moves
        CheckGameEndConditions();
    }

    /// <summary>
    /// Attempts to play a card from the player's hand.
    /// This is the MAIN GAME LOGIC method.
    /// </summary>
    /// <param name="handIndex">Index of card in player's hand (0-4)</param>
    public void TryPlayCard(int handIndex)
    {
        // Safety checks
        if (currentState != GameState.Playing)
        {
            Debug.LogWarning("Game is not in Playing state!");
            return;
        }

        if (handIndex < 0 || handIndex >= playerHand.Count)
        {
            Debug.LogError($"Invalid hand index: {handIndex}");
            return;
        }

        // Get the selected card
        Card selectedCard = playerHand[handIndex];

        // Check if the card can be played (CORE RULE)
        if (selectedCard.CanPlayOn(centerCard))
        {
            // ===== VALID MOVE =====
            Debug.Log($"Valid play: {selectedCard} on {centerCard}");

            // 1. Replace center card
            centerCard = selectedCard;

            // 2. Remove card from hand
            playerHand.RemoveAt(handIndex);

            // 3. Draw new card (if deck is not empty)
            if (!DeckManager.Instance.IsDeckEmpty)
            {
                Card newCard = DeckManager.Instance.DrawCard();
                if (newCard != null)
                {
                    playerHand.Add(newCard);
                }
            }

            // 4. Increase score
            score += POINTS_PER_MOVE;

            // Notify listeners
            OnCardPlayed?.Invoke();
            OnScoreUpdated?.Invoke();
            OnHandUpdated?.Invoke();

            // 5. Check win/lose conditions
            CheckGameEndConditions();
        }
        else
        {
            // ===== INVALID MOVE =====
            Debug.Log($"Invalid play: {selectedCard} cannot be played on {centerCard}");

            // Notify listeners (UI will show error message)
            OnInvalidMove?.Invoke();
        }
    }

    /// <summary>
    /// Checks if the game has been won or lost.
    /// Called after every valid card play.
    /// </summary>
    private void CheckGameEndConditions()
    {
        // WIN: Deck empty AND Hand empty
        if (DeckManager.Instance.IsDeckEmpty && playerHand.Count == 0)
        {
            currentState = GameState.Won;
            Debug.Log($"YOU WIN! Final Score: {score}");

            // Save high score
            HighScoreManager.SaveHighScore(score);

            OnGameStateChanged?.Invoke(GameState.Won);
            return;
        }

        // LOSE: No valid moves in hand
        // (Whether deck is empty or not, if player can't play, game is over)
        if (!HasValidMoves())
        {
            currentState = GameState.Lost;
            Debug.Log($"GAME OVER! No valid moves. Final Score: {score}");

            // Save high score
            HighScoreManager.SaveHighScore(score);

            // START COROUTINE TO SHOW SMOOTH TRANSITION
            StartCoroutine(ShowGameOverWithDelay());
            return;
        }

        /*
        // LOSE: Deck empty AND no valid moves in hand
        if (DeckManager.Instance.IsDeckEmpty && !HasValidMoves())
        {
            currentState = GameState.Lost;
            Debug.Log($"GAME OVER! Final Score: {score}");

            // Save high score
            HighScoreManager.SaveHighScore(score);

            OnGameStateChanged?.Invoke(GameState.Lost);
            return;
        }
        */
    }

    /// <summary>
    /// Shows "No Valid Moves" message, then triggers game over screen
    /// </summary>
    private IEnumerator ShowGameOverWithDelay()
    {
        // First, notify UI to show "No Valid Moves" message
        OnNoValidMoves?.Invoke();

        // Wait for player to see the message
        yield return new WaitForSeconds(1.8f);

        // Then show Game Over screen
        OnGameStateChanged?.Invoke(GameState.Lost);
    }

    /// <summary>
    /// Checks if the player has at least one valid move.
    /// Used for lose condition detection.
    /// </summary>
    /// <returns>True if at least one card can be played</returns>
    private bool HasValidMoves()
    {
        foreach (Card card in playerHand)
        {
            if (card.CanPlayOn(centerCard))
            {
                return true; // At least one valid move exists
            }
        }
        return false; // No valid moves
    }

    // ----- Navigation Methods -----

    /// <summary>
    /// Restarts the current game
    /// </summary>
    public void RestartGame()
    {
        Debug.Log("Restarting game...");
        StartNewGame();
    }

    /// <summary>
    /// Goes back to Main Menu scene
    /// </summary>
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Quits the application
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}