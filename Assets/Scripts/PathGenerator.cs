using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator
{
    private int height, width;
    private List<Vector2Int> pathCells;

    public PathGenerator(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public List<Vector2Int> GenerateEasyPath()
    {
        pathCells = new List<Vector2Int>();
        int y = (int)(height / 2);
        int x = 0;
        while (x < width)
        {
            pathCells.Add(new Vector2Int(x, y));
            bool validMove = false;
            while (!validMove)
            {
                int move = Random.Range(0, 3);
                if (move == 0 || x % 2 == 0 || x > (width - 2)) // continue right
                {
                    x++;
                    validMove = true;
                }
                else if (move == 1 && CellIsEmpty(x, y + 1) && y < (height - 2)) // put a tile up
                {
                    y++;
                    validMove = true;
                }
                else if (move == 2 && CellIsEmpty(x, y - 1) && y > 1) // put a tile down
                {
                    y--;
                    validMove = true;
                }
            }
        }

        return pathCells;
    }


    public bool CellIsEmpty(int x, int y)
    {
        return !pathCells.Contains(new Vector2Int(x, y));
    }

    public bool CellIsTaken(int x, int y)
    {
        return pathCells.Contains(new Vector2Int(x, y));
    }


    public int getCellNeighborValue(int x, int y)
    {
        int returnValue = 0;
        if (CellIsTaken(x, y - 1)) returnValue += 1;
        if (CellIsTaken(x, y + 1)) returnValue += 8;
        if (CellIsTaken(x - 1, y)) returnValue += 2;
        if (CellIsTaken(x + 1, y)) returnValue += 4;
        return returnValue;
    }

    public bool GenerateCrossroads()
    {
        for (int i = 0; i < pathCells.Count; i++)
        {
            Vector2Int pathcell = pathCells[i];

            if (CellIsEmpty(pathcell.x, pathcell.y + 3) &&
                CellIsEmpty(pathcell.x + 1, pathcell.y + 3) &&
                CellIsEmpty(pathcell.x + 2, pathcell.y + 3) &&
                // CellIsEmpty(pathcell.x - 1, pathcell.y + 2) &&
                CellIsEmpty(pathcell.x, pathcell.y + 2) &&
                CellIsEmpty(pathcell.x + 1, pathcell.y + 2) &&
                CellIsEmpty(pathcell.x + 2, pathcell.y + 2) &&
                CellIsEmpty(pathcell.x + 3, pathcell.y + 2) &&
                CellIsEmpty(pathcell.x - 1, pathcell.y + 1) &&
                CellIsEmpty(pathcell.x, pathcell.y + 1) &&
                CellIsEmpty(pathcell.x + 1, pathcell.y + 1) &&
                CellIsEmpty(pathcell.x + 2, pathcell.y + 1) &&
                CellIsEmpty(pathcell.x + 3, pathcell.y + 1) &&
                CellIsEmpty(pathcell.x + 1, pathcell.y) &&
                CellIsEmpty(pathcell.x + 2, pathcell.y) &&
                CellIsEmpty(pathcell.x + 3, pathcell.y) &&
                CellIsEmpty(pathcell.x + 1, pathcell.y - 1) &&
                CellIsEmpty(pathcell.x + 2, pathcell.y - 1))

            {
                pathCells.InsertRange(i + 1,
                    new List<Vector2Int>
                    {
                        new Vector2Int(pathcell.x + 1, pathcell.y), new Vector2Int(pathcell.x + 2, pathcell.y),
                        new Vector2Int(pathcell.x + 2, pathcell.y + 1), new Vector2Int(pathcell.x + 2, pathcell.y + 2),
                        new Vector2Int(pathcell.x + 1, pathcell.y + 2), new Vector2Int(pathcell.x, pathcell.y + 2),
                        new Vector2Int(pathcell.x, pathcell.y + 1)
                    });
                return true;
            }
        }

        return true;
    }
}