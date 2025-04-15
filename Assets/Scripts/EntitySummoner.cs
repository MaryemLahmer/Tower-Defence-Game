using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySummoner : MonoBehaviour
{
    public static List<Enemy> enemiesAlive;
    public static Dictionary<int, GameObject> enemyPrefabs;
    public static Dictionary<int, Queue<Enemy>> enemyobjectPools;

    public static bool isInialized;

    public static void Init()
    {
        if (!isInialized)
        {
            enemyPrefabs = new Dictionary<int, GameObject>();
            enemyobjectPools = new Dictionary<int, Queue<Enemy>>();
            enemiesAlive = new List<Enemy>();
        
            EnemySummonData[] enemies = Resources.LoadAll<EnemySummonData>("Enemies");
        
            foreach (EnemySummonData enemy in enemies)
            {
                enemyPrefabs.Add(enemy.enemyId, enemy.enemeyPrefab);
                enemyobjectPools.Add(enemy.enemyId, new Queue<Enemy>());
                Debug.Log($"Registered enemy ID {enemy.enemyId}");

            }

            isInialized = true;
        }
        Debug.Log("EntitySummoner initialized.");

    }

    public static Enemy SummonEnemy(int enemyID)
    {   
        Debug.Log($"Summoned enemy {enemyID}");
        Enemy summonedEnemy = null;
        if (enemyobjectPools.ContainsKey(enemyID))
        {
            Queue<Enemy> referencedQueue = enemyobjectPools[enemyID];
            if (referencedQueue.Count > 0)
            {
                // dequeue enemy and initialize
                summonedEnemy = referencedQueue.Dequeue();
                summonedEnemy.Init();
            }
            else
            {
                // instantiate new enemy and initialize
                GameObject newEnemy = Instantiate(enemyPrefabs[enemyID], new Vector3(0, 0.2f, 5f), enemyPrefabs[enemyID].transform.rotation);
                summonedEnemy = newEnemy.GetComponent<Enemy>();
                summonedEnemy.Init();
            }
        }
        else
        {
            Debug.Log("enemy id not found in dictionary");
            return null;
        }
        
       
        enemiesAlive.Add(summonedEnemy);
        summonedEnemy.id = enemyID;
        return summonedEnemy;
    }

    public static void RemoveEnemey(Enemy enemyToRemove)
    {
        enemyobjectPools[enemyToRemove.id].Enqueue(enemyToRemove);
        enemyToRemove.gameObject.SetActive(false);
        enemiesAlive.Remove(enemyToRemove);
    }
}