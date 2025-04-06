using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth, health, speed;
    public int id;
    public int currentPathIndex;
    private List<Vector2Int> pathCells;
    private bool reachedEnd = false;
    public void Init()
    {
        health = maxHealth;
        currentPathIndex = 0;
        reachedEnd = false;
    }

    public void SetPath(List<Vector2Int> path)
    {
        pathCells = path;
        if (pathCells != null && pathCells.Count > 0)
        {
            // set intitial position to start of path
            transform.position = new Vector3(pathCells[0].x, 0.2f, pathCells[0].y);
            currentPathIndex = 1;
        } 
    }

    void Update()
    {
        // only move if we have a path and still not reached the end
        if (pathCells != null && pathCells.Count > currentPathIndex && !reachedEnd)
        {
            MoveAlongPath();
        }
    }

    void MoveAlongPath()
    {
        // current position and next position on path
        Vector3 currentPos = transform.position;
        Vector3 nextPos = new Vector3(pathCells[currentPathIndex].x, 0.2f, pathCells[currentPathIndex].y);
        
        // calculate direction to face
        Vector3 direction = nextPos - currentPos;
        if (direction != Vector3.zero)
        {
            // rotate to face movement direction
            transform.rotation = Quaternion.LookRotation(direction);
        }
        
        // move towards next point
        transform.position = Vector3.MoveTowards(currentPos, nextPos, speed * Time.deltaTime);
        
        // check if reached next point
        if (Vector3.Distance(currentPos, nextPos) <= 0.1f)
        {
            currentPathIndex++;
            
            // check if reached the end
            if (currentPathIndex >= pathCells.Count)
            {
                reachedEnd = true;
                OnReachedEnd();
            }
            
        }
    }

    void OnReachedEnd()
    {
        // add trigger to enemy damage 
        // add another trigger to handle score and lives remaining for player 
        GameManager.EnqueueEnemeyToRemove(this);
    }


}
