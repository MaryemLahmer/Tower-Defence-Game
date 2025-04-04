using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth, health, speed;
    public int id;
    private List<Vector2Int> pathCells;
    int nextPathCellIndex;
    bool enemyRunCompleted;
    public void Init()
    {
        health = maxHealth;
    }
    void Start()
    {
        nextPathCellIndex = 1;
        enemyRunCompleted = false;
    }
/*
    void Update()
    {
        if (pathCells != null && pathCells.Count > 1 && !enemyRunCompleted)
        {
            Vector3 currentPos = transform.position;
            Vector3 nextPos =  new Vector3(pathCells[nextPathCellIndex].x, 0.2f, pathCells[nextPathCellIndex].y);
            transform.position = Vector3.MoveTowards(currentPos, nextPos, Time.deltaTime );
            if (Vector3.Distance(currentPos, nextPos) < 0.05f) {
                nextPathCellIndex++;
                if (nextPathCellIndex >= pathCells.Count)
                {
                    enemyRunCompleted = true;
                }
            }
        }
    }
    */

}
