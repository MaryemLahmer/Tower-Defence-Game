using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySummoner : MonoBehaviour
{
    public static List<Enemy> enemiesAlive;
    public static Dictionary<int, GameObject> enemyPrefabs;
    public static Dictionary<int, Queue<Enemy>> enemyobjectPools;

    public static bool isInialized;

    public static event System.Action<Enemy> OnEnemySpawned;
    private static Dictionary<int, EnemySummonData> enemyDataCache = new Dictionary<int, EnemySummonData>();
    public static void Init()
    {
        if (!isInialized)
        {   
            enemyPrefabs = new Dictionary<int, GameObject>();
            enemyobjectPools = new Dictionary<int, Queue<Enemy>>();
            enemiesAlive = new List<Enemy>();
            enemyDataCache = new Dictionary<int, EnemySummonData>();
        
            EnemySummonData[] enemies = Resources.LoadAll<EnemySummonData>("Enemies");
        
            foreach (EnemySummonData enemy in enemies)
            {
                enemyPrefabs.Add(enemy.enemyId, enemy.enemeyPrefab);
                enemyobjectPools.Add(enemy.enemyId, new Queue<Enemy>());
                enemyDataCache.Add(enemy.enemyId, enemy);

            }

            isInialized = true;
        }

    }

    public static EnemySummonData GetEnemyData(int enemyId)
{
    if (enemyDataCache.ContainsKey(enemyId))
    {
        return enemyDataCache[enemyId];
    }
    
    Debug.LogWarning($"Enemy data for ID {enemyId} not found in cache");
    return null;
}

    public static Enemy SummonEnemy(int enemyID)
    {   
        Enemy summonedEnemy = null;
        if (enemyobjectPools.ContainsKey(enemyID))
        {
            Queue<Enemy> referencedQueue = enemyobjectPools[enemyID];
            if (referencedQueue.Count > 0)
            {
                // dequeue enemy and initialize
                summonedEnemy = referencedQueue.Dequeue();
                summonedEnemy.id = enemyID;
                summonedEnemy.Init();
            }
            else
            {
                // instantiate new enemy and initialize
                GameObject newEnemy = Instantiate(enemyPrefabs[enemyID], new Vector3(0, 0.2f, 5f), enemyPrefabs[enemyID].transform.rotation);
                summonedEnemy = newEnemy.GetComponent<Enemy>();
                summonedEnemy.id = enemyID;
                summonedEnemy.Init();
            }
        enemiesAlive.Add(summonedEnemy);
        OnEnemySpawned?.Invoke(summonedEnemy);
        return summonedEnemy;
        }
        else
        {
            Debug.Log("enemy id not found in dictionary");
            return null;
        }
        
       
    /*   enemiesAlive.Add(summonedEnemy);
        summonedEnemy.id = enemyID;
        if (summonedEnemy != null)
    {
        OnEnemySpawned?.Invoke(summonedEnemy);
    }
        return summonedEnemy; */
    }

    public static void RemoveEnemey(Enemy enemyToRemove)
    {   
        int enemyId = enemyToRemove.id;
        if (!enemyobjectPools.ContainsKey(enemyId))
        {
            Debug.LogError($"Trying to remove enemy with ID {enemyId} but no pool exists");
            Destroy(enemyToRemove.gameObject);
            return;
        }
        enemyobjectPools[enemyToRemove.id].Enqueue(enemyToRemove);
        enemyToRemove.gameObject.SetActive(false);
        enemiesAlive.Remove(enemyToRemove);
    }
    // Add to your EntitySummoner class
public static void ClearAllEntities()
{
    // Clear all lists/collections of entities
    if (enemiesAlive != null)
        enemiesAlive.Clear();
        
    // Reset any other static variables here
}
}