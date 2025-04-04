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
            // spawn Enemies 
            Debug.Log("enemies to summon: "+enemyIDsToSummon.Count);

            if (enemyIDsToSummon.Count > 0)
            {
                Debug.Log("summoning enemy");
                for (int i = 0; i < enemyIDsToSummon.Count; i++)
                {
                    waveManager.MoveEnemy(EntitySummoner.SummonEnemy(enemyIDsToSummon.Dequeue()));
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