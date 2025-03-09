using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridWidth = 16;
    public int gridHeight = 8;
    public int minPathLength = 30;
    public GridCellObject[] gridCells;
    public GridCellObject[] sceneryCells; 
    private PathGenerator _pathGenerator;

    void Start()
    {
        _pathGenerator = new PathGenerator(gridWidth, gridHeight);
        List<Vector2Int> pathCells = _pathGenerator.GeneratePath();
        int pathSize = pathCells.Count;
        while (pathSize < minPathLength)
        {
            pathCells = _pathGenerator.GeneratePath();
            pathSize = pathCells.Count;
        }
        
        StartCoroutine(LayPathCells(pathCells));

    }

    private IEnumerator LayPathCells(List<Vector2Int> pathCells)
    {
        foreach (Vector2Int pathCell in pathCells)
        {
            int neighbourValue = _pathGenerator.getCellNeighborValue(pathCell.x, pathCell.y);
            GameObject pathTile = gridCells[neighbourValue].cellPrefab;
            GameObject pathtileCell = Instantiate(pathTile, new Vector3(pathCell.x, 0f, pathCell.y), Quaternion.identity);
            pathtileCell.transform.Rotate(0f, gridCells[neighbourValue].yRotation, 0f, Space.Self);
            yield return new WaitForSeconds(0.25f);
        }

        yield return null;
        StartCoroutine(LaySceneryCells());

    }

    private IEnumerator LaySceneryCells()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (_pathGenerator.CellIsEmpty(x, y))
                {
                    int randomIndex = Random.Range(0, sceneryCells.Length);
                    Instantiate(sceneryCells[randomIndex].cellPrefab, new Vector3(x, 0f, y), Quaternion.identity);
                    yield return new WaitForSeconds(0.1f);

                }
            }
        }

        yield return null;
    }
   
}
