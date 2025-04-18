using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 15f;
    public float damage = 10f;
    public float explosionRadius = 0f;

    public GameObject impactEffect; // particle effect on Hit
    public AudioClip impactSound; // spund effect on hit
    public float soundVolume = 0.5f;
    private AudioSource audioSource;
    public enum ProjectileType {Arrow, Cannon, Turret, Catapult }
    public ProjectileType type = ProjectileType.Arrow;
    
    public LayerMask enemyLayer;
    private Enemy target;
    private bool hasHit = false;


    private void Awake()
    {
        // add audio source component if it doesn't exist
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // 3d sound
            audioSource.maxDistance = 20f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
        }
        
      
        
        // set the appropriate sound based on projectile type if not already set
        if (impactSound == null)
        {
            SetSoundBasedOnType();
        }
    }

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
        
        // Play Impact sound
        PlayImpactSound();
        
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
                // alculate damage based on distance
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                float damagePercent = 1f - Mathf.Clamp01(distance / explosionRadius);
                enemy.TakeDamage(damage * damagePercent);
            }
        }
    }

    void PlayImpactSound()
    {
        // Use the SoundManager to play the projectile impact sound
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayProjectileImpactSound(type, transform.position);
        }
    }


    void SetSoundBasedOnType()
    {
        // this sets the correct audio based on the path
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
    
    // method to change the projectile type 
    public void SetProjectileType(ProjectileType newType)
    {
        type = newType;
        SetSoundBasedOnType();
    }
}