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
        
        //InvokeRepeating("SummonTest", 0, 1);
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
        EnqueueEnemyIDToSummon(1);
        EnqueueEnemyIDToSummon(2);
        EnqueueEnemyIDToSummon(3);
        EnqueueEnemyIDToSummon(4);
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
                    summonedEnemey.SetPath(waveManager.GetPathCells());

                }
            }

            //spawn towers
            // move enemies
            // tick towers
            foreach (TowerBehavior tower in towersInGame)
            {
                tower.target = TowerTargeting.GetTarget(tower, TowerTargeting.TargetType.First);
                tower.Tick();
            }
            
            // apply effects
            // damage enemies
            // remove enemies 
            if (enemiesToRemove.Count > 0)
            {
                for (int i = 0; i < enemiesToRemove.Count; i++)
                {
                    EntitySummoner.RemoveEnemey(enemiesToRemove.Dequeue());
                }
            }

            // remove towers
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