using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageMethod
{
    // public void DamageTick(Enemy target);
    public void DamageTick(GameObject target); // waiting for enemy part to be ready and change the code
    public void Init(float damage, float fireRat);
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
    public void DamageTick(GameObject target)
    {
        if (delay > 0)
        {
            delay -= Time.deltaTime;
            return;
        }
        
        // apply damage to the enemy here
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy) enemy.TakeDamage(damage);
        
        // reset cooldown
        delay = 1f/fireRate;
    }

  
    
}
