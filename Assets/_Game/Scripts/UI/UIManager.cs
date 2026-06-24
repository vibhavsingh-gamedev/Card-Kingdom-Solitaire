using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UIManager handles ALL UI updates in the game.
/// Responsible for:
/// - Updating score, deck count displays
/// - Managing screen panels (Gameplay, Win, Lose)
/// - Creating and arranging card UI elements
/// - Showing feedback messages
/// </summary>
public class UIManager : MonoBehaviour
{
    // ----- Singleton -----
    public static UIManager Instance { get; private set; }

    // ----- Panel References -----
    [Header("Screen Panels")]
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    // ----- Gameplay UI References -----
    [Header("Gameplay UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI deckCountText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private Transform centerCardSlot;        // Where center card sits
    [SerializeField] private Transform playerHandContainer;   // Parent for hand cards
    [SerializeField] private TextMeshProUGUI invalidMoveText; // "Invalid Move" message
    [SerializeField] private TextMeshProUGUI ruleReminderText; // "Same Suit or Higher Rank"

    // ----- Win/Lose UI References -----
    [Header("End Screen UI")]
    [SerializeField] private TextMeshProUGUI winScoreText;
    [SerializeField] private TextMeshProUGUI loseScoreText;

    // ----- Prefab Reference -----
    [Header("Prefabs")]
    [SerializeField] private GameObject cardPrefab;

    // ----- Deck Visual -----
    [Header("Deck Visual")]
    [SerializeField] private GameObject deckVisual;            // Card back image for deck
    [SerializeField] private TextMeshProUGUI deckVisualCount;  // "Deck: 36" text on deck

    // ----- Private Variables -----
    private List<GameObject> handCardObjects = new List<GameObject>();
    private GameObject centerCardObject;
    private Coroutine invalidMoveCoroutine;

    // ----- Unity Lifecycle -----
    private void Awake()
    {
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

    private void OnEnable()
    {
        // Subscribe to GameManager events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStarted += OnGameStarted;
            GameManager.Instance.OnCardPlayed += OnCardPlayed;
            GameManager.Instance.OnInvalidMove += OnInvalidMove;
            GameManager.Instance.OnHandUpdated += UpdateHandDisplay;
            GameManager.Instance.OnScoreUpdated += UpdateScoreDisplay;
            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
        }
    }

    private void Start()
    {
        // Initial setup
        HideAllPanels();
        ShowPanel(gameplayPanel);
        HideInvalidMoveText();

        // Subscribe if not already (for first frame timing)
        SubscribeToEvents();

        // Initial display
        RefreshAllUI();

        // Start background music for gameplay
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic();
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStarted -= OnGameStarted;
            GameManager.Instance.OnCardPlayed -= OnCardPlayed;
            GameManager.Instance.OnInvalidMove -= OnInvalidMove;
            GameManager.Instance.OnHandUpdated -= UpdateHandDisplay;
            GameManager.Instance.OnScoreUpdated -= UpdateScoreDisplay;
            GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
        }
    }

    // ----- Event Subscription Helper -----
    private void SubscribeToEvents()
    {
        if (GameManager.Instance != null)
        {
            // Remove first to avoid double subscription
            GameManager.Instance.OnGameStarted -= OnGameStarted;
            GameManager.Instance.OnCardPlayed -= OnCardPlayed;
            GameManager.Instance.OnInvalidMove -= OnInvalidMove;
            GameManager.Instance.OnHandUpdated -= UpdateHandDisplay;
            GameManager.Instance.OnScoreUpdated -= UpdateScoreDisplay;
            GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
            GameManager.Instance.OnNoValidMoves -= OnNoValidMovesMessage;  

            // Then add
            GameManager.Instance.OnGameStarted += OnGameStarted;
            GameManager.Instance.OnCardPlayed += OnCardPlayed;
            GameManager.Instance.OnInvalidMove += OnInvalidMove;
            GameManager.Instance.OnHandUpdated += UpdateHandDisplay;
            GameManager.Instance.OnScoreUpdated += UpdateScoreDisplay;
            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
            GameManager.Instance.OnNoValidMoves += OnNoValidMovesMessage;  
        }
    }

    // ----- Event Handlers -----

    private void OnGameStarted()
    {
        HideAllPanels();
        ShowPanel(gameplayPanel);
        RefreshAllUI();
    }

    private void OnCardPlayed()
    {
        UpdateCenterCard();
        UpdateHandDisplay();
        UpdateScoreDisplay();
        UpdateDeckCount();

        // Play card place sound
        AudioManager.Instance?.PlaySFX("CardPlace");
    }

    private void OnInvalidMove()
    {
        ShowInvalidMoveMessage();

        // Play error sound
        AudioManager.Instance?.PlaySFX("InvalidMove");
    }

    private void OnGameStateChanged(GameManager.GameState state)
    {
        switch (state)
        {
            case GameManager.GameState.Won:
                ShowWinScreen();
                AudioManager.Instance?.PlaySFX("Win");
                break;

            case GameManager.GameState.Lost:
                ShowLoseScreen();
                AudioManager.Instance?.PlaySFX("Lose");
                break;
        }
    }

    /// <summary>
    /// Shows "No Valid Moves" message before game over screen
    /// </summary>
    private void OnNoValidMovesMessage()
    {
        if (invalidMoveCoroutine != null)
        {
            StopCoroutine(invalidMoveCoroutine);
        }
        invalidMoveCoroutine = StartCoroutine(ShowNoValidMovesRoutine());
    }

    private IEnumerator ShowNoValidMovesRoutine()
    {
        if (invalidMoveText != null)
        {
            invalidMoveText.gameObject.SetActive(true);
            invalidMoveText.text = "NO VALID MOVES!";
            invalidMoveText.color = new Color(1f, 0.3f, 0.3f); // Bright red

            // Scale animation
            Vector3 originalScale = invalidMoveText.transform.localScale;
            invalidMoveText.transform.localScale = Vector3.zero;

            float elapsed = 0f;
            float duration = 0.3f;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                invalidMoveText.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale * 1.2f, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            invalidMoveText.transform.localScale = originalScale;

            yield return new WaitForSeconds(1.2f);

            invalidMoveText.gameObject.SetActive(false);
            invalidMoveText.transform.localScale = originalScale;
        }
    }

    // ----- UI Update Methods -----

    /// <summary>
    /// Refreshes all UI elements. Called on game start.
    /// </summary>
    public void RefreshAllUI()
    {
        UpdateScoreDisplay();
        UpdateDeckCount();
        UpdateBestScore();
        UpdateCenterCard();
        UpdateHandDisplay();
    }

    /// <summary>
    /// Updates the score text display
    /// </summary>
    public void UpdateScoreDisplay()
    {
        if (scoreText != null && GameManager.Instance != null)
        {
            scoreText.text = GameManager.Instance.Score.ToString();
        }
    }

    /// <summary>
    /// Updates the deck remaining count
    /// </summary>
    public void UpdateDeckCount()
    {
        if (deckCountText != null && DeckManager.Instance != null)
        {
            deckCountText.text = DeckManager.Instance.CardsRemaining.ToString();
        }

        if (deckVisualCount != null && DeckManager.Instance != null)
        {
            deckVisualCount.text = $"DECK: {DeckManager.Instance.CardsRemaining}";
        }

        // Hide deck visual if empty
        if (deckVisual != null && DeckManager.Instance != null)
        {
            deckVisual.SetActive(!DeckManager.Instance.IsDeckEmpty);
        }
    }

    /// <summary>
    /// Updates the best score display
    /// </summary>
    public void UpdateBestScore()
    {
        if (bestScoreText != null)
        {
            bestScoreText.text = HighScoreManager.GetHighScore().ToString();
        }
    }

    /// <summary>
    /// Updates the center card visual
    /// </summary>
    public void UpdateCenterCard()
    {
        if (GameManager.Instance == null || GameManager.Instance.CenterCard == null)
            return;

        // Destroy old center card visual
        if (centerCardObject != null)
        {
            Destroy(centerCardObject);
        }

        // Create new center card
        centerCardObject = Instantiate(cardPrefab, centerCardSlot);
        CardUI cardUI = centerCardObject.GetComponent<CardUI>();

        if (cardUI != null)
        {
            cardUI.SetupCard(GameManager.Instance.CenterCard, -1); // -1 = not in hand
        }

        // Center card should not be interactive
        RectTransform rt = centerCardObject.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one * 1.2f; // Slightly larger
        }
    }

