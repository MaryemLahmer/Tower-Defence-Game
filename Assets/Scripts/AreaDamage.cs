using UnityEngine;

public class AreaDamage : MonoBehaviour, IDamageMethod
{
    private float damage, fireRate, delay;
    public float radius = 3f;
    public LayerMask enemyLayer;
    public GameObject effectPrefab;

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

        // Find all enemies in radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, enemyLayer);

        // Create effect if available
        if (effectPrefab != null)
        {
            Instantiate(effectPrefab, transform.position, Quaternion.identity);
        }

        // Apply damage to all enemies in range
        foreach (var hitCollider in hitColliders)
        {
            Enemy enemy = hitCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        // Reset cooldown
        delay = 1f / fireRate;
    }
}