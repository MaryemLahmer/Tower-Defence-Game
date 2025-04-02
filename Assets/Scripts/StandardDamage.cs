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
        // add function to apply damage to the enemy here
        // enemy.applyDamage(damage);
        delay = 1f/fireRate;
    }

  
    
}
