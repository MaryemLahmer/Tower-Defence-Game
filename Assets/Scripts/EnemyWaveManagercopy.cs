using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveManager : MonoBehaviour
{
    public GameObject enemyObject;
    private List<Vector2Int> pathCells;
    private GameObject enemyInstance;
    int nextPathCellIndex;
    bool enemyRunCompleted;

    // Start is called before the first frame update
    void Start()
    {
        enemyInstance = Instantiate(enemyObject, new Vector3(0, 0.2f, 5f), Quaternion.identity);
        nextPathCellIndex = 1;
        enemyRunCompleted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (pathCells != null && pathCells.Count > 1 && !enemyRunCompleted)
        {
            Vector3 currentPos = enemyInstance.transform.position;
            Vector3 nextPos =  new Vector3(pathCells[nextPathCellIndex].x, 0.2f, pathCells[nextPathCellIndex].y);
            enemyInstance.transform.position = Vector3.MoveTowards(currentPos, nextPos, Time.deltaTime * 2);
            if (Vector3.Distance(currentPos, nextPos) < 0.05f) {
                nextPathCellIndex++;
                if (nextPathCellIndex >= pathCells.Count)
                {
                    Debug.Log("Reached end");
                    enemyRunCompleted = true;

                }else
                {
                    nextPathCellIndex++;
                }
            }
        }
    }

    public void SetPathCells(List<Vector2Int> pathCells)
    {
        this.pathCells = pathCells;
    }
}
