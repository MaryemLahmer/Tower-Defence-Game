using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageMethod
{
    // Update to return bool to indicate when tower fires
    bool DamageTick(GameObject target); 
    void Init(float damage, float fireRate);
}

public class StandardDamage : MonoBehaviour, IDamageMethod
{
    private float damage, fireRate, delay;
    
    public void Init(float damage, float fireRate)
    {
        this.damage = damage;
        this.fireRate = fireRate;
        delay = 1f / fireRate;
    }
    
    public bool DamageTick(GameObject target)
    {
        if (delay > 0)
        {
            delay -= Time.deltaTime;
            return false; // Did not fire this frame
        }
        
        // Apply damage to the enemy here
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy) 
        {
            enemy.TakeDamage(damage);
            
            // Reset cooldown
            delay = 1f/fireRate;
            
            return true; // Successfully fired this frame
        }
        
        return false; // Failed to fire (no enemy component)
    }
}