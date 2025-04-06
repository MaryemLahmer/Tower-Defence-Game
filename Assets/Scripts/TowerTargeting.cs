using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class TowerTargeting
{
    public enum TargetType
    {
        First,
        Last,
        Close
    }

    public static Enemy GetTarget(TowerBehavior currentTower, TargetType targetMethod)
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(currentTower.transform.position, currentTower.range,
            currentTower.EnemiesLayer);
        NativeArray<EnemyData> enemiesToCalculte = new NativeArray<EnemyData>();
        
        
        for (int i = 0; i < enemiesToCalculte.Length; i++)
        {
            Enemy currentEnemey = enemiesInRange[i].GetComponent<Enemy>();
            enemiesToCalculte[i] = new EnemyData(currentEnemey.transform.position, currentEnemey.currentPathIndex,
                currentEnemey.health);
            
        }

        return null;
    }

    struct EnemyData
    {
        public EnemyData(Vector3 position, int np, float h)
        {
            enemyPosition = position;
            nodeIndex = np;
            health = h;
        }

        public Vector3 enemyPosition;
        public int nodeIndex;
        public float health;
    }
}