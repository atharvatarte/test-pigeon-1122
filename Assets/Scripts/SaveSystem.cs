using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class CardSaveData
{
    public int cardId;
    public bool isFlipped;
    public bool isMatched;
    public int gridPosition; // Index in the grid
}

[Serializable]
public class GameSaveData
{
    public int score;
    public int moves;
    public float gameTime;
    public int matchedPairs;
    public int totalPairs;
    public bool gameStarted;
    public bool canFlip;
    public List<CardSaveData> cards;
    public List<int> flippedCardIndices; // Store indices of currently flipped cards
    public DateTime saveTime;
    public int gridWidth;
    public int gridHeight;
    public string gameVersion = "1.0";
}

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }
    
    [Header("Save Settings")]
    public bool autoSave = true;
    public float autoSaveInterval = 30f; // Save every 30 seconds
    public bool saveOnPause = true;
    
    [Header("Save Keys")]
    public string saveDataKey = "MemoryGame_SaveData";
    public string highScoreKey = "MemoryGame_HighScore";
    public string totalGamesKey = "MemoryGame_TotalGames";
    
    private float lastAutoSaveTime;
    private GameSaveData currentSaveData;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        lastAutoSaveTime = Time.time;
    }
    
    private void Update()
    {
        if (autoSave && GameManager.Instance != null && GameManager.Instance.IsGameStarted)
        {
            if (Time.time - lastAutoSaveTime >= autoSaveInterval)
            {
                SaveGame();
                lastAutoSaveTime = Time.time;
            }
        }
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && saveOnPause)
        {
            SaveGame();
        }
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && saveOnPause)
        {
            SaveGame();
        }
    }
    
    public void SaveGame()
    {
        if (GameManager.Instance == null) return;
        
        currentSaveData = CreateSaveData();
        string jsonData = JsonUtility.ToJson(currentSaveData, true);
        PlayerPrefs.SetString(saveDataKey, jsonData);
        PlayerPrefs.Save();
    }
    
    public bool LoadGame()
    {
        if (!PlayerPrefs.HasKey(saveDataKey))
        {
            return false;
        }
        
        try
        {
            string jsonData = PlayerPrefs.GetString(saveDataKey);
            currentSaveData = JsonUtility.FromJson<GameSaveData>(jsonData);
            
            // Validate save data
            if (currentSaveData == null || currentSaveData.cards == null)
            {
                return false;
            }
            
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
    
    public void DeleteSaveData()
    {
        PlayerPrefs.DeleteKey(saveDataKey);
        PlayerPrefs.Save();
        currentSaveData = null;
    }
    
    public bool HasSaveData()
    {
        return PlayerPrefs.HasKey(saveDataKey);
    }
    
    public GameSaveData GetCurrentSaveData()
    {
        return currentSaveData;
    }
    
    private GameSaveData CreateSaveData()
    {
        GameManager gameManager = GameManager.Instance;
        if (gameManager == null) return null;
        
        GameSaveData saveData = new GameSaveData
        {
            score = gameManager.GetScore(),
            moves = gameManager.GetMoves(),
            gameTime = gameManager.GetGameTime(),
            matchedPairs = gameManager.GetMatchedPairs(),
            totalPairs = gameManager.GetTotalPairs(),
            gameStarted = gameManager.IsGameStarted,
            canFlip = gameManager.CanFlip,
            gridWidth = gameManager.GetGridWidth(),
            gridHeight = gameManager.GetGridHeight(),
            saveTime = DateTime.Now,
            cards = new List<CardSaveData>(),
            flippedCardIndices = new List<int>()
        };
        
        // Save card data
        List<Card> cards = gameManager.GetCards();
        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            CardSaveData cardData = new CardSaveData
            {
                cardId = card.cardId,
                isFlipped = card.IsFlipped,
                isMatched = card.IsMatched,
                gridPosition = i
            };
            saveData.cards.Add(cardData);
            
            // Track flipped cards
            if (card.IsFlipped && !card.IsMatched)
            {
                saveData.flippedCardIndices.Add(i);
            }
        }
        
        return saveData;
    }
    
    public void SaveHighScore(int score)
    {
        int currentHighScore = PlayerPrefs.GetInt(highScoreKey, 0);
        if (score > currentHighScore)
        {
            PlayerPrefs.SetInt(highScoreKey, score);
            PlayerPrefs.Save();
        }
    }
    
    public int GetHighScore()
    {
        return PlayerPrefs.GetInt(highScoreKey, 0);
    }
    
    public void IncrementTotalGames()
    {
        int totalGames = PlayerPrefs.GetInt(totalGamesKey, 0);
        PlayerPrefs.SetInt(totalGamesKey, totalGames + 1);
        PlayerPrefs.Save();
    }
    
    public int GetTotalGames()
    {
        return PlayerPrefs.GetInt(totalGamesKey, 0);
    }
    
    public string GetSaveTimeString()
    {
        if (currentSaveData != null)
        {
            return currentSaveData.saveTime.ToString("MMM dd, yyyy HH:mm");
        }
        return "No save data";
    }
    
    public bool IsSaveDataValid()
    {
        if (currentSaveData == null) return false;
        
        // Check if save data is not too old (optional)
        TimeSpan timeSinceSave = DateTime.Now - currentSaveData.saveTime;
        if (timeSinceSave.TotalDays > 30) // 30 days old
        {
            return false;
        }
        
        return true;
    }
} 