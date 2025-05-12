using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamage : MonoBehaviour, IDamageMethod
{
    private float damage, fireRate, delay;

    public GameObject projectilePrefab;
    public Transform firePoint;


    public void Init(float damage, float fireRate)
    {
        this.damage = damage;
        this.fireRate = fireRate;
        delay = 1f / fireRate;
        if (firePoint == null) firePoint = transform;
    }


    public bool DamageTick(GameObject target)
    {
        if (delay > 0)
        {
            delay -= Time.deltaTime;
            return false;
        }

        if (projectilePrefab && target)
        {
            // create projectile
            GameObject projectileObj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            projectileObj.SetActive(true);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            if (projectile)
            {
                Enemy enemy = target.GetComponent<Enemy>();
                projectile.Seek(enemy);
                projectile.damage = damage;
                
                delay = 1f / fireRate;
                return true; 
            }
        }
        
        delay = 1f / fireRate;
        return false; 
    }
}