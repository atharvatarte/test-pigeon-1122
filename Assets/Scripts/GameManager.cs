using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game Settings")]
    [Tooltip("Grid width - at least one dimension (width or height) must be even")]
    public int gridWidth = 4;
    [Tooltip("Grid height - at least one dimension (width or height) must be even")]
    public int gridHeight = 4;
    public float initialShowTime = 3f;
    public float matchCheckDelay = 0.5f;
    
    [Header("UI References")]
    public Transform cardGrid;
    public GameObject cardPrefab;
    public UIManager uiManager;
    public CardGrid cardGridComponent;
    
    [Header("Card Sprites")]
    public Sprite[] cardFaces;
    public Sprite cardBackSprite;
    
    private List<Card> cards = new List<Card>();
    private List<Card> flippedCards = new List<Card>();
    private bool canFlip = false;
    private bool gameStarted = false;
    
    // Game stats
    private int score = 0;
    private int moves = 0;
    private float gameTime = 0f;
    private int matchedPairs = 0;
    private int totalPairs;
    
    // Save system integration
    private bool isLoadingGame = false;
    
    // Smooth gameplay settings
    private bool isProcessingMatch = false;
    private Queue<Card> pendingCardFlips = new Queue<Card>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Check for save data first
        if (SaveSystem.Instance != null && SaveSystem.Instance.HasSaveData())
        {
            if (uiManager != null)
                uiManager.ShowSaveLoadPrompt();
            else
                ShowLoadGamePrompt();
        }
        else
        {
            InitializeGame();
        }
    }
    
    private void Update()
    {
        if (gameStarted)
        {
            gameTime += Time.deltaTime;
            UpdateTimer();
        }
        
        // Check for screen size changes and update card scaling
        CheckScreenSizeChange();
    }
    
    private Vector2 lastScreenSize;
    
    private void CheckScreenSizeChange()
    {
        Vector2 currentScreenSize = new Vector2(Screen.width, Screen.height);
        if (lastScreenSize != Vector2.zero && lastScreenSize != currentScreenSize)
        {
            // Screen size changed, update card scaling
            if (cardGridComponent != null)
            {
                cardGridComponent.UpdateCardScaling();
            }
        }
        lastScreenSize = currentScreenSize;
    }
    
    public void InitializeGame()
    {
        ValidateGridDimensions();
        ClearGrid();
        CreateCards();
        ArrangeCards();
        StartCoroutine(ShowCardsInitially());
    }
    
    private void ValidateGridDimensions()
    {
        bool widthIsOdd = gridWidth % 2 != 0;
        bool heightIsOdd = gridHeight % 2 != 0;
        
        if (widthIsOdd && heightIsOdd)
        {
            // Both are odd - increment the smaller dimension by 1
            if (gridWidth <= gridHeight)
            {
                gridWidth++;
                Debug.Log($"Grid validation: Both dimensions were odd. Incremented width to {gridWidth} to ensure at least one even dimension.");
            }
            else
            {
                gridHeight++;
                Debug.Log($"Grid validation: Both dimensions were odd. Incremented height to {gridHeight} to ensure at least one even dimension.");
            }
        }
    }
    
    private void ClearGrid()
    {
        foreach (Transform child in cardGrid)
        {
            Destroy(child.gameObject);
        }
        cards.Clear();
        flippedCards.Clear();
    }
    
    private void CreateCards()
    {
        totalPairs = (gridWidth * gridHeight) / 2;
        
        // Create a list of card IDs (each ID appears twice for pairs)
        List<int> cardIds = new List<int>();
        for (int i = 0; i < totalPairs; i++)
        {
            cardIds.Add(i); // First card of pair
            cardIds.Add(i); // Second card of pair
        }
        
        // Shuffle the card IDs
        ShuffleCardIds(cardIds);
        
        // Create cards with shuffled IDs
        for (int i = 0; i < cardIds.Count; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardGrid);
            Card card = cardObj.GetComponent<Card>();
            
            if (card == null)
            {
                card = cardObj.AddComponent<Card>();
            }
            
            int cardId = cardIds[i];
            card.Initialize(cardId, cardFaces[cardId % cardFaces.Length], cardBackSprite);
            cards.Add(card);
        }
        
        // Setup grid layout after creating cards
        if (cardGridComponent != null)
        {
            cardGridComponent.SetupGrid(gridWidth, gridHeight);
        }
    }
    
    private void ShuffleCardIds(List<int> cardIds)
    {
        for (int i = cardIds.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = cardIds[i];
            cardIds[i] = cardIds[randomIndex];
            cardIds[randomIndex] = temp;
        }
    }
    
    private void ArrangeCards()
    {
        // Use CardGrid component if available, otherwise use manual positioning
        if (cardGridComponent != null)
        {
            cardGridComponent.SetupGrid(gridWidth, gridHeight);
        }
        else
        {
            // Fallback to manual positioning
            float cardWidth = 100f;
            float cardHeight = 140f;
            float spacing = 20f;
            
            float startX = -(gridWidth - 1) * (cardWidth + spacing) / 2f;
            float startY = (gridHeight - 1) * (cardHeight + spacing) / 2f;
            
            for (int i = 0; i < cards.Count; i++)
            {
                int row = i / gridWidth;
                int col = i % gridWidth;
                
                Vector3 position = new Vector3(
                    startX + col * (cardWidth + spacing),
                    startY - row * (cardHeight + spacing),
                    0f
                );
                
                cards[i].transform.localPosition = position;
            }
        }
    }
    
    private IEnumerator ShowCardsInitially()
    {
        // Show all cards face up
        foreach (Card card in cards)
        {
            card.FlipCard(true);
        }
        
        yield return new WaitForSeconds(initialShowTime);
        
        // Flip all cards back
        foreach (Card card in cards)
        {
            card.FlipCard(false);
        }
        
        yield return new WaitForSeconds(0.5f);
        
        // Start the game
        canFlip = true;
        gameStarted = true;
        ResetStats();
    }
    
    private void ResetStats()
    {
        score = 0;
        moves = 0;
        gameTime = 0f;
        matchedPairs = 0;
        isProcessingMatch = false;
        pendingCardFlips.Clear();
        UpdateUI();
    }
    
    public void OnCardClicked(Card card)
    {
        // Check if card can be clicked
        if (!canFlip || !card.CanBeClicked()) return;
        
        // Add to pending flips if we're processing a match
        if (isProcessingMatch)
        {
            pendingCardFlips.Enqueue(card);
            return;
        }
        
        // Flip the card immediately
        card.FlipCard(true);
        flippedCards.Add(card);
        
        // Check if we have 2 cards flipped
        if (flippedCards.Count == 2)
        {
            moves++;
            StartCoroutine(ProcessMatch());
        }
    }
    
    private IEnumerator ProcessMatch()
    {
        isProcessingMatch = true;
        
        // Brief delay to show the second card
        yield return new WaitForSeconds(matchCheckDelay);
        
        Card card1 = flippedCards[0];
        Card card2 = flippedCards[1];
        
        if (card1.cardId == card2.cardId)
        {
            // Match found - keep cards flipped
            card1.SetMatched();
            card2.SetMatched();
            matchedPairs++;
            score += 100;
            
            // Play match sound
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayCardMatch();
            
            if (matchedPairs >= totalPairs)
            {
                GameOver();
            }
        }
        else
        {
            // No match - flip both cards back
            card1.FlipCard(false);
            card2.FlipCard(false);
            score = Mathf.Max(0, score - 10);
            
            // Play mismatch sound
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayCardMismatch();
        }
        
        flippedCards.Clear();
        isProcessingMatch = false;
        UpdateUI();
        
        // Process any pending card flips
        ProcessPendingFlips();
    }
    
    private void ProcessPendingFlips()
    {
        while (pendingCardFlips.Count > 0 && !isProcessingMatch)
        {
            Card pendingCard = pendingCardFlips.Dequeue();
            
            // Check if card is still valid to flip
            if (pendingCard.CanBeClicked())
            {
                pendingCard.FlipCard(true);
                flippedCards.Add(pendingCard);
                
                if (flippedCards.Count == 2)
                {
                    moves++;
                    StartCoroutine(ProcessMatch());
                    break;
                }
            }
        }
    }
    
    private void UpdateUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateScore(score);
            uiManager.UpdateMoves(moves);
        }
    }
    
    private void UpdateTimer()
    {
        if (uiManager != null)
        {
            uiManager.UpdateTimer(gameTime);
        }
    }
    
    private void GameOver()
    {
        gameStarted = false;
        canFlip = false;
        
        // Save high score
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.SaveHighScore(score);
            SaveSystem.Instance.IncrementTotalGames();
            SaveSystem.Instance.DeleteSaveData(); // Clear save data when game is complete
        }
        
        // Play win sound
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayGameWin();
        
        // Show game over screen
        if (uiManager != null)
        {
            int highScore = SaveSystem.Instance != null ? SaveSystem.Instance.GetHighScore() : 0;
            uiManager.ShowGameOverScreen(score, moves, gameTime, highScore);
        }
    }
    
    public void RestartGame()
    {
        if (uiManager != null)
            uiManager.HideGameOverScreen();
        
        // Clear save data when starting new game
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.DeleteSaveData();
        }
        
        InitializeGame();
    }
    
    // Save System Integration Methods
    public void LoadGameFromSave()
    {
        if (SaveSystem.Instance == null || !SaveSystem.Instance.LoadGame())
        {
            InitializeGame();
            return;
        }
        
        GameSaveData saveData = SaveSystem.Instance.GetCurrentSaveData();
        if (saveData == null || !SaveSystem.Instance.IsSaveDataValid())
        {
            InitializeGame();
            return;
        }
        
        isLoadingGame = true;
        LoadGameState(saveData);
    }
    
    private void LoadGameState(GameSaveData saveData)
    {
        // Set grid dimensions
        gridWidth = saveData.gridWidth;
        gridHeight = saveData.gridHeight;
        
        // Clear existing grid
        ClearGrid();
        
        // Create cards with saved data
        CreateCardsFromSave(saveData);
        ArrangeCards();
        
        // Restore game state
        score = saveData.score;
        moves = saveData.moves;
        gameTime = saveData.gameTime;
        matchedPairs = saveData.matchedPairs;
        totalPairs = saveData.totalPairs;
        gameStarted = saveData.gameStarted;
        canFlip = saveData.canFlip;
        
        // Restore card states
        RestoreCardStates(saveData);
        
        // Update UI
        UpdateUI();
        
        // If game was in progress, show current state
        if (gameStarted)
        {
            ShowCurrentGameState();
        }
        
        isLoadingGame = false;
    }
    
    private void CreateCardsFromSave(GameSaveData saveData)
    {
        for (int i = 0; i < saveData.cards.Count; i++)
        {
            CardSaveData cardData = saveData.cards[i];
            
            GameObject cardObj = Instantiate(cardPrefab, cardGrid);
            Card card = cardObj.GetComponent<Card>();
            
            if (card == null)
            {
                card = cardObj.AddComponent<Card>();
            }
            
            card.Initialize(cardData.cardId, cardFaces[cardData.cardId % cardFaces.Length], cardBackSprite);
            cards.Add(card);
        }
    }
    
    private void RestoreCardStates(GameSaveData saveData)
    {
        for (int i = 0; i < cards.Count && i < saveData.cards.Count; i++)
        {
            Card card = cards[i];
            CardSaveData cardData = saveData.cards[i];
            
            // Restore card state
            if (cardData.isMatched)
            {
                card.SetMatched();
            }
            else if (cardData.isFlipped)
            {
                // Show card face without animation
                card.ShowFaceImmediately();
                flippedCards.Add(card);
            }
        }
    }
    
    private void ShowCurrentGameState()
    {
        // Show all cards in their current state
        foreach (Card card in cards)
        {
            if (card.IsFlipped && !card.IsMatched)
            {
                card.ShowFaceImmediately();
            }
        }
    }
    
    private void ShowLoadGamePrompt()
    {
        // Use UIManager if available, otherwise create simple prompt
        if (uiManager != null)
        {
            uiManager.ShowSaveLoadPrompt();
        }
    }
    
    public void ContinueGame()
    {
        if (uiManager != null)
            uiManager.HideSaveLoadPrompt();
        
        LoadGameFromSave();
    }
    
    public void StartNewGame()
    {
        if (uiManager != null)
            uiManager.HideSaveLoadPrompt();
        
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.DeleteSaveData();
        }
        
        InitializeGame();
    }
    
    // Getter methods for SaveSystem
    public int GetScore() => score;
    public int GetMoves() => moves;
    public float GetGameTime() => gameTime;
    public int GetMatchedPairs() => matchedPairs;
    public int GetTotalPairs() => totalPairs;
    public bool IsGameStarted => gameStarted;
    public bool CanFlip => canFlip;
    public int GetGridWidth() => gridWidth;
    public int GetGridHeight() => gridHeight;
    public List<Card> GetCards() => cards;
    
    public void UpdateCardScaling()
    {
        if (cardGridComponent != null)
        {
            cardGridComponent.UpdateCardScaling();
        }
    }
} 