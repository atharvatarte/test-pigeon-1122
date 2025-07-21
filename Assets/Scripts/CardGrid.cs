using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardGrid : MonoBehaviour
{
    [Header("Grid Layout")]
    public float spacing = 20f;
    public Vector2 cardSize = new Vector2(100f, 140f);
    
    [Header("Responsive Scaling")]
    [Tooltip("Enable automatic card scaling to fit the display area")]
    public bool autoScaleCards = true;
    [Tooltip("Minimum card size to maintain readability")]
    public Vector2 minCardSize = new Vector2(60f, 80f);
    [Tooltip("Maximum card size to prevent oversized cards")]
    public Vector2 maxCardSize = new Vector2(200f, 280f);
    [Tooltip("Padding around the grid to ensure cards don't touch edges")]
    public float gridPadding = 40f;
    
    [Header("References")]
    public GridLayoutGroup gridLayout;
    
    private void Awake()
    {
        if (gridLayout == null)
            gridLayout = GetComponent<GridLayoutGroup>();
    }
    
    public void SetupGrid(int gridWidth, int gridHeight)
    {
        if (gridLayout != null)
        {
            // Calculate optimal card size if auto-scaling is enabled
            Vector2 optimalCardSize = cardSize;
            if (autoScaleCards)
            {
                optimalCardSize = CalculateOptimalCardSize(gridWidth, gridHeight);
            }
            
            // Temporarily disable the GridLayoutGroup to force a complete reset
            gridLayout.enabled = false;
            
            // Set all properties
            gridLayout.cellSize = optimalCardSize;
            gridLayout.spacing = new Vector2(spacing, spacing);
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            
            // Use the higher number as constraint count for better layout
            // This ensures proper layout even after grid dimension adjustments
            int constraintCount = Mathf.Max(gridWidth, gridHeight);
            gridLayout.constraintCount = constraintCount;
            
            gridLayout.childAlignment = TextAnchor.MiddleCenter;
            
            // Re-enable the GridLayoutGroup
            gridLayout.enabled = true;
            
            // Force layout update
            LayoutRebuilder.ForceRebuildLayoutImmediate(gridLayout.GetComponent<RectTransform>());
            
            // Start coroutine to ensure layout updates
            StartCoroutine(ForceLayoutUpdate());
        }
    }
    
    private IEnumerator ForceLayoutUpdate()
    {
        // Wait for end of frame to ensure all changes are applied
        yield return new WaitForEndOfFrame();
        
        if (gridLayout != null)
        {
            // Force another layout rebuild
            LayoutRebuilder.ForceRebuildLayoutImmediate(gridLayout.GetComponent<RectTransform>());
        }
    }
    
    public void SetCardSize(Vector2 size)
    {
        cardSize = size;
        if (gridLayout != null)
        {
            gridLayout.cellSize = cardSize;
        }
    }
    
    public void SetSpacing(float newSpacing)
    {
        spacing = newSpacing;
        if (gridLayout != null)
        {
            gridLayout.spacing = new Vector2(spacing, spacing);
        }
    }
    
    private Vector2 CalculateOptimalCardSize(int gridWidth, int gridHeight)
    {
        // Get the available display area
        RectTransform rectTransform = gridLayout.GetComponent<RectTransform>();
        Vector2 availableSize = rectTransform.rect.size;
        
        // Account for padding
        Vector2 usableSize = availableSize - new Vector2(gridPadding * 2, gridPadding * 2);
        
        // Calculate space needed for spacing
        Vector2 spacingSpace = new Vector2(spacing * (gridWidth - 1), spacing * (gridHeight - 1));
        
        // Calculate available space for cards
        Vector2 cardSpace = usableSize - spacingSpace;
        
        // Calculate individual card size
        Vector2 calculatedSize = new Vector2(
            cardSpace.x / gridWidth,
            cardSpace.y / gridHeight
        );
        
        // Maintain aspect ratio (width:height = 5:7 for cards)
        float targetAspectRatio = 5f / 7f;
        float currentAspectRatio = calculatedSize.x / calculatedSize.y;
        
        if (currentAspectRatio > targetAspectRatio)
        {
            // Too wide, adjust height
            calculatedSize.x = calculatedSize.y * targetAspectRatio;
        }
        else
        {
            // Too tall, adjust width
            calculatedSize.y = calculatedSize.x / targetAspectRatio;
        }
        
        // Clamp to min/max sizes
        calculatedSize.x = Mathf.Clamp(calculatedSize.x, minCardSize.x, maxCardSize.x);
        calculatedSize.y = Mathf.Clamp(calculatedSize.y, minCardSize.y, maxCardSize.y);
        
        return calculatedSize;
    }
    
    public void UpdateCardScaling()
    {
        if (autoScaleCards && gridLayout != null)
        {
            // Get current grid dimensions from GameManager
            GameManager gameManager = GameManager.Instance;
            if (gameManager != null)
            {
                int gridWidth = gameManager.GetGridWidth();
                int gridHeight = gameManager.GetGridHeight();
                
                Vector2 optimalSize = CalculateOptimalCardSize(gridWidth, gridHeight);
                gridLayout.cellSize = optimalSize;
                
                // Force layout update
                LayoutRebuilder.ForceRebuildLayoutImmediate(gridLayout.GetComponent<RectTransform>());
            }
        }
    }
    
    public void SetAutoScale(bool enable)
    {
        autoScaleCards = enable;
        if (enable)
        {
            UpdateCardScaling();
        }
    }
    
    public Vector2 GetCurrentCardSize()
    {
        if (gridLayout != null)
        {
            return gridLayout.cellSize;
        }
        return cardSize;
    }
} 