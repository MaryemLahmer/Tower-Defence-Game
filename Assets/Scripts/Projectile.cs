using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 15f;
    public float damage = 10f;
    public float explosionRadius = 0f;

    public GameObject impactEffect; // particle effect on Hit
    public LayerMask enemyLayer;
    private Enemy target;
    private bool hasHit = false;

    public void Seek(Enemy _target)
    {
        target = _target;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Move towards target
        Vector3 dir = target.transform.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;
        if (dir.magnitude < distanceThisFrame && !hasHit)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);

        // projectile looks in the direction it's traveling
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    void HitTarget()
    {
        hasHit = true;
        // create impact effect if available
        if (impactEffect != null)
        {
            GameObject effect = Instantiate(impactEffect, transform.position, transform.rotation);
            Destroy(effect, 2f);
        }

        // apply splash damage if radius >0
        if (explosionRadius > 0) Explode();
        else
        {
            if (target) target.TakeDamage(damage);
        }
        Destroy(gameObject);
    }

    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, enemyLayer);
        foreach (Collider col in colliders)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy)
            {
                // Optional: calculate damage based on distance
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                float damagePercent = 1f - Mathf.Clamp01(distance / explosionRadius);
                enemy.TakeDamage(damage * damagePercent);
            }
        }
    }
    
    // Visualize explosion radius in editor
    void OnDrawGizmosSelected()
    {
        if (explosionRadius > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}