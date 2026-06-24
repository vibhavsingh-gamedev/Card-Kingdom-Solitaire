using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DeckManager handles all deck-related operations:
/// - Creating a standard 52-card deck
/// - Shuffling the deck
/// - Drawing cards from the deck
/// - Tracking remaining cards
/// </summary>
public class DeckManager : MonoBehaviour
{
    // ----- Singleton Pattern -----
    // Ensures only one DeckManager exists in the scene
    public static DeckManager Instance { get; private set; }

    // ----- Private Variables -----
    private List<Card> deck = new List<Card>();  // The draw pile

    // ----- Public Properties -----
    /// <summary>
    /// Returns how many cards are left in the deck
    /// </summary>
    public int CardsRemaining => deck.Count;

    /// <summary>
    /// Returns true if deck has no cards left
    /// </summary>
    public bool IsDeckEmpty => deck.Count == 0;

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
        }
    }

    // ----- Public Methods -----

    /// <summary>
    /// Creates a fresh 52-card deck.
    /// Iterates through all 4 suits and all 13 ranks.
    /// </summary>
    public void CreateDeck()
    {
        deck.Clear(); // Clear any existing cards

        // Loop through each suit
        foreach (Suit suit in System.Enum.GetValues(typeof(Suit)))
        {
            // Loop through each rank
            foreach (Rank rank in System.Enum.GetValues(typeof(Rank)))
            {
                // Create a new card and add to deck
                Card newCard = new Card(suit, rank);
                deck.Add(newCard);
            }
        }

        Debug.Log($"Deck created with {deck.Count} cards.");
    }

    /// <summary>
    /// Shuffles the deck using Fisher-Yates algorithm.
    /// This is the most efficient and unbiased shuffle method.
    /// </summary>
    public void ShuffleDeck()
    {
        // Fisher-Yates Shuffle Algorithm
        for (int i = deck.Count - 1; i > 0; i--)
        {
            // Pick a random index from 0 to i
            int randomIndex = Random.Range(0, i + 1);

            // Swap cards at positions i and randomIndex
            Card temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }

        Debug.Log("Deck shuffled successfully.");
    }

    /// <summary>
    /// Draws (removes and returns) the top card from the deck.
    /// Returns null if deck is empty.
    /// </summary>
    public Card DrawCard()
    {
        // Safety check
        if (IsDeckEmpty)
        {
            Debug.LogWarning("Tried to draw from an empty deck!");
            return null;
        }

        // Take the last card (top of deck)
        Card drawnCard = deck[deck.Count - 1];

        // Remove it from the deck
        deck.RemoveAt(deck.Count - 1);

        Debug.Log($"Drew card: {drawnCard} | Remaining: {deck.Count}");

        return drawnCard;
    }

    /// <summary>
    /// Draws multiple cards at once.
    /// Used for initial hand distribution (5 cards).
    /// </summary>
    /// <param name="count">Number of cards to draw</param>
    /// <returns>List of drawn cards</returns>
    public List<Card> DrawCards(int count)
    {
        List<Card> drawnCards = new List<Card>();

        for (int i = 0; i < count; i++)
        {
            Card card = DrawCard();
            if (card != null)
            {
                drawnCards.Add(card);
            }
            else
            {
                break; // Deck is empty
            }
        }

        return drawnCards;
    }

    /// <summary>
    /// Resets the deck - creates and shuffles fresh.
    /// Used when restarting the game.
    /// </summary>
    public void ResetDeck()
    {
        CreateDeck();
        ShuffleDeck();
        Debug.Log("Deck has been reset.");
    }
}