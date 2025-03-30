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
        int pathSize = pathCells.Count;

        while (pathSize < minPathLength)
        {
            pathCells = _pathGenerator.GenerateEasyPath();
            
           
            

            // for maximum difficulty make the crossroads in a while loop and you'll get more crossroads
            while (_pathGenerator.GenerateCrossroads()) ;

            // for easy levels you can just use the following line of code and it will generate an easy path
            // _pathGenerator.GenerateCrossroads() ;
            
            pathSize = pathCells.Count;
        }
        

        StartCoroutine(LayGrid(pathCells));
    }


    IEnumerator LayGrid(List<Vector2Int> pathCells)
    {
        yield return LayPathCells(pathCells);
        yield return LaySceneryCells();
        waveManager.SetPathCells(_pathGenerator.GenerateRoute());
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
                    GameObject sceneryCell = Instantiate(sceneryCells[randomIndex].cellPrefab, new Vector3(x, 0f, y),
                        Quaternion.identity);
                    if (sceneryCells[randomIndex].isVirginCell) 
                    {
                        sceneryCell.tag = "VirginCell";  
                        sceneryCell.AddComponent<BoxCollider>();

                    }
                    if (sceneryCell.GetComponent<Collider>())
                    {
                    }

                    yield return new WaitForSeconds(0.01f);
                }
            }
        }

        yield return null;
          CreateMapCollider();
    }

    // Add after you've laid all cells
    private void CreateMapCollider()
    {
        GameObject mapCollider = new GameObject("MapCollider");
        BoxCollider collider = mapCollider.AddComponent<BoxCollider>();
        collider.size = new Vector3(gridWidth, 0.1f, gridHeight);
        collider.center = new Vector3((gridWidth - 1) / 2f, 0.1f, (gridHeight - 1) / 2f);
    }

   
}