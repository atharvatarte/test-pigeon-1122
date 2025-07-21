using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Card : MonoBehaviour
{
    [Header("Card Settings")]
    public int cardId;
    public Sprite cardFace;
    public Sprite cardBack;
    
    [Header("Animation Settings")]
    public float flipDuration = 0.4f;
    public AnimationCurve flipCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float matchPulseDuration = 0.3f;
    public float matchPulseScale = 1.1f;
    
    [Header("References")]
    public Image cardImage;
    public Button cardButton;
    
    private bool isFlipped = false;
    private bool isMatched = false;
    private bool isFlipping = false;
    
    public bool IsFlipped => isFlipped;
    public bool IsMatched => isMatched;
    public bool IsFlipping => isFlipping;
    
    private void Awake()
    {
        if (cardImage == null)
            cardImage = GetComponent<Image>();
        
        if (cardButton == null)
            cardButton = GetComponent<Button>();
            
        cardButton.onClick.AddListener(OnCardClicked);
    }
    
    public void Initialize(int id, Sprite face, Sprite back)
    {
        cardId = id;
        cardFace = face;
        cardBack = back;
        cardImage.sprite = cardBack;
        isFlipped = false;
        isMatched = false;
        cardButton.interactable = true;
    }
    
    public void OnCardClicked()
    {
        if (!isFlipped && !isMatched && !isFlipping)
        {
            GameManager.Instance.OnCardClicked(this);
        }
    }
    
    public bool CanBeClicked()
    {
        return !isFlipped && !isMatched && !isFlipping;
    }
    
    public void FlipCard(bool showFace)
    {
        // Don't start new animation if already flipping to the same state
        if (isFlipping && isFlipped == showFace) return;
        
        // Stop any existing animations except match celebration
        StopAllCoroutines();
        
        // Play flip sound
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayCardFlip();
        
        StartCoroutine(FlipCardCoroutine(showFace));
    }
    
    public void FlipCardImmediate(bool showFace)
    {
        // Immediate flip without animation for rapid gameplay
        StopAllCoroutines();
        isFlipping = false;
        isFlipped = showFace;
        cardImage.sprite = showFace ? cardFace : cardBack;
        transform.localScale = Vector3.one;
    }
    
    private IEnumerator FlipCardCoroutine(bool showFace)
    {
        isFlipping = true;
        
        // Reset scale to normal first
        transform.localScale = Vector3.one;
        
        // First half of flip - scale down
        float elapsedTime = 0f;
        while (elapsedTime < flipDuration / 2f)
        {
            elapsedTime += Time.deltaTime;
            float progress = flipCurve.Evaluate(elapsedTime / (flipDuration / 2f));
            transform.localScale = new Vector3(1f - progress * 0.1f, 1f, 1f);
            yield return null;
        }
        
        // Change sprite
        cardImage.sprite = showFace ? cardFace : cardBack;
        isFlipped = showFace;
        
        // Second half of flip - scale back up
        elapsedTime = 0f;
        while (elapsedTime < flipDuration / 2f)
        {
            elapsedTime += Time.deltaTime;
            float progress = flipCurve.Evaluate(elapsedTime / (flipDuration / 2f));
            transform.localScale = new Vector3(0.9f + progress * 0.1f, 1f, 1f);
            yield return null;
        }
        
        transform.localScale = Vector3.one;
        isFlipping = false;
    }
    
    public void SetMatched()
    {
        isMatched = true;
        cardButton.interactable = false;
        
        // Add visual feedback for matched cards
        cardImage.color = new Color(0.7f, 1f, 0.7f, 0.8f);
        
        // Start match celebration animation
        StartCoroutine(MatchCelebration());
    }
    
    private IEnumerator MatchCelebration()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * matchPulseScale;
        
        // Pulse up
        float elapsedTime = 0f;
        while (elapsedTime < matchPulseDuration / 2f)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / (matchPulseDuration / 2f);
            transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            yield return null;
        }
        
        // Pulse down
        elapsedTime = 0f;
        while (elapsedTime < matchPulseDuration / 2f)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / (matchPulseDuration / 2f);
            transform.localScale = Vector3.Lerp(targetScale, originalScale, progress);
            yield return null;
        }
        
        transform.localScale = originalScale;
    }
    
    public void ResetCard()
    {
        // Stop any ongoing animations
        StopAllCoroutines();
        
        isFlipped = false;
        isMatched = false;
        isFlipping = false;
        cardImage.sprite = cardBack;
        cardImage.color = Color.white;
        cardButton.interactable = true;
        transform.localScale = Vector3.one;
    }
    
    public void ForceFlipBack()
    {
        // Force the card to show back side immediately
        StopAllCoroutines();
        isFlipped = false;
        isFlipping = false;
        cardImage.sprite = cardBack;
        transform.localScale = Vector3.one;
    }
    
    public void ShowFaceImmediately()
    {
        // Show card face without animation (for loading saved games)
        StopAllCoroutines();
        isFlipped = true;
        isFlipping = false;
        cardImage.sprite = cardFace;
        transform.localScale = Vector3.one;
    }
} 