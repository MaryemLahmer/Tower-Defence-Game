using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveManager : MonoBehaviour
{
    private List<Vector2Int> pathCells;
    private GameObject enemyInstance;
    int nextPathCellIndex;
    bool enemyRunCompleted;

    void Start()
    {
       // enemyInstance = Instantiate(enemyObject, new Vector3(0, 0.2f, 5f), Quaternion.identity);
       nextPathCellIndex = 1;
       enemyRunCompleted = false;
    }

    public void MoveEnemy(Enemy summonedEnemy)
    {
        while (pathCells != null && pathCells.Count > 1 && !enemyRunCompleted)
        {
            Vector3 currentPos = summonedEnemy.transform.position;
            Vector3 nextPos =  new Vector3(pathCells[nextPathCellIndex].x, 0.2f, pathCells[nextPathCellIndex].y);
            summonedEnemy.transform.position = Vector3.MoveTowards(currentPos, nextPos, Time.deltaTime );
            if (Vector3.Distance(currentPos, nextPos) < 0.05f) {
                nextPathCellIndex++;
                if (nextPathCellIndex >= pathCells.Count)
                {
                    enemyRunCompleted = true;
                }
            }
        }
    }
    
    public void SetPathCells(List<Vector2Int> pathCells)
    {
        this.pathCells = pathCells;
    }
   
}
