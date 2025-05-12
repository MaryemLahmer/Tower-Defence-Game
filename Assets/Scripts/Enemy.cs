using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth ;
    public float health ;
    public float speed ;
    public int id;
    public int reward;
    [Header("Configuration")]
    public EnemySummonData enemyData;

    [Header("Path Following")]
    private List<Vector2Int> pathCells;
    public int currentPathIndex = 0;
    private bool pathCompleted = false;

    private Vector3 currentMoveDirection;   

    private float rotationSpeed = 8f;
    
    [Header("Visual Feedback")]
    [SerializeField] private GameObject healthBarObject;
    [SerializeField] private Transform healthBarFill;
    // Add this method to your Enemy class
public void InitWithData(EnemySummonData data)
{
    if (data == null)
    {
        Debug.LogError("Tried to initialize enemy with null data!");
        return;
    }
    
    // Store reference
    enemyData = data;
    
    // Set stats from scriptable object
    id = data.enemyId;
    maxHealth = data.health;
    health = data.health;
    speed = data.speed;
    
    // Reset path following
    currentPathIndex = 0;
    pathCompleted = false;
    
    // Update visuals
    UpdateHealthBar();
    
    // Make sure object is active
    gameObject.SetActive(true);
    
    Debug.Log($"Enemy initialized with ID: {id}, Health: {health}, Speed: {speed}");
}
    // Events
    public UnityEvent<Enemy> OnDefeat = new UnityEvent<Enemy>();
    public UnityEvent<Enemy> OnLeak = new UnityEvent<Enemy>();
    
    public void Init()
    {
        // Find corresponding data based on ID
    // We need to get the data from somewhere!
    EnemySummonData data = EntitySummoner.GetEnemyData(id);
    
    if (data != null)
    {
        // Store reference to data
        enemyData = data;
        
        // Set stats from data
        maxHealth = data.health;
        health = data.health;
        speed = data.speed;
        reward = data.reward;
        Debug.Log($"Initialized enemy ID {id} with health={health}, speed={speed}");
    }
    else
    {
        Debug.LogWarning($"CRITICAL: No data found for enemy ID {id}, creating fallback data");
        
        // Create temporary data to avoid null references
        enemyData = ScriptableObject.CreateInstance<EnemySummonData>();
        enemyData.enemyId = id;
        enemyData.health = 100;
        enemyData.speed = 2;
        enemyData.reward = 5;
        enemyData.score = 10;
        // Fallback to prevent errors
        maxHealth = 100;
        health = 100;
        speed = 2;
        reward = 10;
    }
    // Reset path following
    currentPathIndex = 0;
    pathCompleted = false;
    
    // Reset health bar
    if (healthBarObject != null)
    {
        healthBarObject.SetActive(false);
    }
    
    // Make sure object is active
    gameObject.SetActive(true);
    }

    private EnemySummonData GetEnemyDataById(int enemyId)
{
    // Look in Resources folder
    EnemySummonData[] allEnemies = Resources.LoadAll<EnemySummonData>("Enemies");
    foreach (var enemyData in allEnemies)
    {
        if (enemyData.enemyId == enemyId)
        {
            return enemyData;
        }
    }
    return null;
}
    
    // Set the path for this enemy to follow
    public void SetPath(List<Vector2Int> path)
    {
        if (path == null || path.Count < 2)
        {
            return;
        }
        
        pathCells = new List<Vector2Int>(path); // Make a copy to avoid reference issues
        currentPathIndex = 1; // Start with the second point (index 1)
        pathCompleted = false;
        
        // Position enemy at first waypoint
        transform.position = new Vector3(path[0].x, 0.2f, path[0].y);

        // Initialize movement direction toward first waypoint
    Vector3 firstWaypointPos = new Vector3(path[1].x, 0.2f, path[1].y);
    currentMoveDirection = (firstWaypointPos - transform.position).normalized;
    transform.rotation = Quaternion.LookRotation(currentMoveDirection);
    }
    
    void Update()
    {
        // Only move if we have a path and haven't reached the end
        if (pathCells != null && pathCells.Count > 1 && !pathCompleted && currentPathIndex < pathCells.Count)
        {
            MoveAlongPath();
        }
    }
    
