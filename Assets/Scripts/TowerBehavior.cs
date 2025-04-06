using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBehavior : MonoBehaviour
{
    public LayerMask EnemiesLayer;
    public float damage, fireRate, range, delay;
    public Transform towerPivot;
    public Enemy target;
    
    void Start()
    {
        delay = 1 / fireRate;
    }

    public void Tick()
    {
        if (target)
        {
            towerPivot.transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);
            
        }
    }
}
