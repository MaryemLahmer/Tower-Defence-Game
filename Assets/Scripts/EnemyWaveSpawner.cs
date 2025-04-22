using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyWaveData
{
    public int enemyID;
    public int baseCount;
    public float spawnDelay;
    public int additionalPerWave;
    [Range(0, 1)]
    public float spawnChance = 1f;
}

public class EnemyWaveSpawner : MonoBehaviour
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
    
    // This method will be called by GridManager to set the path
    public void SetPathCells(List<Vector2Int> path)
    {
        if (path != null && path.Count >= 2)
        {
            pathCells = new List<Vector2Int>(path);
            pathIsReady = true;
            Debug.Log($"Path set with {pathCells.Count} cells. Starting at ({pathCells[0].x}, {pathCells[0].y}) and ending at ({pathCells[pathCells.Count-1].x}, {pathCells[pathCells.Count-1].y})");
        }
        else
        {
            Debug.LogError("Invalid path provided to EnemyWaveSpawner!");
        }
    }
    
    public void StartWaveSpawning()
    {   
        Debug.Log("StartWaveSpawning called.");
        
        // Safety check to make sure path is ready before spawning enemies
        if (!pathIsReady)
        {
            Debug.LogError("Cannot spawn enemies - path not set yet! Enemies will be spawned once path is ready.");
            return;
        }
        
        currentWave++;
        StopAllCoroutines();
        StartCoroutine(SpawnWave());
    }
    
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
            Debug.LogError("Trying to assign path to enemy but path is not ready!");
            
            // Safety measure: destroy the enemy to prevent game freeze
            Destroy(spawnedEnemy.gameObject);
        }
    }
    
    private IEnumerator SpawnWave()
    {
        // Safety check - don't spawn if path isn't ready
        if (!pathIsReady)
        {
            Debug.LogError("Cannot spawn wave - path not set!");
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
                            // IMPORTANT: Assign path to enemy for movement
                            AssignPathToEnemy(spawnedEnemy);
                            
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
    }
    
    // Rest of your methods (CalculateTotalEnemies, CalculateEnemyCount, etc.) remain the same
    
    // Public method to check if all enemies have been cleared (for wave completion)
    public bool AreAllEnemiesCleared()
    {
        return EntitySummoner.enemiesAlive.Count == 0;
    }
    
    // Call this to manually spawn a test enemy
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
            // Assign path to test enemy
            AssignPathToEnemy(enemy);
            Debug.Log($"Test enemy {enemyID} spawned and path assigned!");
        }
    }
    private int CalculateTotalEnemies()
{
    int totalForWave = 0;
    
    // Calculate based on enemy types
    foreach (EnemyWaveData enemyData in enemyTypes)
    {
        int countForThisType = CalculateEnemyCount(enemyData);
        totalForWave += countForThisType;
    }
    
    // Add base enemies (optional - remove if you only want to use enemy types)
    int baseEnemiesForWave = baseEnemyCount + (currentWave - 1) * additionalEnemiesPerWave;
    totalForWave += baseEnemiesForWave;
    
    return totalForWave;
}
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
    private EnemyWaveData GetEnemyDataByID(int enemyID)
{
    if (enemyTypes == null)
    {
        Debug.LogError("No enemy types defined!");
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
    // Visualize the path in the editor - simplified for runtime-generated paths
    private void OnDrawGizmos()
    {
        if (pathCells != null && pathCells.Count > 1 && pathIsReady)
        {
            // Draw path points
            for (int i = 0; i < pathCells.Count; i++)
            {
                // Start point (green)
                if (i == 0)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(new Vector3(pathCells[i].x, 0.2f, pathCells[i].y), 0.3f);
                }
                // End point (red)
                else if (i == pathCells.Count - 1)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(new Vector3(pathCells[i].x, 0.2f, pathCells[i].y), 0.3f);
                }
                // Waypoints (yellow)
                else
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(new Vector3(pathCells[i].x, 0.2f, pathCells[i].y), 0.2f);
                }
                
                // Draw path lines
                if (i < pathCells.Count - 1)
                {
                    Gizmos.color = Color.yellow;
                    Vector3 start = new Vector3(pathCells[i].x, 0.2f, pathCells[i].y);
                    Vector3 end = new Vector3(pathCells[i+1].x, 0.2f, pathCells[i+1].y);
                    Gizmos.DrawLine(start, end);
                }
            }
        }
    }
}