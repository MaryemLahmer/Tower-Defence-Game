using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static Queue<int> enemyIDsToSummon;
    public bool loopShouldEnd;
    private EnemyWaveManager waveManager;

    void Start()
    {
        waveManager = GetComponent<EnemyWaveManager>();
        enemyIDsToSummon = new Queue<int>();
        EntitySummoner.Init();
        StartCoroutine(GameLoop());
        InvokeRepeating("SummonTest", 0, 1);
    }

    void SummonTest()
    {
        EnqueueEnemyIDToSummon(1);
    }

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
}