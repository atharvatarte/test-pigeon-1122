using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardGrid : MonoBehaviour
{
    [Header("Grid Layout")]
    public float spacing = 20f;
    public Vector2 cardSize = new Vector2(100f, 140f);
    
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
            // Temporarily disable the GridLayoutGroup to force a complete reset
            gridLayout.enabled = false;
            
            // Set all properties
            gridLayout.cellSize = cardSize;
            gridLayout.spacing = new Vector2(spacing, spacing);
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            
            // Use the higher number as constraint count for better layout
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
    

} 