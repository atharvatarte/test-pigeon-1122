using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Save/Load UI")]
    public GameObject saveLoadPanel;
    public TextMeshProUGUI saveInfoText;
    public Button continueButton;
    public Button newGameButton;
    public Button deleteSaveButton;
    
    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public Button playAgainButton;
    public Button mainMenuButton;
    
    [Header("In-Game UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI movesText;
    public Button restartButton;
    public Button pauseButton;
    
    [Header("Pause UI")]
    public GameObject pausePanel;
    public Button resumeButton;
    public Button saveAndQuitButton;
    
    private void Start()
    {
        SetupButtonListeners();
    }
    
    private void SetupButtonListeners()
    {
        // Save/Load UI
        if (continueButton != null)
            continueButton.onClick.AddListener(() => GameManager.Instance.ContinueGame());
        
        if (newGameButton != null)
            newGameButton.onClick.AddListener(() => GameManager.Instance.StartNewGame());
        
        if (deleteSaveButton != null)
            deleteSaveButton.onClick.AddListener(DeleteSaveData);
        
        // Game Over UI
        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(() => GameManager.Instance.RestartGame());
        
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ShowMainMenu);
        
        // In-Game UI
        if (restartButton != null)
            restartButton.onClick.AddListener(() => GameManager.Instance.RestartGame());
        
        if (pauseButton != null)
            pauseButton.onClick.AddListener(ShowPauseMenu);
        
        // Pause UI
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
        
        if (saveAndQuitButton != null)
            saveAndQuitButton.onClick.AddListener(SaveAndQuit);
    }
    
    public void ShowSaveLoadPrompt()
    {
        if (saveLoadPanel != null)
        {
            saveLoadPanel.SetActive(true);
            
            if (saveInfoText != null && SaveSystem.Instance != null)
            {
                string saveTime = SaveSystem.Instance.GetSaveTimeString();
                GameSaveData saveData = SaveSystem.Instance.GetCurrentSaveData();
                
                if (saveData != null)
                {
                    string info = $"Continue Previous Game?\n\n" +
                                 $"Saved: {saveTime}\n" +
                                 $"Score: {saveData.score}\n" +
                                 $"Moves: {saveData.moves}\n" +
                                 $"Matched Pairs: {saveData.matchedPairs}/{saveData.totalPairs}\n" +
                                 $"Time: {Mathf.FloorToInt(saveData.gameTime / 60f):00}:{Mathf.FloorToInt(saveData.gameTime % 60f):00}";
                    
                    saveInfoText.text = info;
                }
            }
        }
    }
    
    public void HideSaveLoadPrompt()
    {
        if (saveLoadPanel != null)
            saveLoadPanel.SetActive(false);
    }
    
    public void ShowGameOverScreen(int score, int moves, float gameTime, int highScore)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            if (finalScoreText != null)
            {
                string highScoreText = score >= highScore ? " (NEW HIGH SCORE!)" : "";
                finalScoreText.text = $"Game Complete!\n\n" +
                                     $"Final Score: {score}{highScoreText}\n" +
                                     $"Moves: {moves}\n" +
                                     $"Time: {Mathf.FloorToInt(gameTime / 60f):00}:{Mathf.FloorToInt(gameTime % 60f):00}\n" +
                                     $"High Score: {highScore}";
            }
        }
    }
    
    public void HideGameOverScreen()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
    
    public void ShowPauseMenu()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0f; // Pause the game
        }
    }
    
    public void ResumeGame()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1f; // Resume the game
        }
    }
    
    public void SaveAndQuit()
    {
        // Save the current game
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.SaveGame();
        }
        
        // Resume time scale
        Time.timeScale = 1f;
        
        // Hide pause panel
        if (pausePanel != null)
            pausePanel.SetActive(false);
        
        // Show save confirmation
        ShowSaveConfirmation();
    }
    
    private void ShowSaveConfirmation()
    {
        // Create a simple confirmation popup
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (finalScoreText != null)
            {
                finalScoreText.text = "Game Saved!\n\nYou can continue your game when you return.";
            }
        }
    }
    
    private void DeleteSaveData()
    {
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.DeleteSaveData();
            HideSaveLoadPrompt();
            GameManager.Instance.StartNewGame();
        }
    }
    
    private void ShowMainMenu()
    {
        // This would typically load a main menu scene
        // For now, just restart the game
        GameManager.Instance.RestartGame();
    }
    
    public void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }
    
    public void UpdateTimer(float gameTime)
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(gameTime / 60f);
            int seconds = Mathf.FloorToInt(gameTime % 60f);
            timerText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }
    
    public void UpdateMoves(int moves)
    {
        if (movesText != null)
            movesText.text = $"Moves: {moves}";
    }
    
    public void ShowSaveIndicator()
    {
        // Optional: Show a small "Saving..." indicator
        Debug.Log("Game saved!");
    }
} 