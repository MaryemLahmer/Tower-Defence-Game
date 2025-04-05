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
   


}