    /// <summary>
    /// Updates all cards in the player's hand
    /// </summary>
    public void UpdateHandDisplay()
    {
        if (GameManager.Instance == null) return;

        // STOP ALL RUNNING COROUTINES FIRST
        StopAllCoroutines();

        // Clear existing hand cards
        ClearHandCards();

        List<Card> hand = GameManager.Instance.PlayerHand;

        if (hand == null) return;

        // Create card UI for each card in hand
        for (int i = 0; i < hand.Count; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, playerHandContainer);
            CardUI cardUI = cardObj.GetComponent<CardUI>();

            if (cardUI != null)
            {
                cardUI.SetupCard(hand[i], i);

                // Set original position for hover effect
                RectTransform rt = cardObj.GetComponent<RectTransform>();
                if (rt != null)
                {
                    // Cards will be arranged by HorizontalLayoutGroup
                    // So we set original position after layout
                    StartCoroutine(SetCardPositionAfterLayout(cardUI));
                }
            }

            handCardObjects.Add(cardObj);
        }
    }

    /// <summary>
    /// Waits one frame for layout to calculate, then stores position
    /// </summary>
    private IEnumerator SetCardPositionAfterLayout(CardUI cardUI)
    {
        yield return new WaitForEndOfFrame();

        // SAFETY CHECK: Verify cardUI still exists
        if (cardUI == null) yield break;
        if (cardUI.gameObject == null) yield break;

        try
        {
            RectTransform rt = cardUI.GetComponent<RectTransform>();
            if (rt != null)
            {
                cardUI.SetOriginalPosition(rt.anchoredPosition);
            }
        }
        catch (MissingReferenceException)
        {
            // Card was destroyed, ignore
        }
    }

    /// <summary>
    /// Removes all card GameObjects from hand
    /// </summary>
    private void ClearHandCards()
    {
        foreach (GameObject cardObj in handCardObjects)
        {
            if (cardObj != null)
            {
                // Disable the card first to stop Update() calls
                cardObj.SetActive(false);

                // Then destroy
                Destroy(cardObj);
            }
        }
        handCardObjects.Clear();
    }

    // ----- Feedback Messages -----

    /// <summary>
    /// Shows "Invalid Move" message for 1.5 seconds
    /// </summary>
    public void ShowInvalidMoveMessage()
    {
        if (invalidMoveCoroutine != null)
        {
            StopCoroutine(invalidMoveCoroutine);
        }
        invalidMoveCoroutine = StartCoroutine(ShowInvalidMoveRoutine());
    }

    private IEnumerator ShowInvalidMoveRoutine()
    {
        if (invalidMoveText != null)
        {
            invalidMoveText.gameObject.SetActive(true);
            invalidMoveText.text = "Invalid Move!";

            // Optional: Shake animation
            Vector3 originalPos = invalidMoveText.rectTransform.anchoredPosition;
            float elapsed = 0f;
            float shakeDuration = 0.3f;

            while (elapsed < shakeDuration)
            {
                float x = Random.Range(-5f, 5f);
                invalidMoveText.rectTransform.anchoredPosition = originalPos + new Vector3(x, 0, 0);
                elapsed += Time.deltaTime;
                yield return null;
            }

            invalidMoveText.rectTransform.anchoredPosition = originalPos;

            yield return new WaitForSeconds(1.2f);

            invalidMoveText.gameObject.SetActive(false);
        }
    }

    private void HideInvalidMoveText()
    {
        if (invalidMoveText != null)
        {
            invalidMoveText.gameObject.SetActive(false);
        }
    }

    // ----- Screen Management -----

    private void HideAllPanels()
    {
        if (gameplayPanel != null) gameplayPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
    }

    private void ShowPanel(GameObject panel)
    {
        if (panel != null) panel.SetActive(true);
    }

    /// <summary>
    /// Shows the Victory screen
    /// </summary>
    public void ShowWinScreen()
    {
        HideAllPanels();
        ShowPanel(winPanel);

        if (winScoreText != null)
        {
            winScoreText.text = GameManager.Instance.Score.ToString();
        }
    }

    /// <summary>
    /// Shows the Game Over screen
    /// </summary>
    public void ShowLoseScreen()
    {
        HideAllPanels();
        ShowPanel(losePanel);

        if (loseScoreText != null)
        {
            loseScoreText.text = GameManager.Instance.Score.ToString();
        }
    }

    // ----- Helper Methods -----

    /// <summary>
    /// Returns the position of the center card slot.
    /// Used by CardUI for drag-and-drop detection.
    /// </summary>
    public Vector2 GetCenterCardPosition()
    {
        if (centerCardSlot != null)
        {
            RectTransform rt = centerCardSlot.GetComponent<RectTransform>();
            return rt.anchoredPosition;
        }
        return Vector2.zero;
    }

    // ----- Button Callbacks -----
    // These are connected to buttons in the Inspector

    public void OnRestartButtonClicked()
    {
        AudioManager.Instance?.PlaySFX("ButtonClick");
        GameManager.Instance?.RestartGame();
    }

    public void OnMainMenuButtonClicked()
    {
        AudioManager.Instance?.PlaySFX("ButtonClick");
        GameManager.Instance?.GoToMainMenu();
    }

    public void OnPlayAgainButtonClicked()
    {
        AudioManager.Instance?.PlaySFX("ButtonClick");
        GameManager.Instance?.RestartGame();
    }
}