private void MoveAlongPath()
{
    // Get target waypoint
    Vector3 targetPosition = new Vector3(
        pathCells[currentPathIndex].x, 
        0.2f, 
        pathCells[currentPathIndex].y
    );
    
    // Calculate direction to target
    Vector3 directionToTarget = (targetPosition - transform.position).normalized;
    if (directionToTarget != Vector3.zero)
    {
        // Update current direction
        currentMoveDirection = directionToTarget;
        
        // Smoothly rotate towards the movement direction
        Quaternion targetRotation = Quaternion.LookRotation(currentMoveDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    
    // Move towards target (unchanged)
    float step = speed * Time.deltaTime;
    transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
    
    // Check if reached waypoint (unchanged)
    if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
    {
        // Move to next waypoint
        currentPathIndex++;
        
        // Check if we've reached the end
        if (currentPathIndex >= pathCells.Count)
        {   
            Debug.Log($"Enemy {id} reached end of path. Has enemyData: {enemyData != null}");
            // Path completed
            pathCompleted = true;

            if (enemyData == null)
            {
                Debug.LogWarning($"Creating emergency enemyData for enemy {id} before leak event");
                enemyData = ScriptableObject.CreateInstance<EnemySummonData>();
                enemyData.score = 10;
                enemyData.enemyId = id;
            }
            // Play leak sound
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayEnemyLeakSound();
            }
            OnLeak.Invoke(this);
            EntitySummoner.RemoveEnemey(this);
        }
    }
}
    
    void ReachedEndOfPath()
    {
        pathCompleted = true;
        
        // Enemy reached the end - trigger leak event
        OnLeak.Invoke(this);
        
        // Return to pool
        EntitySummoner.RemoveEnemey(this);
    }
    
    // Take damage and check if health <= 0
    public void TakeDamage(float damageAmount)
{
    // Check if already dead or inactive
    if (health <= 0 || !gameObject.activeInHierarchy)
    {
        return; // Don't apply damage to dead/inactive enemies
    }

    health -= damageAmount;
    
    // Update health bar
    UpdateHealthBar();
    
    // Visual feedback (only if active)
    if (gameObject.activeInHierarchy)
    {
        StartCoroutine(FlashDamage());
    }
    
    // Check if enemy is defeated
    if (health <= 0)
    {
        Die();
    }
}
    
    void Die()
{
    // Play death sound
    if (SoundManager.Instance != null)
    {
        SoundManager.Instance.PlayEnemyDefeatedSound();
    }
    
    // Trigger defeat event (for score, etc.)
    OnDefeat.Invoke(this);
    
    // Return to object pool
    EntitySummoner.RemoveEnemey(this);
}
    
    // Visual feedback when taking damage
    IEnumerator FlashDamage()
    {
        // Get all renderers
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        
        // Store original colors
        List<Color> originalColors = new List<Color>();
        foreach (Renderer renderer in renderers)
        {
            if (renderer.material != null)
            {
                originalColors.Add(renderer.material.color);
                renderer.material.color = Color.red;
            }
        }
        
        // Wait a moment
        yield return new WaitForSeconds(0.1f);
        
        // Restore original colors
        for (int i = 0; i < renderers.Length; i++)
        {
            if (i < originalColors.Count && renderers[i].material != null)
            {
                renderers[i].material.color = originalColors[i];
            }
        }
    }
    
    // Update the health bar visualization
    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            float healthPercent = Mathf.Clamp01(health / maxHealth);
            healthBarFill.localScale = new Vector3(healthPercent, 1, 1);
        }
        
        // Show/hide health bar based on damage
        if (healthBarObject != null)
        {
            healthBarObject.SetActive(health < maxHealth);
        }
    }
}