using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatapultDamage : MonoBehaviour, IDamageMethod
{
    private float damage, fireRate, delay;
    public GameObject boulderPrefab;
    public Transform firePoint;
    public float maxArcHeight = 5f;
    public bool predictTargetMovement = true;
    
    // Visual animation fields
    public Transform catapultArm;
    public float armRotationSpeed = 4f;
    private Quaternion restPosition;
    private Quaternion firePosition;
    private bool isAnimating = false;
    
    public void Init(float damage, float fireRate)
    {
        this.damage = damage;
        this.fireRate = fireRate;
        delay = 1f / fireRate;
        
        if (firePoint == null)
            firePoint = transform;
            
        if (catapultArm != null)
        {
            restPosition = catapultArm.rotation;
            firePosition = Quaternion.Euler(restPosition.eulerAngles + new Vector3(-60, 0, 0)); // Adjust as needed
        }
    }
    
    public bool DamageTick(GameObject target)
{
    if (delay > 0)
    {
        delay -= Time.deltaTime;
        return false; // Did not fire, still on cooldown
    }
    
    // Return arm to rest position when not firing
    if (catapultArm != null && !isAnimating)
    {
        catapultArm.rotation = Quaternion.Slerp(catapultArm.rotation, restPosition, Time.deltaTime * armRotationSpeed);
    }
    
    if (delay <= 0 && target != null)
    {
        FireProjectile(target);
        delay = 1f / fireRate;
        return true; // Successfully fired
    }
    
    return false; // Did not fire (no target)
}
    
    private void FireProjectile(GameObject target)
    {
        if (boulderPrefab == null || firePoint == null || target == null)
        {
            Debug.LogError("Missing components for catapult fire!");
            return;
        }
        
        StartCoroutine(FireAnimation(target));
    }
    
    private IEnumerator FireAnimation(GameObject target)
    {
        isAnimating = true;
        
        // Pull back the arm
        if (catapultArm != null)
        {
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * armRotationSpeed;
                catapultArm.rotation = Quaternion.Slerp(restPosition, firePosition, t);
                yield return null;
            }
        }
        
        // Create the boulder
        GameObject projectileObj = Instantiate(boulderPrefab, firePoint.position, firePoint.rotation);
        projectileObj.SetActive(true);
        BallisticProjectile projectile = projectileObj.GetComponent<BallisticProjectile>();
        
        if (projectile != null)
        {
            projectile.damage = damage;
            
            Enemy enemy = target.GetComponent<Enemy>();
            if (predictTargetMovement && enemy != null)
            {
                projectile.LaunchAtEnemy(enemy);
            }
            else
            {
                projectile.Launch(target.transform.position);
            }
            
            projectile.maxHeight = maxArcHeight;
        }
        
        // Release the arm
        if (catapultArm != null)
        {
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * armRotationSpeed * 1.5f; // A bit faster on release
                catapultArm.rotation = Quaternion.Slerp(firePosition, restPosition, t);
                yield return null;
            }
        }
        
        isAnimating = false;
    }
}