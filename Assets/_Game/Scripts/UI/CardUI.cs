using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// CardUI represents the visual version of a Card.
/// Uses full card sprites (Kenney boardgame pack).
/// </summary>
public class CardUI : MonoBehaviour, IPointerClickHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    [Header("Card UI Elements")]
    [SerializeField] private Image cardImage;

    [Header("Hover Settings")]
    [SerializeField] private float hoverRaiseAmount = 30f;
    [SerializeField] private float hoverSpeed = 10f;

    // Private variables
    private Card cardData;
    private int handIndex;
    private Vector3 originalPosition;
    private bool isHovering = false;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private bool isBeingDestroyed = false; // NEW FLAG

    // Properties
    public Card CardData => cardData;
    public int HandIndex => handIndex;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void OnDestroy()
    {
        // Mark as being destroyed to prevent further updates
        isBeingDestroyed = true;
        StopAllCoroutines();
    }

    private void Update()
    {
        // SAFETY CHECKS - prevent MissingReferenceException
        if (isBeingDestroyed) return;
        if (this == null || !this) return;
        if (rectTransform == null) return;
        if (gameObject == null || !gameObject.activeInHierarchy) return;

        // Smooth hover animation
        try
        {
            if (isHovering && originalPosition != Vector3.zero)
            {
                Vector3 targetPos = originalPosition + Vector3.up * hoverRaiseAmount;
                rectTransform.anchoredPosition = Vector3.Lerp(
                    rectTransform.anchoredPosition,
                    targetPos,
                    Time.deltaTime * hoverSpeed
                );
            }
            else if (originalPosition != Vector3.zero)
            {
                rectTransform.anchoredPosition = Vector3.Lerp(
                    rectTransform.anchoredPosition,
                    originalPosition,
                    Time.deltaTime * hoverSpeed
                );
            }
        }
        catch (MissingReferenceException)
        {
            // Object was destroyed mid-update, ignore
            isBeingDestroyed = true;
        }
    }

    public void SetupCard(Card card, int index = -1)
    {
        if (isBeingDestroyed) return;

        cardData = card;
        handIndex = index;

        string spriteName = GetKenneySpriteName(card);
        Sprite cardSprite = Resources.Load<Sprite>($"Cards/{spriteName}");

        if (cardSprite != null && cardImage != null)
        {
            cardImage.sprite = cardSprite;
        }
        else
        {
            Debug.LogWarning($"Card sprite not found: Cards/{spriteName}");
        }

        gameObject.name = $"Card_{card}";
    }

    private string GetKenneySpriteName(Card card)
    {
        string suit = card.CardSuit.ToString();
        string rank = GetRankCode(card.CardRank);
        return $"card{suit}{rank}";
    }

    private string GetRankCode(Rank rank)
    {
        switch (rank)
        {
            case Rank.Ace: return "A";
            case Rank.Jack: return "J";
            case Rank.Queen: return "Q";
            case Rank.King: return "K";
            default: return ((int)rank).ToString();
        }
    }

    public void SetOriginalPosition(Vector3 position)
    {
        originalPosition = position;
    }

    // ----- Event Handlers -----

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isBeingDestroyed) return;
        if (handIndex < 0) return;

        Debug.Log($"Clicked card: {cardData} at index {handIndex}");

        if (GameManager.Instance != null)
        {
            AudioManager.Instance?.PlaySFX("CardClick");
            GameManager.Instance.TryPlayCard(handIndex);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isBeingDestroyed) return;
        if (handIndex < 0) return;

        isHovering = true;
        if (transform != null)
            transform.localScale = Vector3.one * 1.08f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isBeingDestroyed) return;
        if (handIndex < 0) return;

        isHovering = false;
        if (transform != null)
            transform.localScale = Vector3.one;
    }
}