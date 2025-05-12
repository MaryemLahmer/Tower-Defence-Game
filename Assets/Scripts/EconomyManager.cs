using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    [Header("Economy Settings")]
    [SerializeField] private int startingMoney = 200;
    
    [Header("Multiplier Settings")]
    [SerializeField] private int startingMultiplier = 1;
    [SerializeField] private int maxMultiplier = 10;
    [SerializeField] private int minMultiplier = -10;
    [SerializeField] private int killsForMultiplierIncrease = 1;
    [SerializeField] private int leaksForMultiplierDecrease = 1;
    
    [Header("References")]
    [SerializeField] private TowerDefenseUI uiManager;
    
    private int currentScore = 0;
    private int currentMoney = 0;
    private int currentMultiplier = 1;
    private int consecutiveKills = 0;
    private int consecutiveLeaks = 0;
    
    // Singleton pattern
    public static EconomyManager Instance { get; private set; }
    
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
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<TowerDefenseUI>();
        }
        
        currentMoney = startingMoney;
        currentMultiplier = startingMultiplier;
        
        UpdateUI();
        
        RegisterEnemyEvents();
    }
    
    // Register for enemy events when they're spawned
    public void RegisterEnemyEvents()
    {
        EntitySummoner.OnEnemySpawned += OnEnemySpawned;
    }
    
    private void OnEnemySpawned(Enemy enemy)
    {
        if (enemy != null)
        {
            enemy.OnDefeat.AddListener(OnEnemyDefeated);
            enemy.OnLeak.AddListener(OnEnemyLeak);
            
        }
    }
    
    // Called when an enemy is defeated
    private void OnEnemyDefeated(Enemy enemy)
    {
        
        consecutiveLeaks = 0;
        
        consecutiveKills++;
        
        if (consecutiveKills >= killsForMultiplierIncrease)
        {
            consecutiveKills = 0;
            IncreaseMultiplier(1);
        }
        
        if (currentMultiplier < 0)
        {
            currentMultiplier = 1;
        }
        
        // Apply score and money rewards
        int scoreReward = enemy.enemyData.score * currentMultiplier;
        AddScore(scoreReward);
        AddMoney(enemy.enemyData.reward);
        
        
        UpdateUI();
    }
    
    private void OnEnemyLeak(Enemy enemy)
    {
            if (enemy == null)
        {
            return;
        }
        
        // Reset consecutive kills
        consecutiveKills = 0;
        
        // Increase consecutive leaks
        consecutiveLeaks++;
        bool multiplierChanged = false;
        // Check if multiplier should decrease
        if (consecutiveLeaks >= leaksForMultiplierDecrease)
        {
            consecutiveLeaks = 0;
            multiplierChanged = true;
            DecreaseMultiplier(1);
        }
        
        // If multiplier is positive, reset to -1
        if (currentMultiplier > 0)
        {
            currentMultiplier = -1;
            multiplierChanged = true;
        }
      
       int scorePenalty = 10; // Default penalty if enemyData is null
    
    // Add this check to prevent null reference
    if (enemy != null && enemy.enemyData != null)
    {
        scorePenalty = enemy.enemyData.score * Mathf.Abs(currentMultiplier);
        Debug.Log($"Using score from enemyData: {enemy.enemyData.score}");
    }
  
    
    AddScore(-scorePenalty);
        
        UpdateUI();
    }
    
    public void AddScore(int amount)
    {
        currentScore = Mathf.Max(0, currentScore + amount);
    }
    
    // Add to current money
    public void AddMoney(int amount)
    {
        currentMoney = Mathf.Max(0, currentMoney + amount);
    }
    
    // Check if player can afford cost
    public bool CanAfford(int cost)
    {
        return currentMoney >= cost;
    }
    
    // Try to purchase something
    public bool TryPurchase(int cost)
    {
        if (CanAfford(cost))
        {
            currentMoney -= cost;
            UpdateUI();
            return true;
        }
        return false;
    }
    
    // Increase multiplier
    private void IncreaseMultiplier(int amount)
    {
        currentMultiplier = Mathf.Min(maxMultiplier, currentMultiplier + amount);
    }
    
    // Decrease multiplier
    private void DecreaseMultiplier(int amount)
    {
        currentMultiplier = Mathf.Max(minMultiplier, currentMultiplier - amount);
    }
    
    // Update all UI elements
    private void UpdateUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateScore(currentScore);
            uiManager.UpdateMoney(currentMoney);
            uiManager.UpdateMultiplier(currentMultiplier);
        }
    }
    
  
    public bool TryPurchaseTower(int towerIndex)
    {
        // Define tower costs (should be retrieved from tower data)
        int[] towerCosts = { 80, 110, 200, 250 };
        
        if (towerIndex < 0 || towerIndex >= towerCosts.Length)
        {
            return false;
        }
        
        int cost = towerCosts[towerIndex];
        
        if (TryPurchase(cost))
        {
            return true;
        }
        
        return false;
    }
    
    /* Upgrade tower logic
    public bool TryUpgradeTower(Tower tower)
    {
        if (tower == null)
            return false;
            
        int upgradeCost = tower.GetUpgradeCost();
        
        if (TryPurchase(upgradeCost))
        {
            tower.Upgrade();
            return true;
        }
        
        return false;
    } */
    
    // Cleanup
    private void OnDestroy()
    {
        EntitySummoner.OnEnemySpawned -= OnEnemySpawned;
    }
}