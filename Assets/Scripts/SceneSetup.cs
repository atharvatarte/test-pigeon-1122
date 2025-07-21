using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SceneSetup : MonoBehaviour
{
    [Header("Setup Options")]
    public bool setupOnStart = false;
    public bool createCardSprites = false;
    
    [Header("Grid Settings")]
    public int gridWidth = 4;
    public int gridHeight = 4;
    
    [Header("Card Settings")]
    public Vector2 cardSize = new Vector2(100f, 140f);
    public float spacing = 20f;
    
    private void Start()
    {
        if (setupOnStart)
        {
            SetupScene();
        }
    }
    
    [ContextMenu("Setup Memory Game Scene")]
    public void SetupScene()
    {
        SetupCanvas();
        SetupGameManager();
        SetupCardGrid();
        SetupUIElements();
        SetupAudioManager();
        CreateCardPrefab();
        
        Debug.Log("Memory Game scene setup complete! Check the README for next steps.");
    }
    
    private void SetupCanvas()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
        }
    }
    
    private void SetupGameManager()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            GameObject gameManagerObj = new GameObject("GameManager");
            gameManager = gameManagerObj.AddComponent<GameManager>();
            gameManager.gridWidth = gridWidth;
            gameManager.gridHeight = gridHeight;
        }
    }
    
    private void SetupCardGrid()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;
        
        GameObject cardGridObj = GameObject.Find("CardGrid");
        if (cardGridObj == null)
        {
            cardGridObj = new GameObject("CardGrid");
            cardGridObj.transform.SetParent(canvas.transform, false);
            
            RectTransform rectTransform = cardGridObj.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.1f, 0.2f);
            rectTransform.anchorMax = new Vector2(0.9f, 0.8f);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            
            GridLayoutGroup gridLayout = cardGridObj.AddComponent<GridLayoutGroup>();
            gridLayout.cellSize = cardSize;
            gridLayout.spacing = new Vector2(spacing, spacing);
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = gridWidth;
            gridLayout.childAlignment = TextAnchor.MiddleCenter;
            
            cardGridObj.AddComponent<CardGrid>();
        }
    }
    
    private void SetupUIElements()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;
        
        // Score Text
        CreateUIText("ScoreText", "Score: 0", new Vector2(0.1f, 0.9f), new Vector2(0.3f, 0.95f));
        
        // Timer Text
        CreateUIText("TimerText", "Time: 00:00", new Vector2(0.35f, 0.9f), new Vector2(0.65f, 0.95f));
        
        // Moves Text
        CreateUIText("MovesText", "Moves: 0", new Vector2(0.7f, 0.9f), new Vector2(0.9f, 0.95f));
        
        // Restart Button
        CreateUIButton("RestartButton", "Restart", new Vector2(0.4f, 0.05f), new Vector2(0.6f, 0.15f));
        
        // Game Over Panel
        CreateGameOverPanel();
    }
    
    private void CreateUIText(string name, string text, Vector2 anchorMin, Vector2 anchorMax)
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;
        
        GameObject textObj = GameObject.Find(name);
        if (textObj == null)
        {
            textObj = new GameObject(name);
            textObj.transform.SetParent(canvas.transform, false);
            
            RectTransform rectTransform = textObj.AddComponent<RectTransform>();
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            
            TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
            tmpText.text = text;
            tmpText.fontSize = 24;
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.color = Color.white;
        }
    }
    
    private void CreateUIButton(string name, string text, Vector2 anchorMin, Vector2 anchorMax)
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;
        
        GameObject buttonObj = GameObject.Find(name);
        if (buttonObj == null)
        {
            buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(canvas.transform, false);
            
            RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            
            Image image = buttonObj.AddComponent<Image>();
            image.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            
            Button button = buttonObj.AddComponent<Button>();
            
            // Create text child
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
            tmpText.text = text;
            tmpText.fontSize = 20;
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.color = Color.white;
        }
    }
    
    private void CreateGameOverPanel()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;
        
        GameObject panelObj = GameObject.Find("GameOverPanel");
        if (panelObj == null)
        {
            panelObj = new GameObject("GameOverPanel");
            panelObj.transform.SetParent(canvas.transform, false);
            
            RectTransform rectTransform = panelObj.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            
            Image image = panelObj.AddComponent<Image>();
            image.color = new Color(0, 0, 0, 0.8f);
            
            // Final Score Text
            CreateUIText("FinalScoreText", "Final Score: 0\nMoves: 0\nTime: 00:00", 
                new Vector2(0.3f, 0.4f), new Vector2(0.7f, 0.6f));
            
            // Play Again Button
            CreateUIButton("PlayAgainButton", "Play Again", 
                new Vector2(0.4f, 0.25f), new Vector2(0.6f, 0.35f));
            
            panelObj.SetActive(false);
        }
    }
    
    private void SetupAudioManager()
    {
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager == null)
        {
            GameObject audioManagerObj = new GameObject("AudioManager");
            audioManager = audioManagerObj.AddComponent<AudioManager>();
        }
    }
    
    private void CreateCardPrefab()
    {
        GameObject cardPrefab = GameObject.Find("CardPrefab");
        if (cardPrefab == null)
        {
            cardPrefab = new GameObject("CardPrefab");
            
            RectTransform rectTransform = cardPrefab.AddComponent<RectTransform>();
            rectTransform.sizeDelta = cardSize;
            
            Image image = cardPrefab.AddComponent<Image>();
            image.color = Color.white;
            image.type = Image.Type.Simple;
            
            Button button = cardPrefab.AddComponent<Button>();
            
            cardPrefab.AddComponent<Card>();
            cardPrefab.AddComponent<CardPrefab>();
            
            // Create prefab
            #if UNITY_EDITOR
            UnityEditor.PrefabUtility.SaveAsPrefabAsset(cardPrefab, "Assets/CardPrefab.prefab");
            DestroyImmediate(cardPrefab);
            #endif
        }
    }
} 