using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class TowerTargeting
{
    public enum TargetType
    {
        First,
        Last,
        Close,
        Strong
    }

    public static Enemy GetTarget(TowerBehavior currentTower, TargetType targetMethod)
    {
        // Get all enemies in range using OverlapSphere
        Collider[] enemiesInRange = Physics.OverlapSphere(currentTower.transform.position, currentTower.range,
            currentTower.EnemiesLayer);

        if (enemiesInRange.Length == 0) return null;

        List<Enemy> validEnemies = new List<Enemy>();
        // Extract enemy components
        for (int i = 0; i < enemiesInRange.Length; i++)
        {
            Enemy currentEnemy = enemiesInRange[i].GetComponent<Enemy>();

            if (currentEnemy != null)
            {
                validEnemies.Add(currentEnemy);
            }
            else
            {
                Debug.Log($"No Enemy component on object: {enemiesInRange[i].gameObject.name}");
            }
        }


        if (validEnemies.Count == 0) return null;

        // Apply targeting strategy
        switch (targetMethod)
        {
            case TargetType.First: return GetFirstEnemy(validEnemies);
            case TargetType.Last: return GetLastEnemy(validEnemies);
            case TargetType.Close: return GetClosestEnemy(validEnemies, currentTower.transform.position);
            case TargetType.Strong: return GetStrongestEnemy(validEnemies);
            default: return validEnemies[0];
        }
    }

    private static Enemy GetFirstEnemy(List<Enemy> enemies)
    {
        Enemy furthestAlongPath = enemies[0];
        int highestNodeIndex = furthestAlongPath.currentPathIndex;
        foreach (Enemy enemy in enemies)
        {
            // higher node index means further along the path
            if (enemy.currentPathIndex >= highestNodeIndex)
            {
                furthestAlongPath = enemy;
                highestNodeIndex = enemy.currentPathIndex;
            }
        }

        return furthestAlongPath;
    }

    private static Enemy GetLastEnemy(List<Enemy> enemies)
    {
        Enemy earliestInPath = enemies[0];
        int lowestNodeIndex = earliestInPath.currentPathIndex;

        foreach (Enemy enemy in enemies)
        {
            if (enemy.currentPathIndex < lowestNodeIndex)
            {
                earliestInPath = enemy;
                lowestNodeIndex = enemy.currentPathIndex;
            }
        }

        return earliestInPath;
    }

    private static Enemy GetClosestEnemy(List<Enemy> enemies, Vector3 towerPosition)
    {
        Enemy closestEnemy = enemies[0];
        float closestDistance = Vector3.Distance(towerPosition, closestEnemy.transform.position);

        foreach (Enemy enemy in enemies)
        {
            float distance = Vector3.Distance(towerPosition, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestEnemy = enemy;
                closestDistance = distance;
            }
        }

        return closestEnemy;
    }

    private static Enemy GetStrongestEnemy(List<Enemy> enemies)
    {
        Enemy strongestEnemy = enemies[0];
        float highestHealth = strongestEnemy.health;

        foreach (Enemy enemy in enemies)
        {
            if (enemy.health > highestHealth)
            {
                strongestEnemy = enemy;
                highestHealth = enemy.health;
            }
        }

        return strongestEnemy;
    }
}