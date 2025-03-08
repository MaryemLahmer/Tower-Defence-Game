using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridWidth = 16;
    public int gridHeight = 8;
    public int minPathLength = 30;
    public GridCellObject[] gridCells;
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
            yield return new WaitForSeconds(0.4f);
        }

        yield return null;
    }

   
}
