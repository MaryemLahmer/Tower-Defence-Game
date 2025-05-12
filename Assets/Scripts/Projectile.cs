using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 15f;
    public float damage = 10f;
    public float explosionRadius = 0f;

    [Header("Effects")]
    public GameObject impactEffect; 
    
    [Header("Audio")]
    public AudioClip impactSound;
    private AudioSource audioSource;
    
    public enum ProjectileType { Arrow, Cannon, Turret, Catapult }
    public ProjectileType type = ProjectileType.Arrow;
    public LayerMask enemyLayer;
    
    private Enemy target;
    private bool hasHit = false;
    private Vector3 lastTargetPosition;

    private void Awake()
    {
        // Add audio source component if it doesn't exist
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // 3D sound
            audioSource.maxDistance = 20f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
        }
        
        // Set the appropriate sound based on projectile type if not already set
        if (impactSound == null)
        {
            SetSoundBasedOnType();
        }
    }

    public void Seek(Enemy _target)
    {
        target = _target;
        if (target != null)
        {
            lastTargetPosition = target.transform.position;
        }
    }

    void Update()
    {
        if (target == null)
        {
            // If target was destroyed, still fly to its last position
            if (lastTargetPosition != Vector3.zero)
            {
                Vector3 dir = lastTargetPosition - transform.position;
                float _distanceThisFrame = speed * Time.deltaTime;
                
                if (dir.magnitude < _distanceThisFrame)
                {
                    // We've reached last known position, explode
                    HitTarget();
                    return;
                }
                
                transform.Translate(dir.normalized * _distanceThisFrame, Space.World);
                
                // Rotate to face direction
                if (dir != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(dir);
                }
            }
            else
            {
                Destroy(gameObject);
            }
            return;
        }

        // Update last known position
        lastTargetPosition = target.transform.position;

        // Move towards target
        Vector3 _dir = target.transform.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;
        if (_dir.magnitude < distanceThisFrame && !hasHit)
        {
            HitTarget();
            return;
        }

        transform.Translate(_dir.normalized * distanceThisFrame, Space.World);

        // Projectile looks in the direction it's traveling
        if (_dir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(_dir);
        }
    }

    void HitTarget()
    {
        hasHit = true;
        
        // Play Impact sound
        PlayImpactSound();
        
        // Create impact effect if available
        if (impactEffect != null)
        {
            GameObject effect = Instantiate(impactEffect, transform.position, Quaternion.identity);
            
            // Scale the effect based on projectile type (optional)
            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                // Optionally adjust scale based on projectile type
                float scaleFactor = 1f;
                switch (type)
                {
                    case ProjectileType.Arrow:
                        scaleFactor = 0.7f;
                        break;
                    case ProjectileType.Cannon:
                        scaleFactor = 1.5f;
                        break;
                    case ProjectileType.Turret:
                        scaleFactor = 1.0f;
                        break;
                    case ProjectileType.Catapult:
                        scaleFactor = 1.8f;
                        break;
                }
                
                // Apply scale to the effect
                effect.transform.localScale *= scaleFactor;
            }
            
            // Destroy after playing
            Destroy(effect, 2f);
        }

        // Apply splash damage if radius >0
        if (explosionRadius > 0) 
            Explode();
        else
        {
            if (target) 
                target.TakeDamage(damage);
        }
        
        // Destroy this projectile
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
                // Calculate damage based on distance
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                float damagePercent = 1f - Mathf.Clamp01(distance / explosionRadius);
                enemy.TakeDamage(damage * damagePercent);
            }
        }
    }

    void PlayImpactSound()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayProjectileImpactSound(type, transform.position);
        }
    }

    void SetSoundBasedOnType()
    {
        // This sets the correct audio based on the path
        string soundPath = "";
        switch (type)
        {
            case ProjectileType.Arrow:
                soundPath = "Audio/ArrowSound";
                break;
            case ProjectileType.Cannon:
                soundPath = "Audio/CannonSound";
                break;
            case ProjectileType.Turret:
                soundPath = "Audio/TurretSound";
                break;
            case ProjectileType.Catapult:
                soundPath = "Audio/CatapultSound";
                break;
        }

        if (!string.IsNullOrEmpty(soundPath))
        {
            impactSound = Resources.Load<AudioClip>(soundPath);
        }
    }
    
    // Method to change the projectile type 
    public void SetProjectileType(ProjectileType newType)
    {
        type = newType;
        SetSoundBasedOnType();
    }
    
   
}