using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static Queue<int> enemyIDsToSummon;
    public bool loopShouldEnd;
    private EnemyWaveManager waveManager;
    private WaveManager phaseManager;

    void Start()
    {
        waveManager = GetComponent<EnemyWaveManager>();
        phaseManager = FindObjectOfType<WaveManager>();
        enemyIDsToSummon = new Queue<int>();
        EntitySummoner.Init();
        StartCoroutine(GameLoop());
        
        // Remove the test summoning since we'll use the wave manager now
        // InvokeRepeating("SummonTest", 0, 1);
        
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
    }

    // This can be removed since we're using the WaveManager to spawn enemies
    /*
    void SummonTest()
    {
        EnqueueEnemyIDToSummon(1);
    }
    */

    IEnumerator GameLoop()
    {
        while (!loopShouldEnd)
        {
            if (enemyIDsToSummon.Count > 0)
            {
                // spawn Enemies 
                for (int i = 0; i < enemyIDsToSummon.Count; i++)
                {
                    Enemy summonedEnemey = EntitySummoner.SummonEnemy(enemyIDsToSummon.Dequeue());
                    waveManager.MoveEnemy(summonedEnemey);
                }
            }

            //spawn towers
            // move enemies
            // tick towers
            // apply effects
            // damage enemies
            // remove enemies 
            // remove towers
            yield return null;
        }
    }

    public static void EnqueueEnemyIDToSummon(int ID)
    {
        enemyIDsToSummon.Enqueue(ID);
    }
    
    // Add a method to notify when all enemies are cleared
    public void NotifyAllEnemiesCleared()
    {
        // This would be called when all enemies from a wave are dead or reached the end
        // It will help the WaveManager know if we need to end a wave early
    }
}