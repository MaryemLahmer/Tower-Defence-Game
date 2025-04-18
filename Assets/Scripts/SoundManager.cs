using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    // Singleton instance
    private static SoundManager _instance;
    public static SoundManager Instance { get { return _instance; } }

    // Audio mixer
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;
    
    // Audio sources
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    
    // Audio clips
    [Header("Music")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;
    
    [Header("Sound Effects")]
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip towerPlacedSound;
    [SerializeField] private AudioClip enemyDefeatedSound;
    
    // Projectile sounds
    [Header("Projectile Sounds")]
    [SerializeField] private AudioClip arrowImpactSound;
    [SerializeField] private AudioClip cannonImpactSound;
    [SerializeField] private AudioClip turretImpactSound;
    
    // Dictionary to store sounds
    private Dictionary<string, AudioClip> soundLibrary = new Dictionary<string, AudioClip>();
    
    // Volume settings
    private const float MIN_DB = -80f; // Minimum decibel value (effectively muted)
    private bool isMusicMuted = false;
    private bool isSfxMuted = false;

    private void Awake()
    {
        // Singleton pattern implementation
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Create audio sources if not assigned
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Setup sources with mixer groups if mixer is assigned
        if (audioMixer != null)
        {
            AudioMixerGroup[] groups = audioMixer.FindMatchingGroups("Master");
            if (groups.Length > 0)
            {
                AudioMixerGroup musicGroup = audioMixer.FindMatchingGroups("Music")[0];
                if (musicGroup != null)
                    musicSource.outputAudioMixerGroup = musicGroup;
                
                AudioMixerGroup sfxGroup = audioMixer.FindMatchingGroups("SFX")[0];
                if (sfxGroup != null)
                    sfxSource.outputAudioMixerGroup = sfxGroup;
            }
        }
        
        // Configure sources
        musicSource.loop = true;
        
        // Initialize sound library
        InitializeSoundLibrary();
        
        // Load saved settings
        LoadSettings();
        
        // Listen for scene changes
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void InitializeSoundLibrary()
    {
        // Add music
        if (menuMusic != null) soundLibrary["MenuMusic"] = menuMusic;
        if (gameMusic != null) soundLibrary["GameMusic"] = gameMusic;
        
        // Add SFX
        if (buttonClickSound != null) soundLibrary["ButtonClick"] = buttonClickSound;
        if (towerPlacedSound != null) soundLibrary["TowerPlaced"] = towerPlacedSound;
        if (enemyDefeatedSound != null) soundLibrary["EnemyDefeated"] = enemyDefeatedSound;
        
        // Add projectile sounds
        if (arrowImpactSound != null) soundLibrary["ArrowImpact"] = arrowImpactSound;
        if (cannonImpactSound != null) soundLibrary["CannonImpact"] = cannonImpactSound;
        if (turretImpactSound != null) soundLibrary["TurretImpact"] = turretImpactSound;
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Play appropriate music based on scene
        if (scene.name == "Menu")
            PlayMenuMusic();
        else if (scene.name == "Game" || scene.name == "Main")
            PlayGameMusic();
    }
    
    // Music control methods
    public void PlayMenuMusic()
    {
        PlayMusic(soundLibrary.ContainsKey("MenuMusic") ? soundLibrary["MenuMusic"] : null);
    }
    
    public void PlayGameMusic()
    {
        PlayMusic(soundLibrary.ContainsKey("GameMusic") ? soundLibrary["GameMusic"] : null);
    }
    
    private void PlayMusic(AudioClip clip)
    {
        if (clip != null && (musicSource.clip != clip || !musicSource.isPlaying))
        {
            musicSource.clip = clip;
            if (!isMusicMuted)
                musicSource.Play();
        }
    }
    
    // SFX methods
    public void PlayButtonSound()
    {
        PlaySoundEffect("ButtonClick");
    }
    
    public void PlayTowerPlacedSound()
    {
        PlaySoundEffect("TowerPlaced");
    }
    
    public void PlayEnemyDefeatedSound()
    {
        PlaySoundEffect("EnemyDefeated");
    }
    
    // Play projectile impact sounds
    public void PlayProjectileImpactSound(Projectile.ProjectileType type, Vector3 position = default)
    {
        string soundKey = "";
        
        switch (type)
        {
            case Projectile.ProjectileType.Arrow:
                soundKey = "ArrowImpact";
                break;
            case Projectile.ProjectileType.Cannon:
                soundKey = "CannonImpact";
                break;
            case Projectile.ProjectileType.Turret:
                soundKey = "TurretImpact";
                break;
        }
        
        if (!string.IsNullOrEmpty(soundKey))
        {
            PlaySoundEffect(soundKey, position);
        }
    }
    
    // Play a sound by key from dictionary
    public void PlaySoundEffect(string soundKey, Vector3 position = default)
    {
        if (soundLibrary.ContainsKey(soundKey) && !isSfxMuted)
        {
            AudioClip clip = soundLibrary[soundKey];
            
            if (position != default)
            {
                // Play sound at position in 3D space
                AudioSource.PlayClipAtPoint(clip, position);
            }
            else
            {
                // Play through our SFX source
                sfxSource.PlayOneShot(clip);
            }
        }
    }
    
    // Get a sound clip by key
    public AudioClip GetSoundClip(string soundKey)
    {
        if (soundLibrary.ContainsKey(soundKey))
        {
            return soundLibrary[soundKey];
        }
        return null;
    }
    
    // Volume control methods
    public void SetMasterVolume(float normalizedValue)
    {
        float dbValue = ConvertToDecibel(normalizedValue);
        audioMixer.SetFloat("MasterVolume", dbValue);
        SaveSettings();
    }
    
    public void SetMusicVolume(float normalizedValue)
    {
        float dbValue = ConvertToDecibel(normalizedValue);
        audioMixer.SetFloat("MusicVolume", dbValue);
        SaveSettings();
    }
    
    public void SetSfxVolume(float normalizedValue)
    {
        float dbValue = ConvertToDecibel(normalizedValue);
        audioMixer.SetFloat("SFXVolume", dbValue);
        SaveSettings();
    }
    
    public void ToggleMusicMute()
    {
        isMusicMuted = !isMusicMuted;
        float db = isMusicMuted ? MIN_DB : ConvertToDecibel(GetMusicVolume());
        audioMixer.SetFloat("MusicVolume", db);
        
        // Also handle direct muting of the source as a backup
        musicSource.mute = isMusicMuted;
        
        SaveSettings();
    }
    
    public void ToggleSfxMute()
    {
        isSfxMuted = !isSfxMuted;
        float db = isSfxMuted ? MIN_DB : ConvertToDecibel(GetSfxVolume());
        audioMixer.SetFloat("SFXVolume", db);
        SaveSettings();
    }
    
    // Convert normalized 0-1 value to decibel scale for audio mixer
    private float ConvertToDecibel(float normalizedValue)
    {
        // Avoid log(0) which is -infinity
        if (normalizedValue <= 0)
            return MIN_DB;
            
        // Convert to logarithmic scale that humans perceive
        return Mathf.Log10(normalizedValue) * 20;
    }
    
    // Convert from decibel to normalized 0-1 value
    private float ConvertFromDecibel(float dbValue)
    {
        if (dbValue <= MIN_DB)
            return 0;
            
        return Mathf.Pow(10, dbValue / 20f);
    }
    
    // Settings persistence
    private void SaveSettings()
    {
        float masterVolume, musicVolume, sfxVolume;
        
        audioMixer.GetFloat("MasterVolume", out masterVolume);
        audioMixer.GetFloat("MusicVolume", out musicVolume);
        audioMixer.GetFloat("SFXVolume", out sfxVolume);
        
        PlayerPrefs.SetFloat("MasterVolumeDB", masterVolume);
        PlayerPrefs.SetFloat("MusicVolumeDB", musicVolume);
        PlayerPrefs.SetFloat("SfxVolumeDB", sfxVolume);
        PlayerPrefs.SetInt("MusicMuted", isMusicMuted ? 1 : 0);
        PlayerPrefs.SetInt("SfxMuted", isSfxMuted ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("MasterVolumeDB"))
        {
            float masterVolume = PlayerPrefs.GetFloat("MasterVolumeDB");
            float musicVolume = PlayerPrefs.GetFloat("MusicVolumeDB");
            float sfxVolume = PlayerPrefs.GetFloat("SfxVolumeDB");
            isMusicMuted = PlayerPrefs.GetInt("MusicMuted") == 1;
            isSfxMuted = PlayerPrefs.GetInt("SfxMuted") == 1;
            
            audioMixer.SetFloat("MasterVolume", masterVolume);
            
            // Apply music volume (accounting for mute)
            if (isMusicMuted)
                audioMixer.SetFloat("MusicVolume", MIN_DB);
            else
                audioMixer.SetFloat("MusicVolume", musicVolume);
                
            // Apply SFX volume (accounting for mute)
            if (isSfxMuted)
                audioMixer.SetFloat("SFXVolume", MIN_DB);
            else
                audioMixer.SetFloat("SFXVolume", sfxVolume);
                
            musicSource.mute = isMusicMuted;
        }
        else
        {
            // Default values for first run
            SetMasterVolume(1.0f);
            SetMusicVolume(0.5f);
            SetSfxVolume(0.7f);
        }
    }
    
    // Getters for UI
    public float GetMasterVolume()
    {
        float dbValue;
        audioMixer.GetFloat("MasterVolume", out dbValue);
        return ConvertFromDecibel(dbValue);
    }
    
    public float GetMusicVolume()
    {
        float dbValue;
        audioMixer.GetFloat("MusicVolume", out dbValue);
        return ConvertFromDecibel(dbValue);
    }
    
    public float GetSfxVolume()
    {
        float dbValue;
        audioMixer.GetFloat("SFXVolume", out dbValue);
        return ConvertFromDecibel(dbValue);
    }
    
    public bool IsMusicMuted() { return isMusicMuted; }
    public bool IsSfxMuted() { return isSfxMuted; }
}