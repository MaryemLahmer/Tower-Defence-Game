using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyWaveData
{
    public int enemyID;
    public int baseCount;    // Base number of this enemy type
    public float spawnDelay; // Delay between spawns of this type
    public int additionalPerWave; // How many more to add each wave
    [Range(0, 1)]
    public float spawnChance = 1f; // Chance to spawn (for rare enemies)
}

public class EnemyWaveSpawner : MonoBehaviour
{
    [Header("Enemy Configuration")]
    [SerializeField] private EnemyWaveData[] enemyTypes;
    
    [Header("Wave Settings")]
    [SerializeField] private int baseEnemyCount = 10;         // Starting number of enemies in wave 1
    [SerializeField] private int additionalEnemiesPerWave = 3; // Additional enemies per wave
    [SerializeField] private float spawnDelayReductionPerWave = 0.05f; // Speeds up spawning each wave
    
    [Header("Spawn Settings")]
    [SerializeField] private float initialSpawnDelay = 1f;  // Wait before first enemy
    [SerializeField] private float minSpawnDelay = 0.2f;    // Minimum delay between enemies
    
    // Runtime variables
    private int currentWave = 0;
    private int enemiesRemaining = 0;
    private float waveProgress = 0f;
    private TowerDefenseUI uiManager;
    
    private void Start()
    {
        // Ensure EntitySummoner is initialized
        if (!EntitySummoner.isInialized)
        {
            EntitySummoner.Init();
        }
        
        uiManager = FindObjectOfType<TowerDefenseUI>();
        
        // Find and register with WaveManager if it exists
        WaveManager waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.onWavePhaseStart.AddListener(StartWaveSpawning);
            waveManager.onWaveCompleted.AddListener(OnWaveCompleted);
        }
    }
    
    public void StartWaveSpawning()
    {
        currentWave++;
        StopAllCoroutines();
        StartCoroutine(SpawnWave());
    }
    
    private void OnWaveCompleted(int waveNumber)
    {
        // Wave completed - could add rewards here
        Debug.Log($"Wave {waveNumber} completed! Increasing difficulty for next wave.");
    }
    
    private IEnumerator SpawnWave()
    {
        // Wait before starting spawns
        yield return new WaitForSeconds(initialSpawnDelay);
        
        // Calculate total enemies for this wave
        int totalEnemies = CalculateTotalEnemies();
        enemiesRemaining = totalEnemies;
        waveProgress = 0f;
        
        if (uiManager != null)
        {
            uiManager.UpdateEnemiesRemaining(enemiesRemaining);
        }
        
        Debug.Log($"Starting wave {currentWave} with {totalEnemies} enemies");
        
        // Create dictionaries to track spawn counts and timing for each enemy type
        Dictionary<int, int> enemyCounts = new Dictionary<int, int>();
        Dictionary<int, float> nextSpawnTimes = new Dictionary<int, float>();
        
        // Initialize spawn data
        foreach (EnemyWaveData enemyData in enemyTypes)
        {
            int countForThisWave = CalculateEnemyCount(enemyData);
            if (countForThisWave > 0)
            {
                enemyCounts[enemyData.enemyID] = countForThisWave;
                nextSpawnTimes[enemyData.enemyID] = Time.time;
            }
        }
        
        // Continue spawning until all enemies are spawned
        while (enemyCounts.Count > 0)
        {
            // Check each enemy type
            List<int> completedTypes = new List<int>();
            
            foreach (var enemyID in enemyCounts.Keys)
            {
                // Check if it's time to spawn this enemy type
                if (Time.time >= nextSpawnTimes[enemyID])
                {
                    // Find the corresponding enemy data
                    EnemyWaveData enemyData = GetEnemyDataByID(enemyID);
                    
                    // Calculate actual spawn chance (some enemies might be rare)
                    if (Random.value <= enemyData.spawnChance)
                    {
                        // Spawn the enemy
                        Enemy spawnedEnemy = EntitySummoner.SummonEnemy(enemyID);
                        if (spawnedEnemy != null)
                        {
                            // Decrease remaining count for this type
                            enemyCounts[enemyID]--;
                            enemiesRemaining--;
                            
                            // Update UI
                            if (uiManager != null)
                            {
                                uiManager.UpdateEnemiesRemaining(enemiesRemaining);
                            }
                            
                            // Calculate progress
                            waveProgress = 1f - ((float)enemiesRemaining / totalEnemies);
                        }
                    }
                    
                    // Calculate next spawn time with wave scaling
                    float spawnDelay = Mathf.Max(minSpawnDelay, 
                        enemyData.spawnDelay - (currentWave-1) * spawnDelayReductionPerWave);
                    
                    nextSpawnTimes[enemyID] = Time.time + spawnDelay;
                    
                    // Check if we've spawned all of this type
                    if (enemyCounts[enemyID] <= 0)
                    {
                        completedTypes.Add(enemyID);
                    }
                }
            }
            
            // Remove completed enemy types
            foreach (int completedType in completedTypes)
            {
                enemyCounts.Remove(completedType);
                nextSpawnTimes.Remove(completedType);
            }
            
            yield return null;
        }
        
        Debug.Log($"All enemies for wave {currentWave} have been spawned!");
        
        // Here we could notify a manager that all enemies are spawned
        // But we still need to wait for enemies to be cleared before the wave is truly complete
    }
    
    private int CalculateTotalEnemies()
    {
        // Base count + additional per wave
        int totalForWave = baseEnemyCount + (currentWave - 1) * additionalEnemiesPerWave;
        
        // Add specific enemy counts
        foreach (EnemyWaveData enemyData in enemyTypes)
        {
            totalForWave += CalculateEnemyCount(enemyData);
        }
        
        return totalForWave;
    }
    
    private int CalculateEnemyCount(EnemyWaveData enemyData)
    {
        // Base count for this enemy type
        int count = enemyData.baseCount;
        
        // Add additional enemies based on wave number
        count += (currentWave - 1) * enemyData.additionalPerWave;
        
        // Some enemy types might not appear until later waves
        if (enemyData.enemyID > 1 && currentWave < enemyData.enemyID)
        {
            count = 0; // Don't spawn advanced enemies in early waves
        }
        
        return count;
    }
    
    private EnemyWaveData GetEnemyDataByID(int enemyID)
    {
        foreach (EnemyWaveData data in enemyTypes)
        {
            if (data.enemyID == enemyID)
                return data;
        }
        return null;
    }
    
    // Public method to check if all enemies have been cleared (for wave completion)
    public bool AreAllEnemiesCleared()
    {
        return EntitySummoner.enemiesAlive.Count == 0;
    }
    
    // Call this to manually spawn a test enemy
    public void SpawnTestEnemy(int enemyID)
    {
        Enemy enemy = EntitySummoner.SummonEnemy(enemyID);
        if (enemy != null)
        {
            Debug.Log($"Test enemy {enemyID} spawned!");
        }
    }
}