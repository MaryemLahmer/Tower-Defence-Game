using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static Queue<int> enemyIDsToSummon;
    private static Queue<Enemy> enemiesToRemove;
    public static List<TowerBehavior> towersInGame;
    public static float[] nodeDistances;
    public bool loopShouldEnd;
    private EnemyWaveManager waveManager;
    private WaveManager phaseManager;

    void Start()
    {
        waveManager = GetComponent<EnemyWaveManager>();
        enemiesToRemove = new Queue<Enemy>();
        phaseManager = FindObjectOfType<WaveManager>();
        enemyIDsToSummon = new Queue<int>();
        towersInGame = new List<TowerBehavior>();
        EntitySummoner.Init();
        StartCoroutine(GameLoop());
        // Listen for wave phase start
        if (phaseManager != null)
        {
            phaseManager.onWavePhaseStart.AddListener(OnWavePhaseStarted);
        }
    }

    void OnWavePhaseStarted()
    {
        // Wave phase has started, enemies will be spawned by the WaveManager
        // Any additional setup for the wave phase can go here
        EnqueueEnemyIDToSummon(1);
       
    }


    IEnumerator GameLoop()
    {
        while (!loopShouldEnd)
        {
            if (enemyIDsToSummon != null && enemyIDsToSummon.Count > 0)
            {
                // spawn & move Enemies 
                int count = enemyIDsToSummon.Count;
                for (int i = 0; i < count; i++)
                {
                    int enemyId = enemyIDsToSummon.Dequeue();

                    // Check if waveManager is not null before using it
                    if (waveManager != null)
                    {
                        Enemy summonedEnemy = EntitySummoner.SummonEnemy(enemyId);
                        if (summonedEnemy != null)
                        {
                            waveManager.MoveEnemy(summonedEnemy);
                            summonedEnemy.SetPath(waveManager.GetPathCells());
                        }
                    }
                    else
                    {
                        Debug.LogError("waveManager is null in GameLoop");
                    }
                }
            }

            // tick towers - check if towersInGame is not null
            if (towersInGame != null)
            {
                // Use a for loop to avoid issues with null items in the collection
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
                        Debug.LogWarning("Found null tower in towersInGame list, removing it");
                        towersInGame.RemoveAt(i);
                        i--; // Adjust index after removal
                    }
                }
            }

            // Remove enemies with null checks
            if (enemiesToRemove != null && enemiesToRemove.Count > 0)
            {
                int count = enemiesToRemove.Count;
                for (int i = 0; i < count; i++)
                {
                    Enemy enemy = enemiesToRemove.Dequeue();
                    if (enemy != null)
                    {
                        EntitySummoner.RemoveEnemey(enemy);
                    }
                }
            }

            yield return null;
        }
    }

    public static void EnqueueEnemyIDToSummon(int ID)
    {
        enemyIDsToSummon.Enqueue(ID);
    }

    public static void EnqueueEnemeyToRemove(Enemy enemeyToRemove)
    {
        enemiesToRemove.Enqueue(enemeyToRemove);
    }

    // Add a method to notify when all enemies are cleared
    public void NotifyAllEnemiesCleared()
    {
        // This would be called when all enemies from a wave are dead or reached the end
        // It will help the WaveManager know if we need to end a wave early
    }
}