using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBehavior : MonoBehaviour
{
    public LayerMask EnemiesLayer;
    public float damage = 10f;
    public float fireRate = 1f; // Shots per second
    public float range = 5f;
    
    public Transform towerPivot; // The rotating part of the tower
    public Enemy target;
    
    [Header("Targeting")]
    public TowerTargeting.TargetType targetingMethod = TowerTargeting.TargetType.First;
    public bool showRangeVisually = true;
    
    // Damage method component
    private IDamageMethod damageMethod;
    
    void Start()
    {
        if (towerPivot == null)
            towerPivot = transform;
            
        // Get the damage method component
        damageMethod = GetComponent<IDamageMethod>();
        
        if (damageMethod != null)
        {
            damageMethod.Init(damage, fireRate);
            Debug.Log("damage method instantiated");
        }
        else
        {
            Debug.LogError("No damage method component found on tower!");
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (showRangeVisually)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }

    public void Tick()
    {
        // Find target
        target = TowerTargeting.GetTarget(this, targetingMethod);
        
        if (target != null)
        {
            // Look at target (Y-axis rotation for ground units)
            Vector3 targetPosition = target.transform.position;
            Vector3 direction = targetPosition - towerPivot.position;
            
            // For ground units, set y to same level
            direction.y = 0f;
            
            if (direction != Vector3.zero)
            {
                towerPivot.rotation = Quaternion.LookRotation(direction);
            }
            
            // Apply damage via IDamageMethod
            if (damageMethod != null)
            {
                damageMethod.DamageTick(target.gameObject);
            }
        }
        else
        {
            Debug.Log("no target found");
        }
    }
}