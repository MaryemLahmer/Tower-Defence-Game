using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridWidth = 16;
    public int gridHeight = 8;
    public int minPathLength = 30;

    private EnemyWaveManager waveManager;
    public GridCellObject[] gridCells;
    public GridCellObject[] sceneryCells;
    private PathGenerator _pathGenerator;

    void Start()
    {
        _pathGenerator = new PathGenerator(gridWidth, gridHeight);
        waveManager = GetComponent<EnemyWaveManager>();
        List<Vector2Int> pathCells = _pathGenerator.GenerateEasyPath();
        _pathGenerator.GenerateCrossroads();
        int pathSize = pathCells.Count;

        while (pathSize < minPathLength)
        {
            pathCells = _pathGenerator.GenerateEasyPath();
            
            while (_pathGenerator.GenerateCrossroads()) ;
            

            pathSize = pathCells.Count;
        }
        

        StartCoroutine(LayGrid(pathCells));
    }

    IEnumerator LayGrid(List<Vector2Int> pathCells)
    {
        yield return LayPathCells(pathCells);
        yield return LaySceneryCells();
        waveManager.SetPathCells(pathCells);
    }

    private IEnumerator LayPathCells(List<Vector2Int> pathCells)
    {
        foreach (Vector2Int pathCell in pathCells)
        {
            int neighbourValue = _pathGenerator.getCellNeighborValue(pathCell.x, pathCell.y);
            GameObject pathTile = gridCells[neighbourValue].cellPrefab;
            GameObject pathtileCell =
                Instantiate(pathTile, new Vector3(pathCell.x, 0f, pathCell.y), Quaternion.identity);
            pathtileCell.transform.Rotate(0f, gridCells[neighbourValue].yRotation, 0f, Space.Self);
            yield return new WaitForSeconds(0.02f);
        }

        yield return null;
        //StartCoroutine(LaySceneryCells());
    }

    private IEnumerator LaySceneryCells()
    {
        for (int y = gridHeight - 1; y > 0; y--)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (_pathGenerator.CellIsEmpty(x, y))
                {
                    int randomIndex = Random.Range(0, sceneryCells.Length);
                    Instantiate(sceneryCells[randomIndex].cellPrefab, new Vector3(x, 0f, y), Quaternion.identity);
                    yield return new WaitForSeconds(0.01f);
                }
            }
        }

        yield return null;
    }
}