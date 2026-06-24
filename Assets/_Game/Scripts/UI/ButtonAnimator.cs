using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Adds smooth scale animation to buttons on hover and press.
/// Attach to any Button GameObject for polished UI feedback.
/// </summary>
[RequireComponent(typeof(Button))]
public class ButtonAnimator : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [Header("Animation Settings")]
    [SerializeField] private float hoverScale = 1.08f;
    [SerializeField] private float pressedScale = 0.95f;
    [SerializeField] private float animationSpeed = 12f;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private Button button;
    private bool isHovering = false;
    private bool isPressed = false;

    private void Awake()
    {
        button = GetComponent<Button>();
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    private void Update()
    {
        // Smooth scale animation
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * animationSpeed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button != null && !button.interactable) return;

        isHovering = true;
        targetScale = originalScale * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        isPressed = false;
        targetScale = originalScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (button != null && !button.interactable) return;

        isPressed = true;
        targetScale = originalScale * pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        if (isHovering)
            targetScale = originalScale * hoverScale;
        else
            targetScale = originalScale;
    }

    private void OnDisable()
    {
        // Reset to original when disabled
        transform.localScale = originalScale;
        isHovering = false;
        isPressed = false;
    }
}