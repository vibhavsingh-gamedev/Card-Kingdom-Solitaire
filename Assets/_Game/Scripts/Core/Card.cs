using UnityEngine;

/// <summary>
/// Enum for card suits - Hearts, Diamonds, Clubs, Spades
/// </summary>
public enum Suit
{
    Hearts,    
    Diamonds,  
    Clubs,     
    Spades     
}

/// <summary>
/// Enum for card ranks with their integer values
/// </summary>
public enum Rank
{
    Ace = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 11,
    Queen = 12,
    King = 13
}

/// <summary>
/// Represents a single playing card with a Suit and Rank.
/// This is a plain C# class (not MonoBehaviour) because 
/// it only stores data - no Unity component behavior needed.
/// </summary>
[System.Serializable]
public class Card
{
    // ----- Properties -----
    public Suit CardSuit { get; private set; }
    public Rank CardRank { get; private set; }

    /// <summary>
    /// Constructor - creates a new card with given suit and rank
    /// </summary>
    /// <param name="suit">The suit of the card</param>
    /// <param name="rank">The rank of the card</param>
    public Card(Suit suit, Rank rank)
    {
        CardSuit = suit;
        CardRank = rank;
    }

    /// <summary>
    /// Returns the integer value of the card's rank.
    /// Used for comparing card values.
    /// Ace=1, 2=2, ..., 10=10, Jack=11, Queen=12, King=13
    /// </summary>
    public int GetRankValue()
    {
        return (int)CardRank;
    }

    /// <summary>
    /// Checks if this card can be played on the given center card.
    /// RULE 1: Same Suit OR
    /// RULE 2: Higher Rank
    /// </summary>
    /// <param name="centerCard">The current card on the center pile</param>
    /// <returns>True if this card can be played, false otherwise</returns>
    public bool CanPlayOn(Card centerCard)
    {
        // Rule 1: Same Suit
        bool sameSuit = this.CardSuit == centerCard.CardSuit;

        // Rule 2: Higher Rank
        bool higherRank = this.GetRankValue() > centerCard.GetRankValue();

        // Either condition satisfies
        return sameSuit || higherRank;
    }

    /// <summary>
    /// Returns suit symbol for display
    /// </summary>
    public string GetSuitSymbol()
    {
        switch (CardSuit)
        {
            case Suit.Hearts: return "";
            case Suit.Diamonds: return "";
            case Suit.Clubs: return "";
            case Suit.Spades: return "";
            default: return "?";
        }
    }

    /// <summary>
    /// Returns short rank string for display
    /// </summary>
    public string GetRankString()
    {
        switch (CardRank)
        {
            case Rank.Ace: return "A";
            case Rank.Jack: return "J";
            case Rank.Queen: return "Q";
            case Rank.King: return "K";
            default: return GetRankValue().ToString();
        }
    }

    /// <summary>
    /// Returns the sprite name for loading card sprites
    /// Format: "Hearts_A", "Spades_10", "Diamonds_K" etc.
    /// </summary>
    public string GetSpriteName()
    {
        return $"{CardSuit}_{GetRankString()}";
    }

    /// <summary>
    /// Returns card as readable string like "A", "10", "K"
    /// </summary>
    public override string ToString()
    {
        return $"{GetRankString()}{GetSuitSymbol()}";
    }

    /// <summary>
    /// Returns the color of the card (Red for Hearts/Diamonds, Black for Clubs/Spades)
    /// </summary>
    public Color GetCardColor()
    {
        if (CardSuit == Suit.Hearts || CardSuit == Suit.Diamonds)
            return Color.red;
        else
            return Color.black;
    }
}