using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveManager : MonoBehaviour
{
    [Header("Path Settings")]
    private List<Vector2Int> pathCells = new List<Vector2Int>();
    private bool pathIsReady = false;

    [Header("Enemy Configuration")]
    [SerializeField] private EnemyWaveData[] enemyTypes;
    
    [Header("Wave Settings")]
    [SerializeField] private int baseEnemyCount = 10;
    [SerializeField] private int additionalEnemiesPerWave = 3;
    [SerializeField] private float spawnDelayReductionPerWave = 0.05f;
    
    [Header("Spawn Settings")]
    [SerializeField] private float initialSpawnDelay = 1f;
    [SerializeField] private float minSpawnDelay = 0.2f;
    
    // Runtime variables
    private int currentWave = 0;
    private int enemiesRemaining = 0;
    private float waveProgress = 0f;
    private TowerDefenseUI uiManager;
    private WaveManager phaseManager;
    
    // Static collection for managing towers
    public static List<TowerBehavior> towersInGame = new List<TowerBehavior>();
    
    void Start()
    {
        // Ensure EntitySummoner is initialized
        if (!EntitySummoner.isInialized)
        {
            EntitySummoner.Init();
        }
        
        uiManager = FindObjectOfType<TowerDefenseUI>();
        phaseManager = FindObjectOfType<WaveManager>();
        
        // Register with WaveManager if it exists
        if (phaseManager != null)
        {
            phaseManager.onWavePhaseStart.AddListener(StartWaveSpawning);
            phaseManager.onWaveCompleted.AddListener(OnWaveCompleted);
        }
        
        // Start the tower processing loop
        StartCoroutine(TowerProcessingLoop());
    }
    
    // Process towers every frame
    private IEnumerator TowerProcessingLoop()
    {
        while (true)
        {
            for (int i = 0; i < towersInGame.Count; i++)
            {
                TowerBehavior tower = towersInGame[i];
                if (tower != null)
                {
                    tower.Tick();
                }
                else
                {
                    // Remove null towers from the list
                    towersInGame.RemoveAt(i);
                    i--; 
                }
            }
            
            yield return null;
        }
    }
    
    public void SetPathCells(List<Vector2Int> path)
    {
        if (path != null && path.Count >= 2)
        {
            pathCells = new List<Vector2Int>(path);
            pathIsReady = true;
        }
    }
    
    public List<Vector2Int> GetPathCells()
    {
        return pathCells;
    }
    
    // Start a new wave of enemies
    public void StartWaveSpawning()
    {   
        if (!pathIsReady)
        {
            return;
        }
        
        currentWave++;
        StopAllCoroutines();
        StartCoroutine(TowerProcessingLoop()); // Restart tower management
        StartCoroutine(SpawnWave());
    }
    
    // Handle wave completion
    private void OnWaveCompleted(int waveNumber)
    {
        Debug.Log($"Wave {waveNumber} completed! Increasing difficulty for next wave.");
    }
    
    // Assign path to a spawned enemy
    private void AssignPathToEnemy(Enemy spawnedEnemy)
    {
        if (spawnedEnemy != null && pathIsReady)
        {
            spawnedEnemy.SetPath(pathCells);
        }
        else if (spawnedEnemy != null)
        {
            // Safety measure: destroy the enemy to prevent game freeze
            Destroy(spawnedEnemy.gameObject);
        }
    }
    
    // Main wave spawning logic
    private IEnumerator SpawnWave()
    {
        if (!pathIsReady)
        {
            yield break;
        }
        
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
            
            foreach (var enemyID in new List<int>(enemyCounts.Keys))
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
                            // Ensure enemy has health and speed set
                            spawnedEnemy.Init();
                            
                            // Assign path to enemy for movement
                            AssignPathToEnemy(spawnedEnemy);
                            
                            // Hook up events for enemy defeat/leaking
                            SetupEnemyEvents(spawnedEnemy);
                            
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
        
    }
    
    private void SetupEnemyEvents(Enemy enemy)
    {
        // Clear previous listeners if any
        enemy.OnDefeat.RemoveAllListeners();
        enemy.OnLeak.RemoveAllListeners();
        
        // Add new listeners
        enemy.OnDefeat.AddListener(OnEnemyDefeated);
        enemy.OnLeak.AddListener(OnEnemyLeaked);
    }
    
    // Handle enemy defeat
    private void OnEnemyDefeated(Enemy enemy)
    {
        // Add score, give resources, etc.
        if (enemy.enemyData != null)
        {
            // Award currency to player
            Debug.Log($"Enemy defeated, awarding {enemy.reward} currency");
            // TODO: Update player currency
        }
        
        CheckWaveCompletion();
    }
    
    // Handle enemy leaking (reaching end of path)
    private void OnEnemyLeaked(Enemy enemy)
    {
        // Reduce player lives
        Debug.Log("Enemy leaked through defenses!");
        // TODO: Reduce player lives/health
        
        CheckWaveCompletion();
    }
    
    // Check if wave is complete
    private void CheckWaveCompletion()
    {
        if (AreAllEnemiesCleared())
        {
            // Notify wave manager
            if (phaseManager != null)
            {
                //GameManager.Instance.NotifyAllEnemiesCleared();
                // phaseManager.NotifyWaveCompleted(currentWave);
                // This would be called if you have a method to notify wave completion
            }
        }
    }
    
    // Check enemy status and notify when all are cleared
    public bool AreAllEnemiesCleared()
    {
        return EntitySummoner.enemiesAlive.Count == 0;
    }
    
    // Spawn a test enemy - useful for debugging
    public void SpawnTestEnemy(int enemyID)
    {
        // Safety check - don't spawn test enemy if path isn't ready
        if (!pathIsReady)
        {
            Debug.LogError("Cannot spawn test enemy - path not set yet!");
            return;
        }
        
        Enemy enemy = EntitySummoner.SummonEnemy(enemyID);
        if (enemy != null)
        {
            enemy.Init();
            AssignPathToEnemy(enemy);
            SetupEnemyEvents(enemy);
            
        }
    }
    
    // Calculate total enemies in a wave
    private int CalculateTotalEnemies()
    {
        int totalForWave = 0;
        
        // Calculate based on enemy types
        foreach (EnemyWaveData enemyData in enemyTypes)
        {
            int countForThisType = CalculateEnemyCount(enemyData);
            totalForWave += countForThisType;
        }
        
        
        return totalForWave;
    }
    
    // Calculate count for a specific enemy type in the current wave
    private int CalculateEnemyCount(EnemyWaveData enemyData)
    {
        // Base count for this enemy type
        int count = enemyData.baseCount;
        
        // Add additional enemies based on wave number
        count += (currentWave - 1) * enemyData.additionalPerWave;
        
        // Some enemy types might not appear until later waves (optional)
        if (enemyData.enemyID > 1 && currentWave < enemyData.enemyID)
        {
            count = 0; // Don't spawn advanced enemies in early waves
        }
        
        return count;
    }
    
    // Find enemy data by ID
    private EnemyWaveData GetEnemyDataByID(int enemyID)
    {
        if (enemyTypes == null)
        {
            return null;
        }
        
        foreach (EnemyWaveData data in enemyTypes)
        {
            if (data.enemyID == enemyID)
                return data;
        }
        
        Debug.LogWarning($"No enemy data found for ID: {enemyID}");
        return null;
    }
    
}
    