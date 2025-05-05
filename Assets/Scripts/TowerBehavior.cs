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
    public Transform firePoint; // Add this

    [Header("Targeting")] 
    public TowerTargeting.TargetType targetingMethod = TowerTargeting.TargetType.First;
    public bool showRangeVisually = true;

    [Header("Audio")]
    [SerializeField] public int towerSoundTypeIndex = 0; // Index into SoundManager's towerFireSounds array
    [SerializeField] private float soundCooldown = 0.1f; // Prevent sound spam
    private float lastSoundTime;
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
            Debug.Log($"Tower initialized with {damageMethod.GetType().Name} damage method");
        }
        else
        {
            Debug.LogError("No damage method component found on tower!");
        }
    }

    // Add this Update method to call Tick every frame
    void Update()
    {
        Tick();
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
            // Modify damageMethod.DamageTick to return bool indicating if it fired
            bool didFire = damageMethod.DamageTick(target.gameObject);
            
            // If tower fired and enough time has passed since last sound
            if (didFire && Time.time > lastSoundTime + soundCooldown)
            {
                // Play firing sound
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayTowerFireSound(towerSoundTypeIndex, transform.position);
                    lastSoundTime = Time.time;
                }
            }
        }
        }
    }
    
    public void PlayFireSound()
{
    if (Time.time > lastSoundTime + soundCooldown)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayTowerFireSound(towerSoundTypeIndex, transform.position);
            lastSoundTime = Time.time;
        }
    }
}
    // Visualize range in Scene view
    void OnDrawGizmosSelected()
    {
        if (showRangeVisually)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}