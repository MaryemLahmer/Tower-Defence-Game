using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    // Singleton pattern
    private static PauseManager _instance;
    public static PauseManager Instance { get { return _instance; } }
    
    // Game state tracking
    public static bool IsGamePaused { get; private set; } = false;
    public static bool GameInProgress { get; private set; } = false;
    
    // Saved game state
    private int currentScore;
    private int currentMoney;
    private int currentWave;
    private int currentEnemiesRemaining;
    
    private void Awake()
    {
        // Singleton setup
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    private void Update()
    {
        // Check for Escape key press in game scene
        if (SceneManager.GetActiveScene().name == "Main" && Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }
    
    public void StartNewGame()
    {
        // Reset game state
        IsGamePaused = false;
        GameInProgress = true;
        
        // Load game scene
        SceneManager.LoadScene("Main");
    }
    
    public void PauseGame()
    {
        if (SceneManager.GetActiveScene().name != "Main") return;
        
        // Pause the actual game logic
        Time.timeScale = 0f;
        
        // Save current game state
        SaveGameState();
        
        // Mark game as paused
        IsGamePaused = true;
        
        // Return to menu
        SceneManager.LoadScene("Menu");
    }
    
    public void ResumeGame()
    {
        // Reset pause state
        IsGamePaused = false;
        
        // Restore normal time
        Time.timeScale = 1f;
        
        // Load game scene
        SceneManager.LoadScene("Main");
    }
    
    private void SaveGameState()
    {
        // Get references to game objects that contain state
        TowerDefenseUI ui = FindObjectOfType<TowerDefenseUI>();
        if (ui != null)
        {
            // Store relevant game state
            string scoreText = ui.scoreText.text;
            string moneyText = ui.moneyText.text;
            string waveText = ui.waveNumberText.text;
            string enemiesText = ui.enemiesRemainingText.text;
            
            // Parse values (handling the text formatting)
            currentScore = int.Parse(scoreText.Replace("Score: ", ""));
            currentMoney = int.Parse(moneyText.Replace(" $", ""));
            currentWave = int.Parse(waveText.Replace("Wave: ", ""));
            currentEnemiesRemaining = int.Parse(enemiesText.Replace("Enemies: ", ""));
        }
        
        // NOTE: For a complete implementation, you would save more game state:
        // - Tower positions and upgrades
        // - Current wave progress
        // - Enemy positions and health
    }
    
    public void RestoreGameState()
    {
        if (!IsGamePaused) return;
        
        // Get UI references to restore values
        TowerDefenseUI ui = FindObjectOfType<TowerDefenseUI>();
        if (ui != null)
        {
            // Restore UI state
            ui.UpdateScore(currentScore);
            ui.UpdateMoney(currentMoney);
            ui.UpdateWaveNumber(currentWave);
            ui.UpdateEnemiesRemaining(currentEnemiesRemaining);
        }
        
        // NOTE: You would also restore:
        // - Recreate towers with saved positions and upgrades
        // - Restore enemy positions
    }
    
    public void QuitToMainMenu()
    {
        // Reset all states
        IsGamePaused = false;
        GameInProgress = false;
        Time.timeScale = 1f;
        
        // Return to menu as a new game
        SceneManager.LoadScene("Menu");
    }
}