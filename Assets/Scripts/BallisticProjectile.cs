using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallisticProjectile : MonoBehaviour
{
    public float damage = 10f;
    public float splashRadius = 3f;
    public float maxHeight = 5f; // Maximum height of the arc
    public float speed = 10f; // How fast the projectile moves
    public GameObject impactEffect;
    public LayerMask enemyLayer;
    
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float journeyLength;
    private float startTime;
    private bool hasTarget = false;
    private bool hasHit = false;
    
    // Call this to launch the projectile
    public void Launch(Vector3 targetPos)
    {
        startPosition = transform.position;
        targetPosition = targetPos;
        journeyLength = Vector3.Distance(startPosition, targetPosition);
        startTime = Time.time;
        hasTarget = true;
    }
    
    // Alternative method to launch at a specific enemy
    public void LaunchAtEnemy(Enemy enemy)
    {
        if (enemy != null)
        {
            // Aim slightly ahead of the enemy based on their speed
            Vector3 predictedPos = PredictImpactPosition(enemy);
            Launch(predictedPos);
        }
    }
    
    // Predict where the enemy will be when the projectile lands
    private Vector3 PredictImpactPosition(Enemy enemy)
    {
        // Simple prediction assuming the enemy moves at constant speed and direction
        float distanceToTarget = Vector3.Distance(transform.position, enemy.transform.position);
        float timeToImpact = journeyLength / speed;
        
        // Get enemy's velocity (this assumes the enemy has a constant velocity)
        Vector3 enemyDirection = enemy.transform.forward;
        float enemySpeed = enemy.speed;
        
        // Predict position
        Vector3 predictedPosition = enemy.transform.position + (enemyDirection * enemySpeed * timeToImpact);
        
        return predictedPosition;
    }
    
    void Update()
    {
        if (!hasTarget || hasHit)
            return;
            
        // Calculate journey completion percentage
        float distanceCovered = (Time.time - startTime) * speed;
        float fractionOfJourney = distanceCovered / journeyLength;
        
        if (fractionOfJourney >= 1.0f)
        {
            // We've reached the destination
            Impact();
            return;
        }
        
        // Move in arc
        Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);
        
        // Add height using a sine curve for a nice arc
        currentPos.y = startPosition.y + maxHeight * Mathf.Sin(fractionOfJourney * Mathf.PI);
        
        // Update position
        transform.position = currentPos;
        
        // Face in the direction of movement
        if (fractionOfJourney > 0.01f)
        {
            Vector3 previousFramePosition = Vector3.Lerp(startPosition, targetPosition, (fractionOfJourney - 0.01f));
            previousFramePosition.y = startPosition.y + maxHeight * Mathf.Sin((fractionOfJourney - 0.01f) * Mathf.PI);
            
            Vector3 direction = currentPos - previousFramePosition;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
    
    void Impact()
    {
        hasHit = true;
        
        // Show impact effect
        if (impactEffect != null)
        {
            GameObject effect = Instantiate(impactEffect, transform.position, Quaternion.identity);
            Destroy(effect, 3f);
        }
        
        // Apply splash damage
        if (splashRadius > 0)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, splashRadius, enemyLayer);
            foreach (Collider hitCollider in hitColliders)
            {
                Enemy enemy = hitCollider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    // Apply damage falloff based on distance
                    float distance = Vector3.Distance(transform.position, enemy.transform.position);
                    float damagePercent = 1f - Mathf.Clamp01(distance / splashRadius);
                    enemy.TakeDamage(damage * damagePercent);
                }
            }
        }
        
        // Destroy the projectile
        Destroy(gameObject);
    }
    
    // Visualize splash radius in editor
    void OnDrawGizmosSelected()
    {
        if (splashRadius > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, splashRadius);
        }
    }
}