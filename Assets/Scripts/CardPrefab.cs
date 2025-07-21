using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class CardPrefab : MonoBehaviour
{
    [Header("Card Components")]
    public Image cardImage;
    public Button cardButton;
    public Card cardScript;
    
    private void Awake()
    {
        // Ensure we have all required components
        if (cardImage == null)
            cardImage = GetComponent<Image>();
            
        if (cardButton == null)
            cardButton = GetComponent<Button>();
            
        if (cardScript == null)
            cardScript = GetComponent<Card>();
            
        if (cardScript == null)
            cardScript = gameObject.AddComponent<Card>();
            
        // Setup default card appearance
        SetupDefaultAppearance();
    }
    
    private void SetupDefaultAppearance()
    {
        // Set default card size
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(100f, 140f);
        }
        
        // Setup button colors
        ColorBlock colors = cardButton.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        colors.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        colors.selectedColor = Color.white;
        colors.disabledColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);
        cardButton.colors = colors;
        
        // Setup image
        cardImage.type = Image.Type.Simple;
        cardImage.preserveAspect = true;
    }
    
    public void SetCardBack(Sprite backSprite)
    {
        if (cardImage != null && backSprite != null)
        {
            cardImage.sprite = backSprite;
        }
    }
    
    public void SetCardFace(Sprite faceSprite)
    {
        if (cardScript != null)
        {
            cardScript.cardFace = faceSprite;
        }
    }
} 